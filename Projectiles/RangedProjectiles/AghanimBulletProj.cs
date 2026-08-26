using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Graphics;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class AghanimBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public Player Owner => Main.player[Projectile.owner];
        public Color Colour = new(153, 110, 255);
        public float velocityMult = 1f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 1600;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
        }
        public override void OnSpawn(IEntitySource source) => velocityMult = Main.zenithWorld ? 0.1f: Main.rand.NextFloat(0.9f, 0.99f);
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation(); // important so that trails can be drawn correctly
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * 0.5f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.6f), 100);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
            dust.color = Main.rand.NextBool(3) ? Colour : Colour * 0.5f;

            if (Projectile.velocity.Length() < 4) Projectile.velocity += (Owner.MogMod().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.3f;
            else Projectile.velocity *= velocityMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw trail
            TrailDrawer trailDrawer = default;
            trailDrawer.Draw(Projectile, "MagicMissile", Color.White, Colour, minLength: 20, maxLength: 30);

            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(tex, drawPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                Texture2D starTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
                Main.EntitySpriteDraw(starTex, drawPosition, null, Colour * (0.25f + i), Projectile.rotation + MathHelper.PiOver2, starTex.Size() * 0.5f, Projectile.scale * (0.5f + i), SpriteEffects.None);

                Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
                if (i % 0.5f == 0) Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * (0.75f - i), Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * (0.15f + i), SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 600);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 600);
    }
}