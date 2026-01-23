using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class ButterflyProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.ArmorPenetration = 12;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
            if (Projectile.ai[0] >= 300f)
                Projectile.Kill();
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
            MogModUtils.HomeInOnNPC(Projectile, true, 500f, 5f, 18f);

            if (Main.rand.NextBool(25))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Terra, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, .5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);;

            Vector2 origin = sourceRectangle.Size() / 2f;

            float offsetX = 20f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i <= 3; i++)
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Terra, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, .5f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Terra, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, .5f);
        }
    }
}
