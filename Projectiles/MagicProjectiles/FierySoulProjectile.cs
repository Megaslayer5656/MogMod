using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class FierySoulProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public static Color Colour => new(255, 179, 87);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * (Projectile.scale * 0.5f));
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fourConst = 4;
                int fireDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)fourConst, Projectile.position.Y + (float)fourConst), Projectile.width - fourConst * 2, Projectile.height - fourConst * 2, DustID.InfernoFork, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[fireDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(10))
            {
                int otherFourConst = 4;
                int fireDustSmol = Dust.NewDust(new Vector2(Projectile.position.X + (float)otherFourConst, Projectile.position.Y + (float)otherFourConst), Projectile.width - otherFourConst * 2, Projectile.height - otherFourConst * 2, DustID.InfernoFork, 0f, 0f, 100, default, 0.6f);
                Main.dust[fireDustSmol].velocity *= 0.25f;
                Main.dust[fireDustSmol].velocity += Projectile.velocity * 0.5f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 15; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.InfernoFork, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 420);
            Player player = Main.player[Projectile.owner];
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<FierySoulStack>(), 600);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.fierySoulLevel += 1;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 420);
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<FierySoulStack>(), 600);
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.fierySoulLevel += 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);
            }
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Colour * 0.7f, 1, bloomTex, scale: Projectile.scale * 0.3f);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}