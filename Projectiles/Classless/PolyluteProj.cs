using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class PolyluteProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public static readonly SoundStyle polylute = new SoundStyle("Terraria/Sounds/Item_105")
        {
            Volume = 1f,
            PitchVariance = 0.2f,
            MaxInstances = -1
        };
        public Player Owner => Main.player[Projectile.owner];
        private float wSpeed = 0f;
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.ShimmerSpark, Projectile.velocity, 100, Color.Pink, 1.5f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(1.1f, 1.617f);
            dust.velocity *= 0.1f;

            if (wSpeed == 0f)
                wSpeed = Projectile.velocity.Length();

            if (Projectile.ai[0] >= 10)
                MogModUtils.HomeInOnNPC(Projectile, true, 200f, wSpeed, 1f);
        }
        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] >= 10 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 12;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int dustAmt = 24;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.ShimmerSpark, dustDirection.X, dustDirection.Y, 100, Color.LightPink, 1.5f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.ShimmerSpark, dustDirection.X, dustDirection.Y, 100, Color.Magenta, 1f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            Projectile.ai[0] = 1f;
            MogPlayer mogPlayer = Owner.GetModPlayer<MogPlayer>();
            if (mogPlayer.polyluteVisual)
                SoundEngine.PlaySound(polylute, Projectile.Center);
        }
    }
}