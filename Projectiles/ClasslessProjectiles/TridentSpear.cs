using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Common.Classes;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Items.Weapons.Classless;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class TridentSpear : BaseSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        // spear code taken from calamity mod
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.DamageType = MeleeRangedMagicDamageClass.Instance;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }
        public override float InitialSpeed => 0.8f;
        public override float ReelbackSpeed => 0.8f;
        public override float ForwardSpeed => 0.7f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Vector2 tridentVelocity = Projectile.velocity * 2;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, tridentVelocity.X, tridentVelocity.Y, ModContent.ProjectileType<TridentProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f, 0f);
        };
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.CritDamage *= 1.2f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
            {
                int idx = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.direction * 2, 0f, 150);
                Main.dust[idx].color = Main.rand.NextBool() ? Color.AliceBlue : Color.LightBlue;
                Main.dust[idx].noGravity = true;
            }
        }
    }
}