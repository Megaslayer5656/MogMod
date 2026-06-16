using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.ClasslessProjectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class ScintillantStabberProj : BaseShortswordProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Melee/ScintillantStabber";
        public override float FadeInDuration => 1f;
        public override float FadeOutDuration => 0f;
        public override float TotalDuration => 6f;
        public bool hitNPC = false;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(12);
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 20;
        }
        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 36 / 2;
            const int HalfSpriteHeight = 36 / 2;

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
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Main.rand.NextBool(3) ? 124 : DustID.Sandnado);
                dust.scale = Main.rand.NextFloat(0.15f, 0.6f);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity * 0.5f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hitNPC)
            {
                ScintillantStabber.hitCount++;
                hitNPC = true;
            }
            if (ScintillantStabber.hitCount > 9)
            {
                var source = Projectile.GetSource_FromThis();
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<SandstormProj>(), Projectile.damage, 0f, Projectile.owner);
                ScintillantStabber.hitCount = 0;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!hitNPC)
            {
                ScintillantStabber.hitCount++;
                hitNPC = true;
            }
            if (ScintillantStabber.hitCount > 9)
            {
                var source = Projectile.GetSource_FromThis();
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<SandstormProj>(), Projectile.damage, 0f, Projectile.owner);
                ScintillantStabber.hitCount = 0;
            }
        }
    }
}