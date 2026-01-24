using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Config;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Global
{
    public class MogModGlobalNPC : GlobalNPC
    {
        // do something for eye of skadi

        // debuffs ID
        public int divineDebuff = 0;
        public int skadiDebuff = 0;

        // apparently neccessary according to calamity
        public override bool InstancePerEntity => true;
        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            MogModGlobalNPC myClone = (MogModGlobalNPC)base.Clone(npc, npcClone);
            myClone.divineDebuff = divineDebuff;
            myClone.skadiDebuff = skadiDebuff;
            return myClone;
        }

        // actual debuff effect
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (divineDebuff > 0)
            {
                ApplyDPSDebuff(400, 100, ref npc.lifeRegen, ref damage);
            }
            if (skadiDebuff > 0)
            {
                ApplyDPSDebuff(200, 40, ref npc.lifeRegen, ref damage);
            }

        }

        // not quite sure what this does, but its in calamity mod so it has to be important
        public override void PostAI(NPC npc)
        {
            if (divineDebuff > 0)
                divineDebuff--;
            if (skadiDebuff > 0)
            {
                skadiDebuff--;
                npc.velocity *= 0.988f;
            }
        }

        // LEDX and REDX chance to drop
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new CommonDrop(ModContent.ItemType<LedX>(), 10000, 1, 1, 1));
            globalLoot.Add(new CommonDrop(ModContent.ItemType<RedX>(), 100000, 1, 1, 1));
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                hurtInfo = new Player.HurtInfo
                {
                    Damage = 1,
                    Knockback = 0,
                    HitDirection = 0,
                    Dodgeable = false,
                    SoundDisabled = true
                };

                var hitInfo = new NPC.HitInfo
                {
                    Damage = 20,
                    Knockback = 5,
                    HitDirection = target.direction,
                    Crit = false,
                    DamageType = DamageClass.Generic
                };
                npc.StrikeNPC(hitInfo); //Must use this instead of modifying the npc's life stat
                NetMessage.SendStrikeNPC(npc, hitInfo);
            }
        }

        // debuff visual effects
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            drawColor = npc.GetNPCColorTintedByBuffs(drawColor);
            if (divineDebuff > 0)
            {
                DivineMightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.NavajoWhite;
            }
            if (skadiDebuff > 0)
            {
                EyeOfSkadiDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.DarkSlateBlue;
            }
        }

        // QOL for making debuff damage easier
        public void ApplyDPSDebuff(int lifeRegenValue, int damageValue, ref int lifeRegen, ref int damage)
        {
            if (lifeRegen > 0)
                lifeRegen = 0;

            lifeRegen -= lifeRegenValue;

            if (damage < damageValue)
                damage = damageValue;
        }
        public static void DrawAfterimage(NPC npc, SpriteBatch spriteBatch, Color startingColor, Color endingColor, Texture2D texture = null, Func<NPC, int, float> rotationCalculation = null, bool directioning = false, bool invertedDirection = false)
        {
            if (NPCID.Sets.TrailingMode[npc.type] != 1)
                return;

            SpriteEffects spriteEffects = SpriteEffects.None;

            if (npc.spriteDirection == -1 && directioning)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (invertedDirection)
                spriteEffects ^= SpriteEffects.FlipHorizontally; // Same as x XOR 1, or x XOR TRUE, which inverts the bit. In this case, this reverses the horizontal flip

            // Set the rotation calculation to a predefined value. The null default is solely so that
            if (rotationCalculation is null)
                rotationCalculation = (nPC, afterimageIndex) => nPC.rotation;

            endingColor.A = 0;

            Color drawColor = npc.GetAlpha(startingColor);
            Texture2D npcTexture = texture ?? TextureAssets.Npc[npc.type].Value;
            Vector2 screenOffset = npc.IsABestiaryIconDummy ? Vector2.Zero : Main.screenPosition;
            int afterimageCounter = 1;
            while (afterimageCounter < NPCID.Sets.TrailCacheLength[npc.type] && MogClientConfig.Instance.Afterimages)
            {
                Color colorToDraw = Color.Lerp(drawColor, endingColor, afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type]);
                colorToDraw *= afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type];
                spriteBatch.Draw(npcTexture,
                                 npc.oldPos[afterimageCounter] + npc.Size / 2f - screenOffset + Vector2.UnitY * npc.gfxOffY,
                                 npc.frame,
                                 colorToDraw,
                                 rotationCalculation.Invoke(npc, afterimageCounter),
                                 npc.frame.Size() * 0.5f,
                                 npc.scale,
                                 spriteEffects,
                                 0f);
                afterimageCounter++;
            }
        }
    }
}