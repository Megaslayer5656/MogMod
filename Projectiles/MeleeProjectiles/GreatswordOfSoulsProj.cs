using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class GreatswordOfSoulsProj : ModProjectile, ILocalizedModType
    {
    public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults() //TODO: Add sounds and potentially a debuff to this projectile
    {
        Projectile.width = 20;
        Projectile.height = 14;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.ArmorPenetration = 50;
    }

    public override void AI()
    {
            Projectile.netUpdate = true;

            Projectile.ai[0] += 1f;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

            Projectile.rotation += MathHelper.ToRadians(-45f);

            int width = Convert.ToInt32(Projectile.width / 2);
        int height = Convert.ToInt32(Projectile.height / 2);
        Vector2 spawn = Projectile.Center - Projectile.velocity / 2f;

        Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
        Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
        
        MogModUtils.HomeInOnNPC(Projectile, true, 1000f, 8f, 20f);

        if (Main.rand.NextBool(2))
        {
            int d = Dust.NewDust(spawn, Projectile.width, Projectile.height, DustID.PurpleTorch);
            Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 0.1f;
        }
    }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.PurpleTorch);
                    Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                }
            }
        }
    }
}
