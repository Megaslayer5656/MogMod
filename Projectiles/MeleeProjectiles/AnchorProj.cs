using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class AnchorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        private static readonly List<SoundStyle> randomSound = new List<SoundStyle>
        {
            SoundID.Seagull,
            SoundID.Dolphin,
            SoundID.Duck
        };
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 66;

            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            MogModUtils.HomeInOnNPC(Projectile, true, 400, 8f, 20f);

            if (Main.rand.NextBool(15))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++) 
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 2f);
                Main.dust[d].position = Projectile.Center;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            int chosenSound = Main.rand.Next(randomSound.Count);
            SoundEngine.PlaySound(randomSound[chosenSound], Projectile.Center);
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WaterCandle, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 2f);
                Main.dust[d].position = Projectile.Center;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}