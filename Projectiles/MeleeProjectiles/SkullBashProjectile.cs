using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class SkullBashProjectile : ModProjectile, ILocalizedModType
    {
        // GIVE BASH A WAY TO BLEED MEGA
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        private bool initialized = false;
        public static readonly SoundStyle bashProc = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/SkullBash")
        {
            Volume = 1.3f,
            PitchVariance = .2f,
            MaxInstances = 3
        };
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 10;
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
            SoundEngine.PlaySound(bashProc, Projectile.Center);
            for (int n = 0; n < 40; n++)
            {
                int bash = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 1f);
                Main.dust[bash].fadeIn += 1.2f;
                Main.dust[bash].velocity.Y *= 1.02f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Dazed, 420);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Dazed, 420);
        }
    }
}
