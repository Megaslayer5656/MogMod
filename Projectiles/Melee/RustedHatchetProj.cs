using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class RustedHatchetProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Items/Weapons/Melee/RustedHatchet";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = RustedHatchet.NumBounces + 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }
        public override void AI()
        {
            float rotateratio = 0.019f;
            float rotation = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * rotateratio;
            Projectile.rotation += rotation * Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 16f) Projectile.velocity.Y = 16f;
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 240;
            Projectile.damage = (int)(Projectile.damage * 0.5f);
            if (Projectile.velocity.Y > 0) Projectile.velocity.Y -= 6;
            else Projectile.velocity.Y = -6;
            Projectile.velocity.X *= -0.5f;
            Projectile.netUpdate = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}