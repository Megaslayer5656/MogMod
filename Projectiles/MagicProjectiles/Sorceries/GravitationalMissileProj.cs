using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GravitationalMissileProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public ref float Time => ref Projectile.ai[0];
        public static Color Colour => new(239, 143, 255);
        public int NumAnimationFrames = 4;
        public int AnimationFrameTime = 10;
        public int MaxBooms = 10;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = MaxBooms; // how many booms before dying
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Time++;
            //Projectile.rotation += Main.rand.NextFloat(0.2f, 0.9f);
            if (Time >= 40)
                if (Time % 40 == 0)
                {
                    var source = Projectile.GetSource_FromThis();
                    Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GravitationalMissileBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (Projectile.MogMod().meteoriteSpell)
                    {
                        int numSplits = 6;
                        float angleVariance = MathHelper.TwoPi / numSplits;
                        Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.ToRadians(45));
                        for (int i = 0; i < numSplits; ++i)
                        {
                            projVec = projVec.RotatedBy(angleVariance);
                            int type = ModContent.ProjectileType<GravityWellProj>();
                            float velocity = Main.zenithWorld ? 0.5f : 1f;
                            if (Main.zenithWorld && Projectile.ai[2] != 1f)
                                type = Projectile.type;
                            Projectile.NewProjectile(source, Projectile.Center, projVec * velocity, type, (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, ai2: 1f);
                        }
                        if (Main.zenithWorld)
                            Projectile.ai[2] = 1f;
                    }
                    Projectile.penetrate -= 1;
                    if (Projectile.penetrate <= 0)
                        Projectile.Kill();
                }
            if (Main.rand.NextBool(4) && Time > 20)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2.1f, Projectile.height / 2.1f), DustID.PurpleCrystalShard, Vector2.Zero, 100);
                dust.scale = Main.rand.NextFloat(0.2f, 0.4f);
                dust.noGravity = true;
            }
            else
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2f, Projectile.height / 2f), DustID.GemDiamond, Vector2.Zero, 0, Colour);
                dust.scale = Main.rand.NextFloat(0.4f, 1.2f);
                dust.noGravity = true;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
            if (Main.dedServ)
                return;
            float dim = .01f;
            Lighting.AddLight(Projectile.Center, Colour.R * dim, Colour.G * dim, Colour.B * dim);
        }
        public override bool? CanDamage() => false;
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 10)
            {
                float timeAlpha = (float)Projectile.timeLeft / 10f;
                Projectile.alpha = (int)(255f - 255f * timeAlpha);
            }
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw original proj
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int framing = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, tex.Width, framing)), Projectile.GetAlpha(lightColor),
                Projectile.rotation, new Vector2((float)tex.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);

            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}