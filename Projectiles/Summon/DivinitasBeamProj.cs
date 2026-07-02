using Microsoft.Xna.Framework;
using MogMod.Items.Armor.FrostMaiden;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Summon
{
    public class DivinitasBeamProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public int DustOne = 156;
        public int DustTwo = DustID.GoldCoin;
        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.alpha = 255;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            float pi = MathHelper.Pi;
            Projectile.ai[0]++;
            Projectile.ai[1]++;
            if (Projectile.ai[0] == 48f)
                Projectile.ai[0] = 0f;
            else
            {
                if (Projectile.ai[1] >= 5)
                    if (Projectile.ai[1] == 5)
                    {
                        SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                        float dustAmt = 16f;
                        int d = 0;
                        while (d < dustAmt)
                        {
                            Vector2 offset = Vector2.UnitX * 0f;
                            offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt)), default) * new Vector2(1f, 4f);
                            offset = offset.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                            int i = Dust.NewDust(Projectile.Center, 0, 0, DustOne, 0f, 0f, 0, default, 1f);
                            Main.dust[i].scale = 1.5f;
                            Main.dust[i].noGravity = true;
                            Main.dust[i].position = Projectile.Center + offset;
                            Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                            d++;
                        }
                    }
                for (int d = 0; d < 4; d++)
                {
                    Vector2 offset = Vector2.UnitX * -12f;
                    offset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * pi / 24f + d * pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustOne, Projectile.velocity, 100, default, 1f);
                    dust.noGravity = true;
                    dust.scale = Projectile.ai[1] >= 3 ? 1.25f : 0.75f;
                    dust.position = Projectile.Center + offset;
                    dust.velocity = Projectile.velocity;
                }
                if (Projectile.ai[1] >= 7)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 0f)
                    {
                        for (int d = 0; d < 2; d++)
                        {
                            Vector2 source = Projectile.position;
                            source -= Projectile.velocity * 0.25f;
                            Dust dust = Dust.NewDustPerfect(Projectile.position, DustTwo, Projectile.velocity, 100, default, 0.85f);
                            dust.noGravity = true;
                            dust.position = source;
                            dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                            dust.velocity *= 0.1f;
                        }
                    }
                }
            }
            if (Projectile.ai[2] >= 0.3f)
            {
                //Main.NewText($"ai 2 == {Projectile.ai[2]}", 200, 200, 255);
                if (Projectile.ai[1] % 15 == 0)
                    if (Projectile.owner == Main.myPlayer)
                        MogModUtils.ProjectileRain(Projectile.GetSource_FromThis(), Projectile.Center, 100f, 20f, 1200f, 1350f, 17f, ModContent.ProjectileType<DivinitasStarProj>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            int dustAmt = Main.rand.Next(4, 10);
            for (int d = 0; d < dustAmt; d++)
            {
                int fire = Dust.NewDust(Projectile.Center, 0, 0, dustAmt > 6 ? DustOne : DustTwo, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.velocity *= 1.1f;
                dust.velocity.Y -= 1f;
                dust.velocity += -Projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                dust.scale = 1f;
                dust.fadeIn = 2f;
                dust.noGravity = true;
            }
        }
    }
}