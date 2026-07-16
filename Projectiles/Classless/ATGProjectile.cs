using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class ATGProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 14;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 50;
        }
        public override void AI()
        {
            int width = Convert.ToInt32(Projectile.width / 2);
            int height = Convert.ToInt32(Projectile.height / 2);
            Vector2 spawn = Projectile.Center - Projectile.velocity / 2f;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            float velocity = Main.zenithWorld ? 2f : 10f;
            if (Projectile.timeLeft < 570)
                MogModUtils.HomeInOnNPC(Projectile, true, 1500f, velocity, 25f);
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(spawn, DustID.Smoke);
                d.scale = Main.rand.NextFloat(0.8f, 1f);
                d.noGravity = true;
                d.velocity *= 0.1f;
            }
            Dust di = Dust.NewDustPerfect(spawn, DustID.Torch);
            di.scale = Main.rand.NextFloat(1f, 1.2f);
            di.noGravity = true;
            di.velocity *= 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[d].velocity *= 3f;
                Main.dust[d].noGravity = true;
            }
        }
        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 570 ? null : false;
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item73, Projectile.Center); //Might make this a custom sound in the future
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }
    }
}