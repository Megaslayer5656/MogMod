using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles
{
    public class BlazingExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.EnemyProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/FlameProj";
        public static readonly SoundStyle boom = new("Terraria/Sounds/Item_62")
        {
            Volume = 0.7f,
            PitchVariance = 0.2f,
            MaxInstances = 15
        };
        public static Color Colour => new(237, 77, 9);
        public ref float Time => ref Projectile.ai[0];
        public ref float LightPower => ref Projectile.ai[1];
        public static int Lifetime => 50;
        public static int Fadetime => 40;
        public int NumAnimationFrames = 7;
        public int AnimationFrameTime = 4;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 98;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale *= 3f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[2] == 1f)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;
            }
        }
        public override void AI()
        {
            Time++;
            if (Time < Fadetime)
            {
                Vector2 cinderPos = Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Lifetime, 0.5f, 1f);
                float cinderSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust cinder = Dust.NewDustDirect(cinderPos, 4, 4, DustID.SolarFlare, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 10, default, 0.75f);
                if (Main.rand.NextBool(3))
                {
                    cinder.scale *= 1.2f;
                    cinder.velocity *= 2f;
                }
                cinder.noGravity = true;
                cinder.scale *= cinderSize * 1.5f;
                cinder.velocity += Projectile.velocity * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f) * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
                if (Projectile.timeLeft > Lifetime - 10)
                    return;
                float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
                float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 200, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Colour }), fireSize);
                Dust dust = Main.dust[fireDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame == 4)
                SoundEngine.PlaySound(boom, Projectile.Center);
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.Kill();
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.4f);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 240);
        public override bool? CanDamage() => Projectile.frame >= 4;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fire = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Color color1 = Colour * 1.6f;
            Color color2 = Colour * 1.3f;
            Color color3 = Colour * 1.8f;
            Color color4 = Colour * 0.8f;
            float length = ((Time > Fadetime - 10f) ? 0.1f : 0.15f);
            float vOffset = Math.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

            if (timeRatio >= 1f)
                return false;

            for (float j = 1f; j >= 0f; j -= length)
            {
                // color
                Color fireColor = ((timeRatio < 0.1f) ? Color.Lerp(Color.Transparent, color1, Utils.GetLerpValue(0f, 0.1f, timeRatio)) :
                ((timeRatio < 0.2f) ? Color.Lerp(color1, color2, Utils.GetLerpValue(0.1f, 0.2f, timeRatio)) :
                ((timeRatio < 0.35f) ? color2 :
                ((timeRatio < 0.7f) ? Color.Lerp(color2, color3, Utils.GetLerpValue(0.35f, 0.7f, timeRatio)) :
                ((timeRatio < 0.85f) ? Color.Lerp(color3, color4, Utils.GetLerpValue(0.7f, 0.85f, timeRatio)) :
                Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(0.85f, 1f, timeRatio)))))));
                fireColor *= (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);
                Color innerColor = Color.Lerp(fireColor, Color.Black, 0.3f);

                // pos && rot
                Rectangle sourceRectangle = fire.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
                Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
                Vector2 origin = sourceRectangle.Size() / 2f;

                // draw og proj
                Main.EntitySpriteDraw(fire, firePos, sourceRectangle, innerColor, 0f, origin, fireSize * Projectile.scale, SpriteEffects.None);
            }
            return false;
        }
    }
}