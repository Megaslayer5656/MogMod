using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    internal class ParasmaHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            //Projectile.extraUpdates = 70; //makes it really fast, almost like a laser
            Projectile.friendly = true;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 20;
        }
        public override void AI()
        {
            Projectile.localAI[1] += 1f;
            if (Projectile.timeLeft < 170)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] >= 1f)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 500f, 15f, 15f);
            }
            if (Projectile.localAI[1] > 4f)
            {
                for (int k = 0; k < 3; k++)
                {
                    int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 0.75f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity *= 0f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 600);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.7f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[dust2].velocity *= 2f;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 170)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 170;
    }
}
