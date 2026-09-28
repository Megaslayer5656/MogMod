using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class VeryWeakDagonBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public static Color Colour => DagonOne.WeakColor;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 80;
            Projectile.timeLeft = 80;
        }
        public override void AI()
        {
            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            for (int i = 0; i < 4; i++)
            {
                Vector2 projPos = Projectile.position;
                projPos -= Projectile.velocity * (i * 0.25f);
                int dagonDust = Dust.NewDust(projPos, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Flare : DustID.Torch, 0f, 0f, 0, Colour, 0.75f);
                Main.dust[dagonDust].noGravity = true;
                Main.dust[dagonDust].position = projPos;
                Main.dust[dagonDust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                Main.dust[dagonDust].velocity *= 0.2f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.velocity = Vector2.Zero;
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                int fireDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Flare : DustID.Torch, velocity.X, velocity.Y, 100, Colour, 1.2f);
                Main.dust[fireDust].noGravity = Main.rand.Next(5) != 0;
                Dust dust2 = Main.dust[fireDust];
                dust2.scale *= 1f + Main.rand.NextFloat();
                dust2 = Main.dust[fireDust];
                dust2.velocity *= 4.2f;
                if (!Main.dust[fireDust].noGravity)
                {
                    dust2 = Main.dust[fireDust];
                    dust2.scale *= 0.6f;
                    Main.dust[fireDust].fadeIn = 0f;
                    Main.dust[fireDust].noLight = true;
                }
                dust2 = Main.dust[fireDust];
                dust2.velocity *= Main.rand.NextFloat();
            }
            Projectile.Resize(160, 160);
            Projectile.damage = (int)(Projectile.damage * 0.5f);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.knockBack *= 0.35f;
            Projectile.Damage();
            Projectile.Resize(22, 22);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }
    }
}