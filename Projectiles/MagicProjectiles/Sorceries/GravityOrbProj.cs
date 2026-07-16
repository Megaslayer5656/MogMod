using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GravityOrbProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults() => Main.projFrames[Type] = 8;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.localAI[0] = 1f;
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 randomOffset = Main.rand.NextVector2Circular(Projectile.width / 1.9f, Projectile.height / 1.9f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + randomOffset, Main.rand.NextBool(3) ? 27 : 62, -Projectile.DirectionFrom(Projectile.Center + Projectile.velocity + randomOffset) * Main.rand.NextFloat(0.5f, 1f));
                if (Main.rand.NextBool(3))
                {
                    d.scale *= 1.2f;
                    d.fadeIn = 0.3f;
                }
                d.noLight = true;
                d.noGravity = true;
                d.fadeIn = 0.15f;
                d.scale *= 1.05f;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 10 == 4)
                Projectile.frame++;
            if (Projectile.frame >= 9)
                Projectile.Kill();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.HitDirectionOverride = target.position.X < Owner.MountedCenter.X ? 1 : -1;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.HitDirectionOverride = target.position.X < Owner.MountedCenter.X ? 1 : -1;
    }
}