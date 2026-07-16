using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Classes;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class SpectralLightProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public static readonly SoundStyle spawn = new("Terraria/Sounds/Item_20")
        {
            Volume = 0.9f,
            PitchVariance = 0.2f,
            MaxInstances = 15
        };
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public Color Colour = new(204, 230, 227);
        public float launch = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft *= Projectile.MogMod().deathSpell ? 2 : 1;
            if (Main.zenithWorld)
            {
                Projectile.timeLeft *= 5;
                Projectile.extraUpdates += 5;
            }
            float angleVariance = MathHelper.TwoPi / 6;
            Vector2 projVec = new Vector2(5f, 0f).RotatedBy(MathHelper.ToRadians(60 * (Projectile.ai[1] - 1)));
            projVec = projVec.RotatedBy(angleVariance);
            Vector2 playerPosition = Owner.Center + projVec * (Owner.gfxOffY - 10f);
            launch = Projectile.ai[1] * 12f;
            SoundEngine.PlaySound(spawn, Projectile.Center);
            if (Time <= 3)
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(playerPosition, DustID.RainbowTorch, Main.rand.NextVector2Circular(Projectile.width * 0.1f, Projectile.height * 0.1f), 100, Colour, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                    dust.scale *= 1.15f;
                }
        }
        public override void AI()
        {
            Time++;
            if (Time >= (launch * (Main.zenithWorld ? 5 : 1)))
            {
                if (Time == (launch * (Main.zenithWorld ? 5 : 1)))
                    Projectile.extraUpdates += Projectile.MogMod().deathSpell ? 1 : 0;
                if (Projectile.velocity.Length() < 6)
                    Projectile.velocity += (Owner.MogMod().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.35f;
                else
                    Projectile.velocity *= 0.9f;
            }
            else
            {
                float angleVariance = MathHelper.TwoPi / 6;
                Vector2 projVec = new Vector2(5f, 0f).RotatedBy(MathHelper.ToRadians(60 * (Projectile.ai[1] - 1)));
                projVec = projVec.RotatedBy(angleVariance);
                Vector2 playerPosition = Owner.Center + projVec * (Owner.gfxOffY - 10f);
                Projectile.Center = Vector2.Lerp(Projectile.Center, playerPosition, 1f);
            }
            int ghostflameDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Colour }), 1f);
            Main.dust[ghostflameDust].velocity *= 0.5f;
            Main.dust[ghostflameDust].scale *= 1.05f;
            Main.dust[ghostflameDust].fadeIn = 0.7f;
            Main.dust[ghostflameDust].noGravity = true;
            if (Time % 3 == 0)
                Projectile.rotation += 90;
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * 0.5f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GhostflameDebuff>(), 360);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GhostflameDebuff>(), 360);
        public override bool? CanDamage() => Time >= launch ? null : false;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Colour }), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, Colour, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/GlowRingParticle").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.2f, SpriteEffects.None);
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.35f, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}