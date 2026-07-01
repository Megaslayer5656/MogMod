using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    // code taken from the burning sky projectile from calamity mod
    public class FlameMeteorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int eptein = 120;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 90;
            Projectile.alpha = 150;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            int eintein = Main.rand.Next(1, 4);
            eptein -= eintein;
            if (eptein == 0)
                Projectile.tileCollide = true;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 50f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotate = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    dustRotate += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    dustRotate = dustRotate.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int fire = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch, 0f, 0f, 160, default, 1f);
                    Main.dust[fire].scale = 1.1f;
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].position = Projectile.Center + dustRotate;
                    Main.dust[fire].velocity = Projectile.velocity * 0.1f;
                    Main.dust[fire].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[fire].position) * 1.25f;
                }
            }
            Projectile.alpha -= 15;
            int alpha2 = 150;
            if (Projectile.Center.Y >= Projectile.ai[1])
                alpha2 = 0;
            if (Projectile.alpha < alpha2)
                Projectile.alpha = alpha2;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }
        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 180);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 175)
                return false;
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
