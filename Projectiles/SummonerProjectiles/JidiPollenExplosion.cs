using MogMod.Buffs.Debuffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.SummonerProjectiles
{
    public class JidiPollenExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.SummonerProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.extraUpdates = 1;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.width -= 2;
            Projectile.height -= 2;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
            for (int n = 0; n < 20; n++)
            {
                int flare = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JungleSpore, 0f, 0f, 100, default, 1f);
                Main.dust[flare].fadeIn += 1.2f;
                Main.dust[flare].velocity.Y *= 1.02f;
                Main.dust[flare].noGravity = true;
                int smoke = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JungleTorch, 0f, 0f, 100, default, 1f);
                Main.dust[smoke].fadeIn += 1.2f;
                Main.dust[smoke].velocity.Y *= 1.02f;
                Main.dust[smoke].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<JidiPollenBagDebuff>(), 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<JidiPollenBagDebuff>(), 300);
        }
    }
}