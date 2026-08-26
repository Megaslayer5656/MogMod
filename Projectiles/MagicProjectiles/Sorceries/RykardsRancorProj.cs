using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Common.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class RykardsRancorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public static Color Colour => new(237, 77, 9);
        public ref float Time => ref Projectile.ai[0];
        public int NumAnimationFrames = 4;
        public int AnimationFrameTime = 3;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 26;

            Projectile.penetrate = 90;
            Projectile.timeLeft = 1800;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.scale = 1.5f;
        }
        public override void AI()
        {
            Time++;
            if (Time >= 40)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 7f, 30f, false);
                if (Time % 40 == 0)
                {
                    var source = Projectile.GetSource_FromThis();
                    int type = Main.zenithWorld ? ModContent.ProjectileType<RoilingMagmaProj>() : ModContent.ProjectileType<RykardsRancorBoom>();
                    Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, type, Projectile.damage, 0f, Projectile.owner);
                    Projectile.penetrate -= 3;
                    if (Projectile.penetrate <= 0)
                        Projectile.Kill();
                }
            }

            int flameDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Colour }), 1f);
            Main.dust[flameDust].velocity *= 0.5f;
            Main.dust[flameDust].scale *= 1.05f;
            Main.dust[flameDust].fadeIn = 0.7f;
            Main.dust[flameDust].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Daybreak, 360);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Daybreak, 360);
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Colour }), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw trail
            TrailDrawer trailDrawer = default;
            trailDrawer.Draw(Projectile, "FlameLash", Color.OrangeRed, Colour, 0.8f);

            // projectile animation
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(tex,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, direction, 0);

            // glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}