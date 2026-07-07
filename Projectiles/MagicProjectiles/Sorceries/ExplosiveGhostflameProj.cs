using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class ExplosiveGhostflameProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 392;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);

            Random spawnNumb = new Random();
            int[] amount = { 4, 6, 8 };
            int choice = amount[spawnNumb.Next(amount.Length)];

            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < choice; i++)
            {
                Vector2 velocity = ((MathHelper.TwoPi * i / choice) - offset).ToRotationVector2() * (choice/2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<GhostflameHomingProj>(), Convert.ToInt32(Projectile.damage / 2), Projectile.knockBack, Projectile.owner);
            }
            for (int n = 0; n < 80; n++)
            {
                int ghostflame = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[]{Color.Black,Color.White}), 1f);
                Main.dust[ghostflame].noGravity = true;
                Main.dust[ghostflame].velocity *= 0f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GhostflameDebuff>(), 420);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GhostflameDebuff>(), 420);
        }
    }
}