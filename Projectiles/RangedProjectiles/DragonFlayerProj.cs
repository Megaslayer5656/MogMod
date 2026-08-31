using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Ranged;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DragonFlayerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/SmokeParticle";
        public ref float Time => ref Projectile.ai[0];
        public ref float LightPower => ref Projectile.ai[1];
        public static int Lifetime => 70;
        public static int Fadetime => 60;
        public static int Size => 80;
        public int NumAnimationFrames = 7;
        public int MaxPenetrate = 10;
        public float MaxFlameTypes = 0.07f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = Size;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = DragonFlayer.ArmorPenetration;
        }
        public override void OnSpawn(IEntitySource source) => MaxFlameTypes = Main.rand.NextFloat(0.04f, 0.1f);
        public override void AI()
        {
            Time++;
            if (Time < Fadetime)
            {
                Vector2 cinderPos = Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Lifetime, 0.5f, 1f);
                float cinderSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust cinder = Dust.NewDustDirect(cinderPos, 4, 4, DustID.Flare, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 10, default, 0.75f);
                if (Main.rand.NextBool(3))
                {
                    cinder.scale *= 3f;
                    cinder.velocity *= 2f;
                }
                cinder.noGravity = true;
                cinder.scale *= cinderSize * 1.5f;
                cinder.velocity += Projectile.velocity * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f) * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
                if (Projectile.timeLeft > Lifetime - 10) return;
                float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
                float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 200, default, fireSize);
                Dust dust = Main.dust[fireDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }

            if (Time > Lifetime - 25 || Projectile.numHits >= MaxPenetrate) Projectile.alpha += 15;
            if (Projectile.alpha >= 255) Projectile.Kill();

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)Utils.Remap(Time, 0f, Fadetime, 8f, 32f);
            if (Time > Fadetime) size = (int)Utils.Remap(Time, Fadetime, Lifetime, 32f, 0f);
            hitbox.Inflate(size, size);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 1200);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 1200);
        public override bool PreDraw(ref Color lightColor)
        {
            // copied from calamity mods cataclysmic flame proj
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color color1 = new(255, 137, 87, 200);
            Color color2 = new(255, 149, 48, 70);
            Color color3 = new(255, 70, 31, 100);
            Color color4 = new(200, 60, 35, 100);
            float length = MaxFlameTypes;
            float vOffset = Math.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);
            int flameType = 0;
            int flameType2 = 4;

            if (timeRatio >= 1f)
                return false;

            for (float j = 1f; j >= 0f; j -= length)
            {
                // colon
                Color fireColor = ((timeRatio < 0.1f) ? Color.Lerp(Color.Transparent, color1, Utils.GetLerpValue(0f, 0.1f, timeRatio)) :
                ((timeRatio < 0.2f) ? Color.Lerp(color1, color2, Utils.GetLerpValue(0.1f, 0.2f, timeRatio)) :
                ((timeRatio < 0.35f) ? color2 :
                ((timeRatio < 0.7f) ? Color.Lerp(color2, color3, Utils.GetLerpValue(0.35f, 0.7f, timeRatio)) :
                ((timeRatio < 0.85f) ? Color.Lerp(color3, color4, Utils.GetLerpValue(0.7f, 0.85f, timeRatio)) :
                Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(0.85f, 1f, timeRatio)))))));
                fireColor *= (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);
                Color innerColor = Color.Lerp(fireColor, Color.Black, 0.3f);

                // pos && rot
                Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: flameType);
                Rectangle sourceRectangle2 = texture.Frame(1, Main.projFrames[Type], frameY: flameType2);
                Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
                float mainRot = -j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length;
                float trailRot = MathHelper.PiOver4 - mainRot;
                Vector2 origin = sourceRectangle.Size() / 2f;

                if (flameType2 < 6) flameType2++;
                else flameType2 = 0;
                // backtrail
                Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
                Main.EntitySpriteDraw(texture, firePos - trailOffset, sourceRectangle2, innerColor * 0.25f, trailRot, origin, fireSize, SpriteEffects.None);
                // draw og proj
                Main.EntitySpriteDraw(texture, firePos, sourceRectangle, innerColor * 0.75f, -mainRot * 0.9f, origin, fireSize * 0.9f, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, firePos, sourceRectangle, innerColor, mainRot, origin, fireSize, SpriteEffects.None);
                if (flameType < 6) flameType++;
                else flameType = 0;
            }
            return false;
        }
    }
}