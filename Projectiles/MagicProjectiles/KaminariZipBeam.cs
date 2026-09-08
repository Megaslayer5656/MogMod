using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class KaminariZipBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.extraUpdates = 7;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { Pitch = 0.2f, PitchVariance = 0.1f }, Projectile.Center);
        public override void AI()
        {
            int helixType = (int)Projectile.ai[1];
            float ep = Main.rand.NextFloat(0.01f, 0.1f);
            float stein = Main.rand.NextFloat(5f, 21f);
            float krik = (float)helixType * (float)Math.PI;
            float rick = (float)Math.Sin(Projectile.localAI[0] * ((float)Math.PI * 2f) * ep + krik);
            float trick = (float)Math.Sin((Projectile.localAI[0] + 1f) * ((float)Math.PI * 2f) * ep + krik);
            Projectile.localAI[0] += Main.rand.Next(0, 4);
            float kirk = trick - rick;
            Vector2 vector = (Projectile.velocity.ToRotation() + (float)Math.PI / 2f).ToRotationVector2();
            Projectile.position += vector * kirk * stein;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 speed = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 2; i++)
            {
                int num707 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Electric, speed.X, speed.Y, 100, default, 1.2f);
                Main.dust[num707].noGravity = true;
                Dust dust2 = Main.dust[num707];
                dust2.scale *= 1.25f;
                dust2 = Main.dust[num707];
                dust2.velocity *= Main.rand.NextFloat();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 300);
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), DustID.Electric, (Projectile.velocity * 3) * Main.rand.NextFloat(0.1f, 0.9f));
                dust.scale = Main.rand.NextFloat(0.3f, 0.5f);
                dust.noGravity = true;
            }
        }
    }
}