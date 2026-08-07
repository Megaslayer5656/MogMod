using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class SangeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 180;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<Sange>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<Sange>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int AfterImageLength => 8;
        public override int OffsetDistance => 36;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item1;
        public bool playSwingSound = true;
        public bool hitNPC = false;
        public override void Defaults()
        {
            MogModGlobalProjectile mogProj = Projectile.MogMod();
            mogProj.bloodDamage = Sange.BloodDamage;
        }
        public override void Spawn()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (Main.myPlayer == Projectile.owner)
                mogPlayer.swingNum = mogPlayer.swingNum++ % 2;
            StartupTime = 8;
            CooldownTime = 4;
            swingTime -= mogPlayer.swingNum % 2 == 0 ? -StartupTime : StartupTime - CooldownTime;
            Projectile.scale *= mogPlayer.swingNum % 2 == 0 ? 2.8f : 2.2f;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            switch (mogPlayer.swingNum)
            {
                // big swing
                case 0:
                    if (playSwingSound && !inStartup)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.05f, -0.25f) }, Projectile.Center);
                        playSwingSound = false;
                    }
                    //Main.NewText($"big evil swing {mogPlayer.swingNum}");
                    break;
                // small swing
                case 1:
                    //Main.NewText($"small swing {mogPlayer.swingNum}");
                    break;
            }
            if (inStartup) Projectile.scale = baseScale * MathHelper.Lerp(0.5f, 1, 1 - MathF.Pow(1 - StartupCompletion, 2f));
            else if (inCooldown) Projectile.scale = baseScale * MathHelper.Lerp(1, 0.75f, MathF.Pow(CooldownCompletion, 2));
            else Projectile.scale = baseScale * Math.Min(MathHelper.SmoothStep(1, 1.5f, SwingCompletion), MathHelper.SmoothStep(2, 1, SwingCompletion));
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.5f, -swingWidth * 0.75f, 1 - MathF.Pow(1 - StartupCompletion, 2f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.25f, swingWidth * 0.33f, CooldownCompletion));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.75f, swingWidth * 0.25f, SwingCompletion));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            MogModGlobalProjectile mogProj = Projectile.MogMod();
            if (mogPlayer.swingNum == 0)
            {
                modifiers.SourceDamage *= 2.2f;
                modifiers.Knockback += 1;
                if (!hitNPC)
                {
                    mogProj.bloodDamage = (int)(mogProj.bloodDamage * 1.2f);
                    hitNPC = true;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy && hit.Crit)
            {
                int heal = Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum == 0 ? 4 : 2;
                Owner.HealLifestealMult(heal);
            }
        }
    }
}