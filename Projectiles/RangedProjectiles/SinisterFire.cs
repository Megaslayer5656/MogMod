using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Ranged;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class SinisterFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/DustGlowParticle";
        public int Size = 0;
        public int Time = 0;
        public ref float Scale => ref Projectile.ai[0];
        public ref float Light => ref Projectile.ai[1];
        public Color Colour1 = SinisterSpreader.MainColor1;
        public Color Colour2 = SinisterSpreader.MainColor2;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
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
            if (Main.rand.NextBool(4) && Time > 55)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Size * 1.1f, Size * 1.1f), DustID.FireworksRGB, Vector2.Zero, 100, Colour1);
                dust.scale = Main.rand.NextFloat(0.2f, 0.4f);
                dust.noGravity = true;
            }
            else
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Size, Size), DustID.CopperCoin, Vector2.Zero, 0, Colour2);
                dust.scale = Main.rand.NextFloat(0.4f, 1.2f);
                dust.noGravity = true;
            }
            Scale += 0.006f;
            float scaleMax = Main.zenithWorld ? Projectile.scale * 4f : Projectile.scale * 0.8f;
            Scale = MathHelper.Clamp(Scale, 0f, scaleMax);
            Lighting.AddLight(Projectile.Center, new Vector3((float)(Colour1.R * 0.01f), (float)(Colour1.G * 0.01f), (float)(Colour1.B * 0.01f)) * Scale);
            Projectile.velocity *= Main.rand.NextFloat(0.95f, 1f);
            Projectile.Opacity = Utils.GetLerpValue(30f, 50f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 130f, Projectile.timeLeft, true);
            if (Main.dedServ)
                return;
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            Light = MathHelper.Lerp(Light, lightPowerBelow, 0.15f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.97f;
            Projectile.position -= Projectile.velocity;
            return false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float GFBMult = Main.zenithWorld ? 4f : 1f;
            Size = (int)Utils.Remap(Time, 0f, 120 * GFBMult, 10f, 125f * GFBMult);
            hitbox.Inflate(Size, Size);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 1200);
            Projectile.velocity = Projectile.oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 1200);
            Projectile.velocity = Projectile.oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Projectile.Opacity * 0.3f;
            Color drawColor1 = (Colour1 with { A = 0 }) * opacity;
            Color drawColor2 = (Colour2 with { A = 0 }) * opacity;
            Main.EntitySpriteDraw(texture, drawPosition + Main.rand.NextVector2Circular(19, 19), null, drawColor1 * 0.55f, Projectile.rotation, texture.Size() * 0.5f, Scale * 1.2f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor2, -Projectile.rotation * 0.9f, texture.Size() * 0.5f, Scale, SpriteEffects.None);
            return false;
        }
    }
}