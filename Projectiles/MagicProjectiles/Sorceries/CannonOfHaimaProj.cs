using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CannonOfHaimaProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.HallowSpray, Projectile.velocity.X * 1.5f, Projectile.velocity.Y * 1.5f);
            Main.dust[dust].scale = 2f;
            Main.dust[dust].noGravity = true;
            int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric, Projectile.velocity.X * 1.5f, Projectile.velocity.Y * 1.5f);
            Main.dust[dust2].scale = 1.8f;
            Main.dust[dust2].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft > 580 && Projectile.velocity.Y <= 5)
                Projectile.velocity.Y = Projectile.velocity.Y - 0.3f;
            else
                Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            int explosionDamage = Projectile.damage;
            float explosionKB = 6f;
            SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CannonOfHaimaBoom>(), Convert.ToInt32(explosionDamage * .65), explosionKB, Projectile.owner);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}