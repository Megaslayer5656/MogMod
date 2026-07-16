using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class BladesOfStoneProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public static readonly SoundStyle crystal = new("Terraria/Sounds/Item_101")
        {
            Volume = 0.7f,
            PitchVariance = 0.2f,
            MaxInstances = 15
        };
        public int FrameNum = 6;
        public int spriteNumb = Main.rand.Next(0, 6);
        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = FrameNum;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 250;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
            Projectile.ArmorPenetration = 15;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = -MathHelper.PiOver2 * Main.rand.NextFloat(0.8f, 1.2f);
            SoundEngine.PlaySound(crystal, Projectile.Center);
            if (Main.zenithWorld)
            {
                Projectile.hostile = true;
                Projectile.timeLeft *= 50;
                Projectile.localNPCHitCooldown = 10;
            }
        }
        public override void AI()
        {
            if (Projectile.scale < 1f && Projectile.timeLeft > 40)
                Projectile.scale += 0.1f;
            if (Projectile.timeLeft < 40)
            {
                int alpha = Main.zenithWorld ? 10 : 20;
                Projectile.alpha += alpha;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                float numb = Main.zenithWorld ? 4f : 1f;
                int damage = Main.zenithWorld ? (int)(Projectile.damage * 1.5f) : (int)(Projectile.damage * 0.5f);
                if (Projectile.ai[0] != numb && Projectile.alpha == 100 && Projectile.MogMod().meteoriteSpell)
                    for (int x = 0; x < 2; x++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), new(Projectile.Center.X + (float)Main.rand.Next(-80, 80), Projectile.Center.Y), Vector2.Zero, Projectile.type, damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1f);
            }
            int rockDust = Dust.NewDust(Projectile.position, Projectile.width, (int)(Projectile.height * 1.5f), DustID.PurpleCrystalShard, 0f, 0f, 0, default, 1f);
            Main.dust[rockDust].velocity *= 0.5f;
            Main.dust[rockDust].scale *= 1.05f;
            Main.dust[rockDust].fadeIn = 0.7f;
            Main.dust[rockDust].noGravity = true;

            int width = Projectile.width * 3;
            Vector2 position = new(Projectile.position.X - 20f, Projectile.position.Y + 140f);
            int groundDust = Dust.NewDust(position, width, Projectile.width, DustID.PurpleCrystalShard, 0f, 0f, 100, default, 1f);
            Main.dust[groundDust].fadeIn += 1.2f;
            Main.dust[groundDust].velocity.Y *= 1.02f;
            Main.dust[groundDust].noGravity = true;
            int smoke = Dust.NewDust(position, width, Projectile.width, DustID.Smoke, 0f, 0f, 100, Color.Magenta, 1f);
            Main.dust[smoke].fadeIn += 1.2f;
            Main.dust[smoke].velocity.Y *= 1.02f;
            Main.dust[smoke].noGravity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.damage = Main.zenithWorld ? Projectile.damage : (int)(Projectile.damage * .97f);
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / FrameNum;
            int startY = frameHeight * spriteNumb;
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