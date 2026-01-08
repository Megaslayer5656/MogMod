using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

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
                for (int i = 0; i < 75; i++)
                {
                    float colorRando = Main.rand.NextFloat(0, 1);
                    float offsetAngle = MathHelper.TwoPi * i / 75f;
                    // Parametric equations for an asteroid.
                    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                    Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY).RotatedByRandom(100) * 15;
                    Dust charged = Dust.NewDustPerfect(Projectile.Center, 267, puffDustVelocity);
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