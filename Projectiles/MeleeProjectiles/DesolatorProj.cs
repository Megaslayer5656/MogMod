using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class DesolatorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 52;
            Projectile.alpha = 55;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = ProjAIStyleID.Sickle;
            AIType = ProjectileID.DeathSickle;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.65f / 255f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            MogModUtils.HomeInOnNPC(Projectile, true, 700f, 10f, 40f);
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
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 0, 200, 0);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int j = 0; j < 15; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, Color.DarkRed, 1.2f);
                Main.dust[dust].velocity *= 3f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1f;
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                SummonExtraProj();
            }
        }
        private void SummonExtraProj()
        {
            var source = Projectile.GetSource_FromThis();
            float spread = 30f * 0.01f * MathHelper.PiOver2;
            double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            for (int i = 0; i < 2; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 8f * i;
                Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<DesolatorHomingProj>(), Convert.ToInt32(Projectile.damage * .5), Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<DesolatorHomingProj>(), Convert.ToInt32(Projectile.damage * .5), Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
