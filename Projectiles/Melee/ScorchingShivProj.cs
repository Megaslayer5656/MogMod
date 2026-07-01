using Microsoft.Xna.Framework;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class ScorchingShivProj : BaseShortswordProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override float FadeInDuration => 8f;
        public override float FadeOutDuration => 0f;
        public override float TotalDuration => 16f;
        public bool CanExplode = true;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(15);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 56 / 2;
            const int HalfSpriteHeight = 56 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Main.rand.NextBool(3) ? DustID.Lava : 174);
                dust.scale = Main.rand.NextFloat(0.15f, 0.6f);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity * 0.5f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            Explode(target);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            Explode(target);
        }
        public void Explode(Entity target)
        {
            if (CanExplode)
            {
                var source = Projectile.GetSource_FromThis();
                Projectile explosion = Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireBoom>(), Projectile.damage * 2, 0f, Main.myPlayer, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                explosion.DamageType = DamageClass.Melee;
                CanExplode = false;
            }
        }
    }
}