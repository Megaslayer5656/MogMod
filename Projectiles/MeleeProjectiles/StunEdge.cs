using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using MogMod.Buffs.Debuffs;
using Terraria.DataStructures;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class StunEdge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public static readonly SoundStyle stunEdgeSFX = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/StunEdge")
        {
            Volume = .67f,
            PitchVariance = .02f,
        };
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.scale = .60f;

            AIType = ProjectileID.Bullet;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ShockState>(), 1500);
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                int stunEdgeKill = Dust.NewDust(Projectile.position, 1, 1, DustID.Electric, 0, 0, 0, default, 1f);
            }
        }

        public override void AI()
        {
            if (Main.rand.NextBool(15))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 1.25f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        
        public override void OnSpawn(IEntitySource source)
        {
        SoundEngine.PlaySound(stunEdgeSFX, Projectile.Center);
        }
    }
}
