using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class ElysianSeraphThrownProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Items/Weapons/Melee/ElysianSeraph";
        public int Timeleft = 300;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Timeleft;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void AI()
        {
            float rotateratio = 0.05f;
            float rotate = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * rotateratio;
            Projectile.rotation += rotate * Projectile.direction;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.localAI[1]++;
            Projectile.ai[1]++;
            if (Projectile.timeLeft < (Timeleft - 30))
                Projectile.velocity *= 0.932f;
            if (Projectile.timeLeft < (Timeleft - 50))
                Projectile.ai[0] = 1f;
            if (Projectile.ai[0] >= 1f && Projectile.ai[1] % 10 == 0)
            {
                float rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Projectile.rotation = rotation;
                if (Projectile.owner == Main.myPlayer)
                {
                    var source = Projectile.GetSource_FromThis();
                    int type = ModContent.ProjectileType<ElysianSeraphBeamProj>();
                    Projectile.NewProjectile(source, Projectile.Center, new Vector2(10f, 10f).RotatedBy(rotation - MathHelper.PiOver2), type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: 1f);
                }
            }
            if (Projectile.localAI[1] > 4f)
            {
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Main.rand.NextBool(3) ? DustID.HallowSpray : 133);
                    dust.scale = Main.rand.NextFloat(0.15f, 0.6f);
                    dust.noGravity = true;
                    dust.velocity = -Projectile.velocity * 0.5f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustVelocity.Normalize();
                dustVelocity *= 2;

                int dustPos = 10;
                int seraphic = Dust.NewDust(Projectile.Center, dustPos, dustPos, Main.rand.NextBool(3) ? DustID.HallowSpray : 133, dustVelocity.X * 2, dustVelocity.Y * 2, 0, default, 1.2f);
                Main.dust[seraphic].noGravity = true;
                Main.dust[seraphic].fadeIn = 5f;
                Main.dust[seraphic].velocity *= 3f;
            }
        }
    }
}