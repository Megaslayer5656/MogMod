using MogMod.Utilities;
using MogMod.Common.Classes;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Projectiles.Classless
{
    public class TridentHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public static readonly SoundStyle SpawnSound = new SoundStyle("Terraria/Sounds/Item_8")
        {
            Volume = 1f,
            PitchVariance = 0.2f,
            MaxInstances = -1
        };
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = MeleeRangedMagicDamageClass.Instance;
            Projectile.ArmorPenetration = 30;
        }
        public override void AI()
        {
            Projectile.rotation += 0.3f * (float)Projectile.direction;
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 111 : 92, 0, 0, 100, default, .75f);
            }
            if (Projectile.timeLeft < 200)
                Projectile.ai[0] = 1f;
            if (Projectile.ai[0] >= 1f)
                MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 15f, 15f);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 101, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1f);
            }
        }
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SpawnSound, Projectile.Center);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.CritDamage *= 1.2f;
        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] >= 1f ? null : false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}