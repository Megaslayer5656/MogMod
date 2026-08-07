using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class NativitasProj : BaseFlailProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override bool SpawnDust => true;
        public override int ExudeDustType => DustID.FrostHydra;
        public override int WhipDustType => DustID.SnowflakeIce;
        public override int HandleHeight => 42;
        public override int BodyType1StartY => 44;
        public override int BodyType1SectionHeight => 18;
        public override int BodyType2StartY => 64;
        public override int BodyType2SectionHeight => 18;
        public override int TailStartY => 84;
        public override int TailHeight => 50;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FreezingDebuff>(), 300);
            if (Projectile.localAI[1] <= 0f && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FrostExplosion>(), (int)(Projectile.damage * 0.5f), hit.Knockback, Projectile.owner);
            Projectile.localAI[1] = 4f;
        }
    }
}