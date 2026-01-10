using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class DaedalusProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;
        public int time = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            DrawOffsetX = -40;
            DrawOriginOffsetY = -3;
            DrawOriginOffsetX = 18;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Light
            Lighting.AddLight(Projectile.Center, 0.45f, 0.1f, 0.1f);

            if (time > 8 && Main.rand.NextBool())
            {
                Vector2 dustvel = -Projectile.velocity;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 267, dustvel * Main.rand.NextFloat(0.1f, 1.2f), 0, default, Main.rand.NextFloat(0.7f, 0.9f));
                dust.noGravity = true;
                dust.color = Color.Lerp(Color.LightGoldenrodYellow, Color.PaleVioletRed, Main.rand.NextFloat(0, 1));
            }

            // Update animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;

            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Dazed, 480);
            target.AddBuff(BuffID.Daybreak, 480);
        }

        // Explodes
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DaedalusBoom>(), Projectile.damage / 2, 0, Projectile.owner, 1f, 0f, 0f);
            }
            for (int k = 0; k < 10; k++)
            {
                Vector2 velocity = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.2f);
                float colorRando = Main.rand.NextFloat(0, 1);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity, 278, velocity * Main.rand.NextFloat(0.2f, 1f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.3f, 0.65f);
                dust.color = Color.Lerp(Color.LightGoldenrodYellow, Color.PaleVioletRed, colorRando);
                dust.noLight = true;
                dust.noLightEmittence = true;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, 30, targetHitbox);
    }
}