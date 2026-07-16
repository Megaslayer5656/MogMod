using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GravityWellProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/StarProj";
        private Player Owner => Main.player[Projectile.owner];
        public static Color Colour => new(239, 143, 255);
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1800;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.zenithWorld)
            {
                Time = Projectile.timeLeft;
                Projectile.extraUpdates = (int)(Time / 8f);
            }
        }
        public override void AI()
        {
            if (Main.zenithWorld)
                Time--;
            else
                Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float dim = .005f;
            Lighting.AddLight(Projectile.Center, Colour.R * dim, Colour.G * dim, Colour.B * dim);
            if (Main.zenithWorld)
            {
                if (Projectile.extraUpdates > 1)
                    Projectile.extraUpdates -= (int)(Time / 1575f);
            }
            else
                Projectile.extraUpdates = (int)(Time / 8f);

            Dust gDust = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleCrystalShard, Projectile.velocity, 100, default, 0.75f);
            gDust.noGravity = true;
            gDust.velocity *= 0.1f;
            
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextBool(3) ? 27 : 62);
                dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                dust.velocity = -Projectile.velocity * 0.7f;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X < Owner.MountedCenter.X ? 1 : -1;
            if (Main.zenithWorld)
                modifiers.Knockback += 5;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X < Owner.MountedCenter.X ? 1 : -1;
            if (Main.zenithWorld)
                modifiers.Knockback += 5;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 27 : 62, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.MediumPurple, 1f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
            if (Projectile.MogMod().meteoriteSpell)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GravityOrbProj>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack * 2f, Projectile.owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Vector2 trailOffset = Projectile.oldVelocity * 10f;
            for (float n = 0; n < 2; n++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n), null, (Colour * 0.8f) with { A = 255 }, Projectile.oldRot[(int)(n)], bloomTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 2f), null, (Colour * 0.1f) with { A = 255 }, Projectile.oldRot[(int)(n * 2)], bloomTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}