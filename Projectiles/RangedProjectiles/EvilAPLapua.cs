using Microsoft.Xna.Framework;
using MogMod.Items.Ammo.Bullets;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class EvilAPLapua : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => MiscUtils.GetItemName<EvilAPLapuaAmmo>();
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;

            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;


            Projectile.light = .5f;
            Projectile.timeLeft = 270;
            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            AIType = ProjectileID.Bullet;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }
        public override void AI()
        {
            Projectile.localAI[1] += 1f;
            if (Projectile.timeLeft < 240)
                Projectile.velocity *= 0.932f;
            if (Projectile.timeLeft < 200)
                Projectile.ai[0] = 1f;
            if (Projectile.ai[0] >= 1f)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 15f, 15f);
                Projectile.extraUpdates = 70;
            }
            if (Projectile.localAI[1] > 4f)
                for (int k = 0; k < 1; k++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.RainbowMk2, Projectile.velocity, 100, Main.zenithWorld ? Color.BlueViolet : Color.SkyBlue, 1f);
                    dust.noGravity = true;
                }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 1f)
                return false;
            return null;
        }
        public override bool CanHitPvp(Player target) => Projectile.ai[0] < 1f;
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}