using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DreadsProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/RangedProjectiles/DrowRangerArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, 0, Color.DeepSkyBlue);
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 224 : 252, new Vector2(Projectile.velocity.X * Main.rand.NextFloat(-0.1f, 0.1f), Projectile.velocity.Y * Main.rand.NextFloat(-0.1f, 0.1f)));
            dust.noGravity = true;
            if (dust.type == 223)
                dust.scale = Main.rand.NextFloat(0.4f, 0.66f);
            else
                dust.scale = Main.rand.NextFloat(0.65f, 0.9f);
        }
        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.LightSkyBlue;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FreezingDebuff>(), 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<FreezingDebuff>(), 600);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            for (int k = 0; k < 4; k++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 224, Projectile.velocity.RotatedByRandom(0.5) * Main.rand.NextFloat(0.1f, 0.9f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 0.7f);
            }
        }
    }
}