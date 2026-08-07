using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class ChaosBladeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public static readonly SoundStyle UltraCritSFX = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/UltraCrit")
        {
            Volume = 1.1f,
            PitchVariance = .2f
        };
        public Player Owner => Main.player[Projectile.owner];
        public bool ultraCrit = false;
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;

        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(UltraCritSFX, Projectile.Center);
            ultraCrit = Main.rand.NextFloat(0f, 1f) < ChaosArbiter.UltraCritChance;
        }
        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Vector2 projPos = Projectile.position;
            projPos -= Projectile.velocity;
            int chaosss = Dust.NewDust(projPos, 1, 1, DustID.Blood, 0f, 0f, 0, Color.Red, 0.2f);
            Main.dust[chaosss].position = projPos;
            Main.dust[chaosss].scale = Main.rand.Next(10, 30) * 0.014f;
            Main.dust[chaosss].velocity *= 0.2f;
            Main.dust[chaosss].noLight = false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= Main.rand.NextFloat(0.5f, 2f);
            modifiers.CritDamage += Main.rand.NextFloat(-0.75f, 1.25f);
            if (Main.rand.NextBool(3)) modifiers.Knockback *= Main.rand.NextFloat(0f, 1f);
            else modifiers.Knockback += Main.rand.Next(0, 3);
            if (Main.rand.Next(0, 100 + 1) < (Owner.GetTotalCritChance(Projectile.DamageType) * Main.rand.Next(0, 5 + 1))) modifiers.SetCrit();
            if (ultraCrit) modifiers.CritDamage *= ChaosArbiter.CritMult;
        }
    }
}