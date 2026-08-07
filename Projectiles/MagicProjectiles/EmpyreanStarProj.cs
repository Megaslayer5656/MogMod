using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class EmpyreanStarProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public int Timeleft = 40;
        public Color StarColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Timeleft;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 3f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            switch (Projectile.ai[2])
            {
                case 0f:
                    StarColor = new Color(255, 249, 59);
                    break;
                case 1f:
                    StarColor = new Color(247, 119, 224);
                    break;
                case 2f:
                    StarColor = new Color(40, 105, 240);
                    break;
            }
            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ShimmerSpark, -Projectile.velocity * Main.rand.NextFloat(0.4f, 0.7f), 100, StarColor * 0.8f, 1f * Projectile.scale);
        }
        public override void AI()
        {
            if (Projectile.ai[0] < 10f && Projectile.timeLeft % 3 == 0)
            {
                Projectile.ai[0]++;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * Main.rand.NextFloat(1.3f, 1.7f), 200, StarColor * 0.8f, 0.5f * Projectile.scale);
            }
            else if (Projectile.ai[0] > 1f && Projectile.timeLeft < Timeleft / 2 && Projectile.timeLeft % 3 == 0)
            {
                Projectile.ai[0]--;
                if (Projectile.ai[0] <= 0f) Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, StarColor.ToVector3() * 0.5f);
            Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.ShimmerSpark, -Projectile.velocity * 0.8f, 150, StarColor * 1.8f, 0.06f * Projectile.scale);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
            byte alpha;
            if (Projectile.timeLeft > Timeleft / 2) alpha = (byte)MathHelper.Lerp(0f, 255f, Projectile.ai[0] / 10f);
            else alpha = (byte)MathHelper.Lerp(255f, 0f, Projectile.ai[0] / 10f);
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, StarColor with { A = alpha }, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);

                if (Projectile.ai[0] >= 5f)
                {
                    // backtrail
                    Vector2 trailOffset = Projectile.oldVelocity * 5f;
                    for (float n = 0; n < 4; n++)
                    {
                        Color newColor = StarColor * 0.4f;
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.05f), null, newColor with { A = alpha }, Projectile.oldRot[(int)(n * 0.05f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None);
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.1f), null, newColor with { A = alpha }, Projectile.oldRot[(int)(n * 0.1f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);
                    }
                }
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}