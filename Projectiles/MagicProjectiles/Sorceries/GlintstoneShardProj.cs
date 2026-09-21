using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Common.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GlintstoneShardProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public static Color Colour => new(171, 237, 255);
        public bool HitEnemy = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 5;
        }
        public override void AI()
        {
            if (!HitEnemy)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                    if (Projectile.alpha < 0) Projectile.alpha = 0;
                }
            }
            if (HitEnemy)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }
            float dim = .003f;
            Lighting.AddLight(Projectile.Center, Colour.R * dim, Colour.G * dim, Colour.B * dim);
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextBool(3) ? 16 : 20);
                dust.scale = Main.rand.NextFloat(0.2f, 0.6f);
                dust.velocity = -Projectile.velocity * 0.8f;
            }
        }
        public override bool? CanDamage() => !HitEnemy;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => KillEffect();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => KillEffect();
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            KillEffect();
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
            return false;
        }
        public void KillEffect()
        {
            if (!HitEnemy)
            {
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
                for (int i = 0; i < 7; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terragrim, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                    int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Colour, 1f);
                    Dust dust3 = Main.dust[dust2];
                    dust3.noGravity = true;
                    dust3.velocity *= 1.2f;
                    dust3.velocity -= Projectile.oldVelocity * 0.3f;
                }
                HitEnemy = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw trail
            TrailDrawer trailDrawer = default;
            trailDrawer.Draw(Projectile, "MagicMissile", Colour, Color.White);

            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(tex, drawPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}