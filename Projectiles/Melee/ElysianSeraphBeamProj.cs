using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class ElysianSeraphBeamProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        private bool HitNPC = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 60;
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
                if (Projectile.ai[1] >= 5 || Projectile.ai[2] >= 1f)
                    if (Projectile.ai[1] == 5 || Projectile.ai[2] == 1f)
                    {
                        if (Projectile.ai[2] >= 1f)
                            Projectile.ai[2]++;
                        SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                        float dustAmt = 8f;
                        int d = 0;
                        while (d < dustAmt)
                        {
                            Vector2 offset = Vector2.UnitX * 0f;
                            offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt)), default) * new Vector2(1f, 4f);
                            offset = offset.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                            int i = Dust.NewDust(Projectile.Center, 0, 0, DustID.HallowSpray, 0f, 0f, 0, default, 1f);
                            Main.dust[i].scale = 1.5f;
                            Main.dust[i].noGravity = true;
                            Main.dust[i].position = Projectile.Center + offset;
                            Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                            d++;
                        }
                    }
                for (int d = 0; d < 2; d++)
                {
                    Vector2 offset = Vector2.UnitX * -12f;
                    offset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * pi / 24f + d * pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.HallowSpray, Projectile.velocity, 100, default, 1f);
                    dust.noGravity = true;
                    dust.scale = Projectile.ai[1] >= 3 ? 1.25f : 0.75f;
                    dust.position = Projectile.Center + offset;
                    dust.velocity = Projectile.velocity;
                }
                if (Projectile.ai[1] >= 7 || Projectile.ai[2] >= 2f)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 0f)
                    {
                        Vector2 source = Projectile.position;
                        source -= Projectile.velocity * 0.25f;
                        Dust dust = Dust.NewDustPerfect(Projectile.position, 133, Projectile.velocity, 100, default, 0.85f);
                        dust.noGravity = true;
                        dust.position = source;
                        dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        dust.velocity *= 0.1f;
                    }
                }
            }
            if (!HitNPC && (Projectile.ai[1] >= 30 || Projectile.ai[2] >= 1f))
                MogModUtils.HomeInOnNPC(Projectile, true, Projectile.ai[2] >= 1f ? 400f : 250f, 10f, 35f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
                HitNPC = true;
            if (Projectile.ai[2] >= 2f)
                return;
            Player player = Main.player[Projectile.owner];
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (target.type != NPCID.TargetDummy)
                mogPlayer.eSeraphCharge += hit.Crit ? 5 : 3;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.owner == Main.myPlayer)
                HitNPC = true;
            if (Projectile.ai[2] >= 2f)
                return;
            Player player = Main.player[Projectile.owner];
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.eSeraphCharge += 5;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            int dustAmt = Main.rand.Next(4, 10);
            for (int d = 0; d < dustAmt; d++)
            {
                int fire = Dust.NewDust(Projectile.Center, 0, 0, dustAmt > 6 ? DustID.HallowSpray : 133, 0f, 0f, 100, default, 1f);
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