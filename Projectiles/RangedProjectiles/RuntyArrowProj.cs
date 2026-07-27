using Microsoft.Xna.Framework;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class RuntyArrowProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Items/Ammo/Arrows/RuntyArrow";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 35;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.arrow = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.position, (int)(Projectile.width * 1.5f), (int)(Projectile.height * 1.5f), DustID.Tungsten, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = false;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            int dustsplash = 0;
            while (dustsplash < 8)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width * 2, Projectile.height * 2, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
            int numProj = 2;
            float rotation = MathHelper.ToRadians(Main.rand.Next(15, 50));
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<RuntyShardProj>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}