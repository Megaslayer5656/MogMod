using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class SandstormProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        private const float radius = 50f;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
        public override void AI()
        {
            for (int n = 0; n < 6; n++)
            {
                float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                Vector2 swirlPos = Projectile.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 30f;
                Vector2 swirlVelocity = Vector2.Normalize(swirlPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(40)) * 2f;
                Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.GoldCoin, swirlVelocity * Main.rand.NextFloat(2f, 4f), 0, default, 1.5f);
                swirlDust.fadeIn = 0.25f;
                swirlDust.noGravity = true;
            }
            // fire aura
            for (int i = 0; i < 30; i++)
            {
                float randomAngle = Main.rand.NextFloat() * MathHelper.TwoPi;
                float outwardnessFactor = Main.rand.NextFloat();
                Vector2 spawnPosition = Projectile.Center + randomAngle.ToRotationVector2() * MathHelper.Lerp(0f, 10f, outwardnessFactor);
                Vector2 velocity = (randomAngle - 3f * MathHelper.Pi / 8f).ToRotationVector2() * (4f + 3f * Main.rand.NextFloat() + 1f * outwardnessFactor);
                Dust swirlingDust = Dust.NewDustPerfect(spawnPosition, 124, new Vector2?(velocity), 0, default, 1f);
                swirlingDust.fadeIn = 0.25f + outwardnessFactor * 0.05f;
                swirlingDust.noGravity = true;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}