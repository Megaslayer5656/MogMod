using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class StarlightProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        // texture size is 20x20
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/SmallGrayCircle";
        public static Color Colour => new(163, 218, 255);
        public int Length = 14;
        public int dust = 3;
        public override void SetStaticDefaults()
        {
            // required for texture drawing
            ProjectileID.Sets.TrailCacheLength[Type] = Length;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float dim = .012f * Projectile.scale;
            Lighting.AddLight(Projectile.Center, Colour.R * dim, Colour.G * dim, Colour.B * dim);
            // hover above the players head
            // makes it hover slightly ahead of the player (kinda jank though)
            //Vector2 pos = player.direction == 1 ? new(0f, 30f) : new(-30f, 0f);
            //Vector2 playerPosition = player.Center + Vector2.UnitX * (MathHelper.Clamp(player.velocity.X * 5f, pos.X, pos.Y) * 5f) + Vector2.UnitY * (player.gfxOffY - 30f);
            Vector2 playerPosition = player.Center + Vector2.UnitY * (player.gfxOffY - 30f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, playerPosition, 0.1f);

            // if the players dead, delete proj
            if (player.dead)
                Projectile.Kill();
            // onspawn dust effect
            dust--;
            if (dust >= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(playerPosition, DustID.AncientLight, Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f), 100, Colour, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 1.2f;
                    dust.scale *= 1.15f;
                }
            }
            if (Main.zenithWorld)
            {
                float startScale = 0.0004f;
                float endScale = 30f;
                float chargeTime = 1800f;
                Projectile.scale = Utils.Remap(-dust, 0f, chargeTime, startScale, endScale);
                Projectile.ExpandHitboxBy((int)(Projectile.scale * 50f));
                Projectile.hide = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.LightBlue, .8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, 0f, 0f, 100, default, .8f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool? CanDamage() => false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Main.zenithWorld)
                overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // copied from calamity mod
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            // draws the texture
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, Colour, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            // draws a glow around the projectile
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.55f, SpriteEffects.None);

            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1, texture);
            /*
            // draws additional textures that get smaller the further they are
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Vector2 trailPos = Projectile.oldPos[i] - Main.screenPosition;

                // The further the smaller
                Color trailColor = Color.Lerp(Colour, Color.Black, completionRatio);
                float trailScale = MathHelper.Lerp(0.15f, 1f, 1f - completionRatio);

                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, 0f, texture.Size() * 0.5f, Projectile.scale * trailScale, SpriteEffects.None, 0);
            } */
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}