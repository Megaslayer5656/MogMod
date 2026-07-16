using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class FoundingRainOfStarsStarProj : ModProjectile, ILocalizedModType
    {
        // code taken from "universal genesis" from calamity mod;
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.alpha = 50;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 170)
                MogModUtils.HomeInOnNPC(Projectile, true, 250f, 12f, 15f);
            if (Projectile.soundDelay == 0 && Projectile.ai[0] == 0f)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            if (Main.rand.NextBool(48) && !Main.dedServ)
            {
                int idx = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, 16, 1f);
                Main.gore[idx].velocity *= 0.66f;
                Main.gore[idx].velocity += Projectile.velocity * 0.3f;
            }
            if (Main.rand.NextBool(10))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
            if (Main.rand.NextBool(20) && !Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
        }
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);
        public override bool PreDraw(ref Color lightColor)
        {
            // calamity mod custom fallen star-like effect;
            Projectile.DrawStarTrail(Color.Blue, Color.White);

            //Draw the actual projectile
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position += Projectile.Size;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position -= Projectile.Size;
            for (int i = 0; i < 5; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 100, default, 1.2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
        }
    }
}