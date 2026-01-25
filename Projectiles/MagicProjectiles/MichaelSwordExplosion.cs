using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.DataStructures;
using MogMod.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID; 
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class MichaelSwordExplosion : ModProjectile, IAdditiveDrawer, ILocalizedModType
    {
        // code taken from terratomere calamity mod
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
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
            Projectile.scale = 0.2f;
            Projectile.hide = true;
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
        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(3, 6, Projectile.frame / 6, Projectile.frame % 6);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Size() * 0.5f;
            if (Projectile.timeLeft < 149)
                Main.spriteBatch.Draw(texture, drawPosition, frame, Color.White, 0f, origin, 1.6f, SpriteEffects.None, 0);
        }
    }
}