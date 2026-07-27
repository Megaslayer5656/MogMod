using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class APLapuaProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        private const int Lifetime = 1800;
        private const int NoDrawing = 2;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.light = .5f;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;

            MogModGlobalProjectile mogProj = Projectile.MogMod();
            mogProj.bloodDamage = AXMC.BloodDamage;
        }
        public override void OnSpawn(IEntitySource source) => Projectile.extraUpdates = Main.zenithWorld ? 0 : 4;
        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HeavyBleed>(), 360);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HeavyBleed>(), 360);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft >= Lifetime - NoDrawing * Projectile.MaxUpdates)
                return false;
            MogModUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor, null);
            return false;
        }
    }
}