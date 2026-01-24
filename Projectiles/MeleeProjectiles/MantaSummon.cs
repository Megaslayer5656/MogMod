using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class MantaSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public Player clone;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            // kill minion if the player isn't holding manta style
            if (Owner.HeldItem.type != ModContent.ItemType<MantaStyle>() || !Owner.active || Owner.CCed || Owner == null)
            {
                Projectile.Kill();
                return;
            }
            // if the velocity is not zero, the visuals get offset weirdly
            Projectile.velocity = Vector2.Zero;
            // how far the clone should move from the player
            Vector2 moveTo = new Vector2(-30, 0);
            switch (Projectile.ai[0])
            {
                case 1:
                    moveTo = new Vector2(30, 0);
                    break;
                default:
                    break;
            }
            // if all conditions above aren't met, the clone can stick around forever
            Projectile.timeLeft = 2;
            // move the clone to the desired position
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center + moveTo, 0.4f);
            // produce smoke during initial move
            if (Projectile.Distance(Owner.Center + moveTo) < 16)
            {
                Projectile.ai[2] = 1;
            }
            if (Projectile.ai[2] == 0)
            {
                int dustsplash = 0;
                while (dustsplash < 4)
                {
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                    Main.dust[d].position = Projectile.Center;
                    dustsplash += 1;
                }
            }
            // shoot beams while the player is left clicking
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1] = 0;
                Vector2 direction = Projectile.Center.DirectionTo(Main.MouseWorld);
                Projectile.direction = Math.Sign(direction.X);
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction * Owner.HeldItem.shootSpeed, ModContent.ProjectileType<MantaProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 1);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // code taken fron "The dark master" weapon from calamity
            // make a player visual clone. it inherits the player's hair type and clothes style and is otherwise all blue with red pupils
            // Main.playerVisualClone[Projectile.owner] will throw stack trace errors on reloads
            clone ??= new Player();
            clone.CopyVisuals(Owner);
            clone.skinColor = Color.DarkBlue;
            clone.shirtColor = Color.DarkBlue;
            clone.underShirtColor = Color.DarkBlue;
            clone.pantsColor = Color.DarkBlue;
            clone.shoeColor = Color.DarkBlue;
            clone.hairColor = Color.DarkBlue;
            clone.eyeColor = Color.White;
            // blue manta effect
            for (int i = 0; i < clone.dye.Length; i++)
            {
                if (clone.dye[i].type != ItemID.BlueFlameDye)
                {
                    clone.dye[i].SetDefaults(ItemID.BlueFlameDye);
                }
            }
            // updates 
            clone.ResetEffects();
            clone.ResetVisibleAccessories();
            clone.DisplayDollUpdate();
            clone.UpdateSocialShadow();
            clone.UpdateDyes();
            clone.PlayerFrame();
            // copy the player's arm movements while swinging, otherwise idle
            if (Owner.ItemAnimationActive && Owner.altFunctionUse != 2)
                clone.bodyFrame = Owner.bodyFrame;
            else
                clone.bodyFrame.Y = 0;
            // legs never jump or walk
            clone.legFrame.Y = 0;
            // face towards the player's cursor
            clone.direction = Math.Sign(Projectile.DirectionTo(Main.MouseWorld).X);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, clone, Projectile.position, 0f, clone.fullRotationOrigin, 0f, 1f);
            // draw the sword
            if (Owner.ItemAnimationActive && Owner.altFunctionUse != 2)
            {
                Texture2D Sword = ModContent.Request<Texture2D>("MogMod/Items/Weapons/Melee/MantaStyle").Value;
                Vector2 distToPlayer = Projectile.position - Owner.position;
                Main.EntitySpriteDraw(Sword, (Vector2)Owner.HandPosition + distToPlayer - Main.screenPosition, null, lightColor, Owner.direction == clone.direction ? Owner.itemRotation : -Owner.itemRotation, new Vector2(clone.direction == 1 ? 0 : Sword.Width, Sword.Height), 1f, clone.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            return false;
        }
        public override bool? CanCutTiles() => true;
        public override bool? CanDamage() => false;
    }
}
