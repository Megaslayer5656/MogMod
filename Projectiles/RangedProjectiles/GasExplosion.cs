using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class GasExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/DustGlowParticle";
        public int Size = 0;
        public int Time = 0;
        public ref float Scale => ref Projectile.ai[0];
        public ref float Light => ref Projectile.ai[1];
        public Color Colour = new(57, 156, 8);
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.netImportant = true;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.zenithWorld)
            {
                Projectile.timeLeft *= 5;
                Projectile.extraUpdates += 5;
                Projectile.hostile = true;
            }
        }
        public override void AI()
        {
            Time++;
            Projectile.rotation += Main.rand.NextFloat(0.2f, 0.9f);
            if (Projectile.Opacity >= 0.4f)
            {
                if (Main.rand.NextBool(4) && Time > 40)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Size * 1.1f, Size * 1.1f), DustID.FireworkFountain_Green, Vector2.Zero);
                    dust.scale = Main.rand.NextFloat(0.1f, 0.25f);
                    dust.noGravity = true;
                    dust.alpha = 100;
                }
                else
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Size, Size), DustID.GemEmerald, Vector2.Zero, 0, Colour);
                    dust.scale = Main.rand.NextFloat(0.2f, 0.75f);
                    dust.noGravity = true;
                    dust.alpha = 100;
                }
            }
            if (Projectile.Opacity <= 0.05f)
                Projectile.Kill();
            Scale += 0.8f;
            float scaleMax = Main.zenithWorld ? Projectile.scale * 3f : Projectile.scale * 0.2f;
            Scale = MathHelper.Clamp(Scale, 0f, scaleMax);
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0.25f) * Scale);
            Projectile.velocity *= 0.98f;
            Projectile.Opacity = Utils.GetLerpValue(30f, 50f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 100f, Projectile.timeLeft, true);
            if (Main.dedServ)
                return;
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            Light = MathHelper.Lerp(Light, lightPowerBelow, 0.15f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
            return false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float GFBMult = Main.zenithWorld ? 4f : 1f;
            Size = (int)Utils.Remap(Time, 0f, 100 * GFBMult, 10f, 105f * GFBMult);
            hitbox.Inflate(Size, Size);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(4))
               target.AddBuff(BuffID.Confused, 120);
            target.AddBuff(BuffID.Poisoned, 180);
            Projectile.velocity = Projectile.oldVelocity * 0.5f;
            Projectile.position -= Projectile.velocity;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.Confused, 120);
            target.AddBuff(BuffID.Poisoned, 180);
            Projectile.velocity = Projectile.oldVelocity * 0.5f;
            Projectile.position -= Projectile.velocity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Projectile.Opacity * 0.3f;
            Color drawColor = (Colour with { A = 0 }) * opacity;
            Main.EntitySpriteDraw(texture, drawPosition + Main.rand.NextVector2Circular(19, 19), null, drawColor * 0.55f, Projectile.rotation, texture.Size() * 0.5f, Scale * 1.2f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, -Projectile.rotation * 0.9f, texture.Size() * 0.5f, Scale, SpriteEffects.None);
            return false;
        }
    }
}