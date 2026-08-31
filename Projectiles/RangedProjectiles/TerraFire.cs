using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class TerraFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/SmokeParticle";
        public static int Lifetime => 116;
        public static int Size => 52;
        public ref float ColorType => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public int NumAnimationFrames = 7;
        public int MaxPenetrate = 5;
        public bool HitTile = false;
        public float MaxFlameTypes = 0.07f;
        public Color FireColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = Size;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime; // 24
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = Terrablazer.ArmorPenetration;
        }
        public override void OnSpawn(IEntitySource source) => MaxFlameTypes = Main.rand.NextFloat(0.03f, 0.1f);
        public override void AI()
        {
            FireColor = Main.rand.Next(3) switch
            {
                0 => Terrablazer.MainColor1,
                1 => Terrablazer.MainColor2,
                _ => Terrablazer.MainColor3,
            };

            Time++;
            ColorType += 0.02f;

            if (Time >= 1f) Projectile.scale = 2.2f * Utils.GetLerpValue(5f, 30f, Time, true);
            else return;

            if (Time == 1)
            {
                for (int i = 0; i < 12; i++)
                {
                    float rotMulti = Main.rand.NextFloat(0.7f, 1.1f);
                    int dustType = Main.rand.NextBool() ? 66 : 247;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType);
                    dust.scale = Main.rand.NextFloat(1.8f, 2.5f) - rotMulti;
                    dust.noGravity = true;
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.5f * rotMulti) * Main.rand.NextFloat(0.5f, 1.8f) * rotMulti;
                    dust.alpha = Main.rand.Next(90, 150);
                    dust.color = FireColor;
                }
            }

            if (Time > 9)
            {
                float dustArea = Main.rand.NextFloat(0.1f, 1.7f);
                int dustType = Main.rand.NextBool() ? 66 : 247;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9, 9) + Projectile.velocity * Main.rand.NextFloat(-1.8f, 1.8f), dustType);
                dust.scale = (1.8f - dustArea) * 0.65f;
                dust.noGravity = true;
                dust.velocity = new Vector2(4, 4).RotatedByRandom(100) * dustArea;
                dust.alpha = Main.rand.Next(90, 150);
                dust.color = FireColor;
            }
            if (Time > Lifetime - 25 || HitTile || Projectile.numHits >= MaxPenetrate) Projectile.alpha += 15;
            if (Projectile.alpha >= 255) Projectile.Kill();

            float hue = 0.5f * (ColorType % 1f) + 0.5f * Utils.GetLerpValue(30f, Lifetime, Time, true) * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
            Color smokeColor = Main.hslToRgb(hue, 1f, 0.7f);

            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale * 0.3f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.97f;
            Projectile.position -= Projectile.velocity;
            HitTile = true;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, Size * Projectile.scale * 0.5f, targetHitbox);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<TerraFlameDebuff>(), 1200);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<TerraFlameDebuff>(), 1200);
        public override bool? CanDamage() => Projectile.numHits < MaxPenetrate;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Projectile.Opacity * 0.1f;
            Color drawColor = (FireColor with { A = 0 }) * opacity;

            float length = MaxFlameTypes;
            float vOffset = Math.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);
            int flameType = 0;
            int flameType2 = 4;

            for (float j = 1f; j >= 0f; j -= length)
            {
                // pos && rot
                Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: flameType);
                Rectangle sourceRectangle2 = texture.Frame(1, Main.projFrames[Type], frameY: flameType2);
                Vector2 firePos = drawPosition - Projectile.velocity * vOffset * j;
                float mainRot = -j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) / length;
                float trailRot = MathHelper.PiOver4 - mainRot;
                Vector2 origin = sourceRectangle.Size() * 0.5f;

                if (flameType2 < 6) flameType2++;
                else flameType2 = 0;
                // backtrail
                Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
                Main.EntitySpriteDraw(texture, firePos - trailOffset, sourceRectangle2, drawColor * 0.25f, trailRot, origin, fireSize, SpriteEffects.None);
                // draw og proj
                Main.EntitySpriteDraw(texture, firePos, sourceRectangle, drawColor * 0.75f, -mainRot * 0.9f, origin, fireSize * 0.9f, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, firePos, sourceRectangle, drawColor, mainRot, origin, fireSize, SpriteEffects.None);
                if (flameType < 6) flameType++;
                else flameType = 0;
            }
            return false;
        }
    }
}