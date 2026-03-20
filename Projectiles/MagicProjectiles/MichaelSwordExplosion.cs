using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.DataStructures;
using MogMod.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID; 
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class MichaelSwordExplosion : ModProjectile, ILocalizedModType
    {
        // code taken from calamity mod terratomere and example mod advanced animated projectile
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16; // i might be retarded
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 520;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 5;
            Projectile.scale = .6f;
        }

        public override void AI()
        {
            // Play an explosion sound on the first frame of this projectile's existence.
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                Projectile.localAI[0] = 1f;
            }

            // Emit a strong white light.
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.5f);

            // Determine frames. Once the maximum frame is reached the projectile dies.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame++;
            if (Projectile.frame >= 17)
                Projectile.Kill();

            // Exponentially accelerate.
            Projectile.scale *= MichaelSword.ExplosionExpandFactor;
            Projectile.Opacity = Utils.GetLerpValue(5f, 36f, Projectile.timeLeft, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}