using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class UndyingHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 26;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 50;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 570)
                MogModUtils.HomeInOnNPC(Projectile, true, 550f, 5f, 25f);

            int undyingDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.ForestGreen, Color.LightGreen }), 1f);
            Main.dust[undyingDust].velocity *= 0.5f;
            Main.dust[undyingDust].scale *= 1.05f;
            Main.dust[undyingDust].fadeIn = 0.7f;
            Main.dust[undyingDust].noGravity = true;

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (-Projectile.velocity).ToRotation();
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.ForestGreen, Color.LightGreen }), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}