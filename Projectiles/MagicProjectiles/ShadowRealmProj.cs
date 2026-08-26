using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class ShadowRealmProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        private bool hitEnemy = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 420;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
            Player player = Main.player[Projectile.owner];
            var mogPlayer = Main.LocalPlayer.GetModPlayer<MogPlayer>();
            if (mogPlayer.holdingThrowingShade)
                player.ClearBuff(ModContent.BuffType<ShadowRealmBuff>());
        }
        public override void AI()
        {
            MogModUtils.HomeInOnNPC(Projectile, true, 600f, 13f, 10f);
            if (Projectile.alpha > 0 && !hitEnemy)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0) Projectile.alpha = 0;
            }
            if (hitEnemy)
            {
                Projectile.alpha += 35;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.45f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.1f / 255f);
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[fireDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(10))
            {
                int fireDustSmol = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, 0f, 100, default, 0.6f);
                Main.dust[fireDustSmol].velocity *= 0.25f;
                Main.dust[fireDustSmol].velocity += Projectile.velocity * 0.5f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitEnemy = true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            int numb = 2;
            Projectile.position = Projectile.Center;
            Projectile.width *= 2;
            Projectile.height *= 2;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / numb, Projectile.height / numb, DustID.DemonTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 1.2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width / numb, Projectile.height / numb, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 3f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 1.3f;
                dusty = Dust.NewDust(Projectile.position, Projectile.width / numb, Projectile.height / numb, DustID.DemonTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 1.1f;
            }
        }
        public override bool? CanDamage() => !hitEnemy;
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer trailDrawer = default;
            Color innerColor = new(76, 0, 198);
            Color outerColor = new(215, 189, 255);
            Color innerDrawColor = Projectile.GetAlpha(innerColor);
            Color outerDrawColor = Projectile.GetAlpha(outerColor);
            trailDrawer.Draw(Projectile, "MogMod:FlameLashRGB", outerDrawColor, innerDrawColor, 1.05f, 24f, 30f);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float rotation = MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * 2f;
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, -(rotation * Projectile.direction), texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}