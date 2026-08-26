using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class PlasmaShrimpProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 20;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // dust id's 71 - 73 are nebula slop
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 71, 0, 0, 100, default, .75f);
            }

            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 72, 0, 0, 100, default, .75f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 73, 0, 0, 100, default, 1f);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            MogPlayer mogPlayer = Owner.GetModPlayer<MogPlayer>();
            if (mogPlayer.plasmaVisual)
                SoundEngine.PlaySound(SoundID.Item72, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;
            // draw trail
            TrailDrawer trailDrawer = default;
            Color outerColor = Color.Violet;
            Color innerColor = Color.MediumPurple;
            trailDrawer.Draw(Projectile, "MagicMissile", outerColor, innerColor);

            // draw main proj
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/SmallGrayCircle").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Vector2 position = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            float rotation = Projectile.rotation;
            Vector2 scale = Vector2.One * Projectile.scale;
            Vector2 newScale = scale + scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;
            float x12 = 2f * newScale.X;
            newScale *= 0.45f;
            Vector2 offset = new Vector2(x12, 0f).RotatedBy(rotation);
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                Main.EntitySpriteDraw(texture, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, innerColor * (0.5f + i), rotation, texture.Size() * 0.5f, newScale, SpriteEffects.None);
                if (i % 0.5f == 0f) Main.EntitySpriteDraw(bloomTex, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, outerColor * (0.25f + i), rotation, bloomTex.Size() * 0.5f, newScale * 0.3f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, position, null, outerColor * 0.5f, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}