using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class UndyingPortalProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 350;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
        public override void AI()
        {
            Projectile.rotation += 0.1f;
            // drift to a stop after being launched
            if (Projectile.timeLeft < 330)
                Projectile.velocity *= 0.882f;
            var source = Projectile.GetSource_FromThis();
            if (Projectile.timeLeft <= 300 && Projectile.timeLeft % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);

                Random spawnNumb = new Random();
                int[] amount = { 4, 6, 8 };
                int choice = amount[spawnNumb.Next(amount.Length)];

                float offset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < choice; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / choice) - offset).ToRotationVector2() * (choice / 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<UndyingHomingProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Main.rand.NextBool(3))
                for (int i = 0; i < 4; i++)
                {
                    int deathDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald);
                    Main.dust[deathDust].noGravity = true;
                    Main.dust[deathDust].scale = 1.75f;
                }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 70)
            {
                float timeAlpha = (float)Projectile.timeLeft / 10f;
                Projectile.alpha = (int)(255f - 255f * timeAlpha);
            }
            return new Color(255 - Projectile.alpha, 153 - Projectile.alpha, 204 - Projectile.alpha, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.BlueViolet, Projectile.rotation * 1.2f, texture.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor * 0.8f), Projectile.rotation * 0.8f, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor * 0.8f), -Projectile.rotation * 0.8f, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}