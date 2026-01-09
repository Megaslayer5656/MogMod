using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class DaedalusBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
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
                int count = 4;
                float thetaDelta = new Vector2(3, 3).RotatedByRandom(100).ToRotation();
                float weaverMin = 0.5f;
                float weaverDistanceMax = 10f;
                float weaverDistanceInner = 0.3f;
                for (float theta = 0f; theta < MathHelper.TwoPi; theta += 0.05f)
                {
                    float colorRando = Main.rand.NextFloat(0, 1);
                    Vector2 velocity = theta.ToRotationVector2() *
                        (weaverMin + (float)(Math.Sin(thetaDelta + theta * count) + weaverMin - weaverDistanceInner) * weaverDistanceMax);
                    Dust charged = Dust.NewDustPerfect(Projectile.Center, 267, velocity);
                    charged.scale = 1.5f;
                    charged.fadeIn = 0.5f;
                    charged.color = Color.Lerp(Color.PaleVioletRed, Color.MediumVioletRed, colorRando);
                    charged.noGravity = true;
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