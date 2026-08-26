using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Projectiles.MagicProjectiles
{
    public class LagunaBladeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public static readonly SoundStyle LagunaHit = new($"{nameof(MogMod)}/Sounds/SE/LagunaHit")
        {
            Volume = .8f,
            //PitchVariance = .2f,
            MaxInstances = 1,
        };
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.37f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.47f / 255f);
            if (Main.rand.NextBool(1))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric, Projectile.velocity.X * 1.5f, Projectile.velocity.Y * 1.5f);
                Main.dust[dust].noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f);
                Main.dust[dust].noGravity = true;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LagunaBladeBoom>(), Projectile.damage / 2, 0f, Projectile.owner);
            SoundEngine.PlaySound(LagunaHit, Projectile.Center);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<FierySoulStack>(), 1200);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.fierySoulLevel += 30;
            }
            target.AddBuff(BuffID.Electrified, 420);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<FierySoulStack>(), 1200);
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.fierySoulLevel += 30;
            target.AddBuff(BuffID.Electrified, 420);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer trailDrawer = default;
            Color color = new(0, 186, 242);
            trailDrawer.Draw(Projectile, "MogMod:FlameLashRGB", color, Color.White);

            // draw main proj
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Vector2 position = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            float bloomRotation = Projectile.rotation;
            float texRotation = bloomRotation + MathHelper.PiOver2;
            Vector2 scale = Vector2.One * Projectile.scale;
            Vector2 newScale = scale + scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;
            float x12 = 2f * newScale.X;
            newScale *= 1.8f;
            Vector2 bloomOffset = new Vector2(x12, 0f).RotatedBy(bloomRotation);
            Vector2 texOffset = new Vector2(x12, 0f).RotatedBy(texRotation);
            // pulsing effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                //color * (0.5f + i)
                Main.EntitySpriteDraw(texture, position + texOffset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, Color.White, texRotation, texture.Size() * 0.5f, newScale * 1.2f * i, SpriteEffects.None);
                if (i % 0.5f == 0f) Main.EntitySpriteDraw(bloomTex, position + bloomOffset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, color * (0.5f + i), bloomRotation, bloomTex.Size() * 0.5f, newScale * 0.2f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, position, null, Color.White * 0.5f, texRotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}