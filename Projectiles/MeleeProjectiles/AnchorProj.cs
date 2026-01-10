using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class AnchorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1f;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            float maxDetectRadius = 500f;
            NPC closestNPC = null;
            float sqrMaxDetectRadius = maxDetectRadius * maxDetectRadius;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this))
                {
                    float sqrDistanceToNPC = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                    if (sqrDistanceToNPC < sqrMaxDetectRadius)
                    {
                        sqrMaxDetectRadius = sqrDistanceToNPC;
                        closestNPC = npc;
                    }
                }
            }
            // If we found a target, home towards it
            if (closestNPC != null)
            {
                Vector2 direction = closestNPC.Center - Projectile.Center;
                direction.Normalize();

                float speed = 5f;
                float turnStrength = 0.05f;

                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    direction * speed,
                    turnStrength
                );

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if (Main.rand.NextBool(15))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++) 
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 2f);
                Main.dust[d].position = Projectile.Center;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WaterCandle, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 2f);
                Main.dust[d].position = Projectile.Center;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
