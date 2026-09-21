using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class StrongVolatileDagonBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public static Color Colour => DagonFive.WeakColor;
        public bool hitEnemy = false;
        public int timeLeft = 240;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 80;
            Projectile.timeLeft = timeLeft;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.RainbowMk2, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            for (int i = 0; i < 2; i++)
            {
                Vector2 projPos = Projectile.position;
                projPos -= Projectile.velocity * (i * 0.25f);
                int dagonDust = Dust.NewDust(projPos, Projectile.width, Projectile.height, Main.rand.NextBool(2) ? DustID.FireworksRGB : DustID.RainbowMk2, 0f, 0f, 100, Colour, 0.75f);
                Main.dust[dagonDust].noGravity = true;
                Main.dust[dagonDust].position = projPos;
                Main.dust[dagonDust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                Main.dust[dagonDust].velocity *= 0.2f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer) Explode();
            Projectile.velocity = Vector2.Zero;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(2) ? DustID.FireworksRGB : DustID.RainbowMk2, 0f, 0f, 100, Colour, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, Colour, 1f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Daybreak, 480);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Daybreak, 480);
        public void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StrongDagonExplosion>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            float maxOrbs = 5f;
            for (int i = 0; i < maxOrbs; i++)
            {
                Vector2 velocity = ((MathHelper.TwoPi * i / maxOrbs) - offset).ToRotationVector2() * 1.5f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<StrongDagonOrb>(), (int)(Projectile.damage * (1 / maxOrbs)), Projectile.knockBack, Projectile.owner);
            }
        }
    }
}