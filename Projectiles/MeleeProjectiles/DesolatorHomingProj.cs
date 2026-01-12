using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    internal class DesolatorHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 13;
            Projectile.height = 13;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 40;
            Projectile.aiStyle = ProjAIStyleID.Sickle;
            AIType = ProjectileID.DeathSickle;
        }
        public override void AI()
        {
            Projectile.localAI[1] += 1f;
            if (Projectile.timeLeft < 160)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] >= 1f)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 900f, 18f, 20f);
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.65f / 255f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(169, 600);
            target.AddBuff(BuffID.BetsysCurse, 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(169, 600);
            target.AddBuff(BuffID.BetsysCurse, 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int j = 0; j < 5; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, Color.DarkRed, 1.2f);
                Main.dust[dust].velocity *= 3f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 0.5f;
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 160)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 160;
    }
}
