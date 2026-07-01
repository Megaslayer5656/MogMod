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

namespace MogMod.Projectiles.Melee
{
    // did the spear attack type here instead of gunlanceholdout since it was way easier to make a custom spear than do some freaky swing slop
    public class GunlanceSpear : BaseSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

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
            Projectile.localNPCHitCooldown = -1;
        }
        public override float InitialSpeed => 2f;
        public override float ReelbackSpeed => 1f;
        public override float ForwardSpeed => .85f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            var mogPlayerUI = Main.LocalPlayer.GetModPlayer<MogPlayerUI>();
            // bang bang
            if (mogPlayerUI.gunlanceCurrent > 0 && Gunlance.Blast == true)
            {
                mogPlayerUI.gunlanceCurrent--;

                // offsets the projectile so it shoots a little behind the tip instead of at the tip
                float offsetNumb = 60f;
                Vector2 direction = Vector2.Normalize(Projectile.velocity);
                Vector2 offset = Projectile.Center - (direction * offsetNumb);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), offset, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 24f, Gunlance.Bang, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        };
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
        }
    }
}