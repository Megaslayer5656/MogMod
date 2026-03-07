using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class ATGProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 50;
            Projectile.scale = .65f;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 570)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 10f, 25f);
            }

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

            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, 0, 0, 0, default, .75f);
            }

            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.Torch, 0, 0, 0, default, .75f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft < 570)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item73, Projectile.Center); //Might make this a custom sound in the future
        }
    }
}