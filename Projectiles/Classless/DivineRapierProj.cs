using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.BaseProjectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class DivineRapierProj : BaseSpearProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";

        // spear code taken from calamity mod
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 42;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }
        public override float InitialSpeed => 2f;
        public override float ReelbackSpeed => 1f;
        public override float ForwardSpeed => 0.7f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 16f, ModContent.ProjectileType<DivineRapierSecProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        };
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DivineMightDebuff>(), 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DivineMightDebuff>(), 600);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();
            float critDamage = Main.player[Projectile.owner].GetTotalCritChance(Projectile.DamageType) * 0.02f;
            modifiers.CritDamage += critDamage;
        }
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
            {
                int idx = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RainbowTorch, Projectile.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[idx].noGravity = true;
            }
        }
    }
}