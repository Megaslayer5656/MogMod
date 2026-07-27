using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class AnchorSmashProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 240;

            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy)
                Owner.AddBuff(ModContent.BuffType<KrakenShellDebuff>(), OversizedAnchor.BuffTime);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Owner.AddBuff(ModContent.BuffType<KrakenShellDebuff>(), OversizedAnchor.BuffTime);
        public override void AI()
        {
           for (int i = 0; i < 60; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 101);
                int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 111);
            }
        }
    }
}