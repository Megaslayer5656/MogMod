using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class LagunaBladeBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        private const float radius = 50f;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void AI()
        {
            if (Projectile.timeLeft >= 8)
            {
                for (int i = 0; i < 30; i++)
                {
                    // makes it a random dust particle
                    //int dustType = Main.rand.NextBool() ? 132 : 264;

                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 6;

                    int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.t_Martian, dustVelocity.X, dustVelocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<FierySoulStack>(), 1200);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.fierySoulLevel += 30;
            }
            target.AddBuff(BuffID.Electrified, 420);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<FierySoulStack>(), 1200);
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.fierySoulLevel += 30;
            target.AddBuff(BuffID.Electrified, 420);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}