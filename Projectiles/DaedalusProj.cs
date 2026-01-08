using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
    public class DaedalusProj : ModProjectile
    {
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;
        public int time = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            DrawOffsetX = -28;
            DrawOriginOffsetY = -2;
            DrawOriginOffsetX = 12;
            if (Projectile.ai[1] == 2)
            {
                Projectile.rotation = (Projectile.velocity.RotatedBy(0.2f)).ToRotation();
            }
            else
            {
                Projectile.rotation = (Projectile.velocity.RotatedBy(-0.2f)).ToRotation();
            }

            // Light
            Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.45f);

            if (time > 8 && Main.rand.NextBool())
            {
                float colorRando = Main.rand.NextFloat(0, 1);
                Vector2 dustvel = Projectile.ai[1] == 2 ? Projectile.velocity.RotatedBy(0.2f) : Projectile.velocity.RotatedBy(-0.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, 261, -dustvel * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(0.4f, 0.6f));
                dust.noGravity = true;
                dust.color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, colorRando);
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

        // Spawns 6 smaller projectiles that slowly glide outward and ignore iframes (use for ranged)
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

            // Individual split projectiles deal 2% damage per hit.
            int numSplits = 6;
            int splitID = ModContent.ProjectileType<DaedalusSplitProj>();
            int damage = (int)(Projectile.damage * 0.02f);
            float angleVariance = MathHelper.TwoPi / numSplits;
            Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numSplits; ++i)
            {
                projVec = projVec.RotatedBy(angleVariance);
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projVec, splitID, damage, 1.5f, Main.myPlayer);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, 15, targetHitbox);
    }
}