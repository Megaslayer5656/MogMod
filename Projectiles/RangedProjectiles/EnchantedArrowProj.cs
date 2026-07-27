using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class EnchantedArrowProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Items/Ammo/Arrows/EnchantedArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.arrow = true;
            Projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.light = 1.2f;
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * -12f;
            int d = Dust.NewDust(Projectile.position, (int)(Projectile.width * 1.5f), (int)(Projectile.height * 1.5f), 15, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
            Main.dust[d].position = Projectile.Center;
            Main.dust[d].noLight = false;
            Dust dust = Dust.NewDustPerfect(Projectile.position, 57, shootVelocity.RotatedByRandom(MathHelper.ToRadians(18f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
            dust.position = Projectile.Center;
            dust.scale = 1.5f;
            dust.alpha = 100;
            dust.noGravity = true;
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 40 && Projectile.ai[0] == 0)
            {
                var source = Projectile.GetSource_FromThis();
                float Spread = 0.1f;
                int enchanted = ModContent.ProjectileType<EnchantedArrowProj>();
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity.RotatedBy(Spread), enchanted, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 1f);
                Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity.RotatedBy(-Spread), enchanted, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 1f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.damage = (int)(Projectile.damage * .95f);
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            int dustsplash = 0;
            while (dustsplash < 16)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width * 2, Projectile.height * 2, Main.rand.NextBool(3) ? 57 : 15, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }
    }
}