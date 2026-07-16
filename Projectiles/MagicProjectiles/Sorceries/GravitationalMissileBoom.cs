using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Items.Weapons.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GravitationalMissileBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 16;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 520;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1.2f;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, GravitationalMissileProj.Colour.ToVector3() * 1.5f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame++;
            if (Projectile.frame == 9)
                SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            if (Projectile.frame >= 17)
                Projectile.Kill();
            Projectile.scale *= 1.015f;
            Projectile.Opacity = Utils.GetLerpValue(5f, 36f, Projectile.timeLeft, true);
        }
        public override bool? CanDamage() => Projectile.frame >= 9;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.HitDirectionOverride = target.position.X < Projectile.Center.X ? 1 : -1;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.HitDirectionOverride = target.position.X < Projectile.Center.X ? 1 : -1;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}