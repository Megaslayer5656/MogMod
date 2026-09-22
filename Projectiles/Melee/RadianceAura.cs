using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class RadianceAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float HitCooldown => ref Projectile.ai[0];
        private const float radius = 98f;
        private const int debuffTime = 90;
        bool hitEnemy = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 218;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            HitCooldown++;
            if (HitCooldown % Projectile.localNPCHitCooldown == 0) hitEnemy = false;

            for (int s = 0; s < 6; s++)
            {
                float dustRot = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * s);
                Vector2 dustPos = Projectile.Center + Vector2.UnitX.RotatedBy(dustRot) * 85f * (Owner.MogMod().radiancePower + 1f);
                Vector2 dustVel = Vector2.Normalize(dustPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f;
                Dust d = Dust.NewDustPerfect(dustPos, (Owner.MogMod().radiancePower > 0.5f ? DustID.Flare : DustID.Torch), dustVel, 100);
            }
            
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.width = Projectile.height = (int)(218 * (Owner.MogMod().radiancePower + 1f));
                //Projectile.Center = Owner.Center;
            }
        }
        public override bool? CanDamage() => HitCooldown >= Projectile.localNPCHitCooldown;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), debuffTime);
            if (Projectile.owner == Main.myPlayer) if (Owner.MogMod().radiancePower < 1f && !hitEnemy)
            {
                Owner.MogMod().radiancePower += 0.05f;
                hitEnemy = true;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), debuffTime);
            if (Projectile.owner == Main.myPlayer) if (Owner.MogMod().radiancePower < 1f)
                Owner.MogMod().radiancePower += 0.05f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius * (Owner.MogMod().radiancePower + 1f), targetHitbox);
    }
}