using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    /// <summary> Intended to be used for projectiles that leave behind a damaging trail. <br/>
    /// Set Projectile.ai[2] to 5f if you'd like to draw a visual, or 6f if you'd like a dust effect. <br/>
    /// Certain projectile specific changes are done in here instead of NewProjectileDirect, like with KaminariZipProj.</summary>
    public class TrailProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public int Size = 1;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }
        public override void AI()
        {
            MogPlayer mogPlayer = Owner.MogMod();
            Size = (int)(MathHelper.Max(Projectile.width / 4, Projectile.height / 4) * Projectile.scale);

            Projectile.position = Projectile.Center;
            Projectile.Center = Projectile.position;
            if (Projectile.ai[2] == 6f)
            {
                for (int i = 0; i < 2; i++)
                {
                    int fireDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.White);
                    Dust dust = Main.dust[fireDust];
                    dust.noGravity = true;
                    dust.position = Projectile.Center;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            MogPlayer mogPlayer = Owner.MogMod();
            if (mogPlayer.wearingKaminari && mogPlayer.kaminariActive) target.AddBuff(BuffID.Electrified, 180);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, Size, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] != 5f) return false;

            // draw main proj
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/SmallGrayCircle").Value;
            Vector2 position = Projectile.Center + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale * 0.25f;
            Color color = Color.Red;
            Main.EntitySpriteDraw(texture, position, null, color, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            return false;
        }
    }
}
