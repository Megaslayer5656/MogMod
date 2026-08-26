using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class WaveProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public float MiniWave = 126.86f; // gd miniwave degrees
        public float Wave = 63.43f; // gd wave degrees
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 6;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 300;

            Projectile.friendly = true;

            AIType = ProjectileID.Bullet;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Main.zenithWorld ? 1.25f : 2f;
            if (Main.rand.NextBool(2))
                Projectile.ai[1] = 6;
            float rotation = MathHelper.ToRadians(Main.zenithWorld ? MiniWave : Wave);
            Vector2 velocity = Projectile.velocity;
            Projectile.velocity = velocity.RotatedBy(Projectile.ai[1] == 5 ? -rotation * 0.5f : rotation * 0.5f);
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.rotation = Projectile.velocity.ToRotation();

            /* star of david
            float rotation = MathHelper.ToRadians(63.43f);
            Vector2 velocity = Projectile.velocity;
            if (Projectile.ai[1] == 1)
                Projectile.velocity = velocity.RotatedBy(-rotation);
            */

            float rotation = MathHelper.ToRadians(Main.zenithWorld ? MiniWave : Wave);
            Vector2 velocity = Projectile.velocity;
            if (Projectile.ai[1] == 1)
                Projectile.velocity = velocity.RotatedBy(-rotation);
            if (Projectile.ai[1] == 6)
                Projectile.velocity = velocity.RotatedBy(rotation);
            if (Projectile.ai[1] >= 10)
                Projectile.ai[1] = 0;

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * 0.05f, 100);
                dust.noGravity = true;
                dust.scale = 0.4f;
                dust.color = Color.LightBlue;
                dust.fadeIn = 1.2f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer trailDrawer = default;
            trailDrawer.Draw(Projectile, "MagicMissile", Color.White, Color.LightBlue, 1.4f, 24f, 36);

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Color.LightBlue, Projectile.rotation + MathHelper.PiOver2, bloomTex.Size() * 0.5f, Projectile.scale * 0.35f, SpriteEffects.None);
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Color.LightBlue * 0.1f, Projectile.rotation + MathHelper.PiOver2, bloomTex.Size() * 0.5f, Projectile.scale * 0.9f, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}