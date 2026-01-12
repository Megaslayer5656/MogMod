using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    internal class AghanimHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Magic;
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
                Projectile.extraUpdates = 70;
            }
            if (Projectile.localAI[1] > 4f)
            {
                for (int k = 0; k < 4; k++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.RainbowMk2, Projectile.velocity, 100, Color.BlueViolet, 1f);
                    dust.noGravity = true;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 600);
            target.AddBuff(BuffID.StardustMinionBleed, 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Daybreak, 600);
            target.AddBuff(BuffID.StardustMinionBleed, 600);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 16;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int dustAmt = 24;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f; //0.75
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.RainbowTorch, dustDirection.X, dustDirection.Y, 100, Color.BlueViolet, 1f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.RainbowTorch, dustDirection.X, dustDirection.Y, 100, Color.MediumPurple, 1f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
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
