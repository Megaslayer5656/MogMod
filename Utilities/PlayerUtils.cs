using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories.Boots;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Items.Armor.Radiant;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;
using static Terraria.Player;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        public static void SendPacket(this Player player, ModPacket packet, bool server)
        {
            // Client: Send the packet only to the host.
            if (!server)
                packet.Send();

            // Server: Send the packet to every OTHER client.
            else
                packet.Send(-1, player.whoAmI);
        }
        /// <summary>
        /// Calculates and returns the player's total melee scale boosts. This is used mostly for melee holdouts.
        /// </summary>
        /// <param name="addHeldItemScale">If the item scale of the players held item should be added to the calculation.<br/>
        /// Should be disabled for anything that doesn't lock you into holding one item.
        /// </param>
        public static float GetMeleeScale(this Player player, bool addHeldItemScale = true)
        {
            MogPlayer mogPlayer = player.MogMod();
            float baseScale = 1;
            player.ApplyMeleeScale(ref baseScale); // Gets vanilla's glove scale boosts
            if (addHeldItemScale)
                baseScale += (player.HeldItem.scale - 1);

            if (mogPlayer.wearingGiantsMaul)
                baseScale += GiantsMaul.SizeMult + (Main.zenithWorld ? -1.1f : 0);
            if (mogPlayer.wearingTreadsDamage)
                baseScale += PowerTreads.SizeMult;

            return baseScale;
        }
        /// <summary>
        /// Heals the player while accounting for modded life multipliers.
        /// Does not work with other mods.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="life">The amount of life healed.</param>
        public static void HealLifeMult(this Player player, int life)
        {
            MogPlayer mogPlayer = player.MogMod();
            double lifeMult = 1 +
            (mogPlayer.wearingMending ? MendingAspect.LifeMult : 0D);
            if (mogPlayer.healingDisabledDebuff)
                lifeMult = 0D;
            life = (int)(life * lifeMult);
            player.statLife += life;
            player.HealEffect(life);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
        }
        /// <summary>
        /// Heals the player's mana while accounting for modded mana multipliers.
        /// Does not work with other mods.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mana">The amount of mana healed.</param>
        public static void HealManaMult(this Player player, int mana)
        {
            MogPlayer mogPlayer = player.MogMod();
            if (mogPlayer.wearingRadiantArmor)
                mana = (int)(mana * (RadiantFlower.ManaMult + 1));
            player.statMana += mana;
            player.ManaEffect(mana);
            if (player.statMana > player.statManaMax2)
                player.statMana = player.statManaMax2;
        }
        /// <summary>
        /// Applies lifesteal to the player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="lifesteal">The amount of life healed.</param>
        public static void HealLifestealMult(this Player player, int lifesteal)
        {
            lifesteal *= (int)(player.lifeSteal * 0.02f);
            player.statLife += lifesteal;
            player.HealEffect(lifesteal);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
        }
        /// <summary>
        /// Gets an arm stretch amount from a number ranging from 0 to 1
        /// </summary>
        public static CompositeArmStretchAmount ToStretchAmount(this float percent)
        {
            if (percent < 0.25f)
                return CompositeArmStretchAmount.None;
            if (percent < 0.5f)
                return CompositeArmStretchAmount.Quarter;
            if (percent < 0.75f)
                return CompositeArmStretchAmount.ThreeQuarters;

            return CompositeArmStretchAmount.Full;
        }
        /// <summary>
        /// Properly sets the player's held item rotation and position by doing the annoying math for you, since vanilla decided to be wholly inconsistent about it!
        /// This all assumes the player is facing right. All the flip stuff is automatically handled in here
        /// </summary>
        /// <param name="player">The player for which we set the hold style</param>
        /// <param name="desiredRotation">The desired rotation of the item</param>
        /// <param name="desiredPosition">The desired position of the item</param>
        /// <param name="spriteSize">The size of the item sprite (used in calculations)</param>
        /// <param name="rotationOriginFromCenter">The offset from the center of the sprite of the rotation origin</param>
        /// <param name="noSandstorm">Should the swirly effect from the sandstorm jump be disabled</param>
        /// <param name="flipAngle">Should the angle get flipped with the player, or should it be rotated by 180 degrees</param>
        /// <param name="stepDisplace">Should the item get displaced with the player's height during the walk anim? </param>
        public static void CleanHoldStyle(Player player, float desiredRotation, Vector2 desiredPosition, Vector2 spriteSize, Vector2? rotationOriginFromCenter = null, bool noSandstorm = false, bool flipAngle = false, bool stepDisplace = true)
        {
            if (noSandstorm)
                player.sandStorm = false;

            //Since Vector2.Zero isn't a compile-time constant, we can't use it directly as the default parameter
            if (rotationOriginFromCenter == null)
                rotationOriginFromCenter = Vector2.Zero;

            Vector2 origin = rotationOriginFromCenter.Value;
            //Flip the origin's X position, since the sprite will be flipped if the player faces left.
            origin.X *= player.direction;
            //Additionally, flip the origin's Y position in case the player is in reverse gravity.
            origin.Y *= player.gravDir;

            player.itemRotation = desiredRotation;

            if (flipAngle)
                player.itemRotation *= player.direction;
            else if (player.direction < 0)
                player.itemRotation += MathHelper.Pi;

            //This can anchors the item to rotate around the middle left of its sprite
            //Vector2 consistentLeftAnchor = (player.itemRotation).ToRotationVector2() * -10f * player.direction;

            //This anchors the item to rotate around the center of its sprite.
            Vector2 consistentCenterAnchor = player.itemRotation.ToRotationVector2() * (spriteSize.X / -2f - 10f) * player.direction;

            //This shifts the item so it rotates around the set origin instead
            Vector2 consistentAnchor = consistentCenterAnchor - origin.RotatedBy(player.itemRotation);

            //The sprite needs to be offset by half its sprite size.
            Vector2 offsetAgain = spriteSize * -0.5f;

            Vector2 finalPosition = desiredPosition + offsetAgain + consistentAnchor;

            //Account for the players extra height when stepping
            if (stepDisplace)
            {
                int frame = player.bodyFrame.Y / player.bodyFrame.Height;
                if ((frame > 6 && frame < 10) || (frame > 13 && frame < 17))
                {
                    finalPosition -= Vector2.UnitY * 2f;
                }
            }

            player.itemLocation = finalPosition + new Vector2(spriteSize.X * 0.5f, 0);
        }

        /// <summary>
        /// Gives the player the specified number of immunity frames (or "iframes" for short) to all cooldown slots.<br />
        /// If the player already has more iframes than you want to give them, this function does nothing.<br />
        /// <br />
        /// <b>This should be used for effects like dodges or true invulnerability that should prevent the player from being hit for a predetermined time.</b>
        /// </summary>
        /// <param name="player">The player who should be given immunity frames.</param>
        /// <param name="frames">The number of immunity frames to give.</param>
        /// <param name="blink">Whether or not the player should be blinking during this time.</param>
        /// <returns>Whether or not any immunity frames were given.</returns>
        public static bool GiveUniversalIFrames(this Player player, int frames, bool blink = false)
        {
            // Check to see if there is any way for the player to get iframes from this operation.
            bool anyIFramesWouldBeGiven = false;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    anyIFramesWouldBeGiven = true;

            // If they would get nothing, don't do it.
            if (!anyIFramesWouldBeGiven)
                return false;

            // Apply iframes thoroughly. Player.AddImmuneTime is not used because iframes should not exceed the intended amount.
            player.immune = true;
            player.immuneNoBlink = !blink;
            player.immuneTime = frames;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    player.hurtCooldowns[i] = frames;

            return true;
        }
        /// <summary>
        /// Computes the appropriate amount of immunity frames to grant a player when they activate a dodge.<br />
        /// Accounts for all MogMod effects, but not effects from other mods.
        /// </summary>
        /// <param name="player">The player whose immunity frames are being computed.</param>
        /// <returns>The amount of immunity frames the player should receive upon dodging.</returns>
        public static int ComputeDodgeIFrames(this Player player)
        {
            int iframes = 80 + (player.longInvince ? 30 : 0);
            return iframes;
        }

        // Currently, reflects are functionally equivalent to dodges.
        /// <summary>
        /// Computes the appropriate amount of immunity frames to grant a player when they activate a reflect.<br />
        /// Accounts for all MogMod effects, but not effects from other mods.
        /// </summary>
        /// <param name="player">The player whose immunity frames are being computed.</param>
        /// <returns>The amount of immunity frames the player should receive upon reflecting an attack.</returns>
        public static int ComputeReflectIFrames(this Player player) => player.ComputeDodgeIFrames();
    }
}
