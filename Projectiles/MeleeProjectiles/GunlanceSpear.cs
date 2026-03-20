using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class GunlanceSpear : BaseSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";

        // spear code taken from calamity mod
        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 106;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override float InitialSpeed => 2f;
        public override float ReelbackSpeed => 1f;
        public override float ForwardSpeed => 0.7f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            var mogPlayerUI = Main.LocalPlayer.GetModPlayer<MogPlayerUI>();
            // bang bang
            if (mogPlayerUI.gunlanceCurrent > 0 && Gunlance.Blast == true)
            {
                mogPlayerUI.gunlanceCurrent--;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 16f, ModContent.ProjectileType<DaedalusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        };
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(Gunlance.SwingSound2, Projectile.Center);
        }
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
            {
                int idx = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare, Projectile.direction * 2, 0f, 150, default, 1f);
                Main.dust[idx].noGravity = true;
            }
        }
    }
}