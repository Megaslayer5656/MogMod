using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class DaedalusBoom : ModProjectile
    {
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            // Light
            Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.45f);

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 0)
            {
                int flowerPetalCount = 6;
                float thetaDelta = new Vector2(3, 3).RotatedByRandom(100).ToRotation();
                float weaveDistanceMin = 0.5f;
                float weaveDistanceOutwardMax = 10f;
                float weaveDistanceInner = 0.5f;
                for (float theta = 0f; theta < MathHelper.TwoPi; theta += 0.05f)
                {
                    float colorRando = Main.rand.NextFloat(0, 1);
                    Vector2 velocity = theta.ToRotationVector2() *
                        (weaveDistanceMin +
                        // The 0.5 in here is to prevent the petal from looping back into itself. With a 0.5 addition, it is perfect, coming back to (0,0)
                        // instead of weaving backwards.
                        (float)(Math.Sin(thetaDelta + theta * flowerPetalCount) + 0.5f + weaveDistanceInner) *
                        weaveDistanceOutwardMax);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 267, velocity);
                    dust.noGravity = true;
                    dust.scale = 1.15f;
                    dust.color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, colorRando);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.88f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, Projectile.ai[0] == 1 ? 130 : 200, targetHitbox);
    }
}