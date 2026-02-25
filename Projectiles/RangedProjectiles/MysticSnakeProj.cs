using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    // code taken from hadel urn so i dont have to do spritesheet slop
    public class MysticSnakeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        int invistimer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            //Briefly invisible then quickly fades in after 4 frames so it doesn't appear out of your back
            invistimer++;
            if (invistimer > 4)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 50;
                }
                MogModUtils.HomeInOnNPC(Projectile, true, 500f, 12f, 15f);
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Type] / 2)
            {
                Projectile.frame = 0;
            }
            //The snake dies sooner without any food (very sad)
            if (Projectile.timeLeft <= 420)
            {
                Projectile.velocity *= 0.98f;
            }
            if (Projectile.timeLeft <= 390)
            {
                Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int textureheight = tex.Height / Main.projFrames[Type];
            int y = textureheight * Projectile.frame;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, tex.Width, textureheight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(tex.Width, tex.Height / 16f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit32, Projectile.Center);
            int inc;
            for (int i = 0; i < 25; i = inc + 1)
            {
                int hadalDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble, 0f, 0f, 0, default, 1f);
                Main.dust[hadalDust].position = (Main.dust[hadalDust].position + Projectile.position) / 2f;
                Main.dust[hadalDust].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[hadalDust].velocity.Normalize();
                Dust dust = Main.dust[hadalDust];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[hadalDust].alpha = Projectile.alpha;
                inc = i;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 300);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override bool CanHitPvp(Player target) => Projectile.ai[0] == 0f;
    }
}