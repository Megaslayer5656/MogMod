using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class WitchBladeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Ranged/WitchBlade";
        internal float gravspin = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 420;
        }

        public override void AI()
        {
            DrawOriginOffsetY = 11;
            DrawOffsetX = -22;
            if (Projectile.spriteDirection == 1)
            {
                gravspin = Projectile.velocity.Y * 0.03f;
            }
            if (Projectile.spriteDirection == -1)
            {
                gravspin = Projectile.velocity.Y * -0.03f;
            }
            Projectile.ai[0]++;
            

            // slopes
            if (Projectile.ai[0] > 2f)
            {
                Projectile.tileCollide = true;
            }

            // forward facing rotation
            if (Projectile.ai[0] <= 80 || Projectile.velocity.Y <= 0)
            {
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

                // rotating 45 degrees if shooting right
                if (Projectile.spriteDirection == 1)
                {
                    Projectile.rotation += MathHelper.ToRadians(45f);
                }

                // when facing left
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation -= MathHelper.ToRadians(45f);
                }
            }

            // home after hitting an enemy
            if (Projectile.penetrate == 1 && Projectile.ai[2] > 30f)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 600f, 7f, 20f);
            }

            // asuka r#
            else
            {
                if (Projectile.ai[0] > 80)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
                    if (Projectile.velocity.Y > 0)
                    {
                        Projectile.rotation += gravspin;
                    }
                    if (Projectile.velocity.Y > 10f)
                    {
                        Projectile.velocity.Y = 10f;
                    }
                }
            }

            if (Projectile.ai[2] >= 1f)
            {
                Projectile.tileCollide = false;
            }

            if (Projectile.ai[2] >= 1f)
            {
                Projectile.ai[2]++;
            }

            if (Projectile.ai[2] <= 30f && Projectile.ai[2] >= 1f)
            {
                Projectile.friendly = false;
            }
            else
            {
                Projectile.friendly = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 1f;
            if (Projectile.ai[1] == 1f && Projectile.penetrate == 1)
            {
                Projectile.timeLeft = 420;
            }
            target.AddBuff(BuffID.Venom, 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.ai[1] == 1f && Projectile.penetrate == 1)
                Projectile.timeLeft = 180;
            target.AddBuff(BuffID.Venom, 300);
        }
    }
}
