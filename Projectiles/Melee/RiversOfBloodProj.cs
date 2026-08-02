using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class RiversOfBloodProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public const int size = 2;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 76;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            MogModGlobalProjectile mogProj = Projectile.MogMod();
            mogProj.bloodDamage = RiversOfBlood.ProjectileBloodDamage;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3());
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemRuby, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 100);
            }
            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 0, 60, 255, 0);
                Projectile.velocity *= 0.94f;
            }
            else if (Projectile.scale < 2f)
            {
                Projectile.velocity *= 0.997f;
                Projectile.scale += 0.005f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 1.1f;
            if (Projectile.numHits >= 5 && Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.velocity *= 1.1f;
            if (Projectile.numHits >= 5 && Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(new((int)(targetHitbox.Center.X - targetHitbox.Width * size / 2), (int)(targetHitbox.Center.Y - targetHitbox.Height * size / 2), targetHitbox.Width * size, targetHitbox.Height * size));
        public override bool? CanDamage() => (Projectile.alpha == 0 ? null : false);
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2, scale: 2f);
            return false;
        }
    }
}
