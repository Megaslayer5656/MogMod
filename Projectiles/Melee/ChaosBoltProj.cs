using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.NPCs.Global;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Projectiles.Melee
{
    public class ChaosBoltProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public bool ultraCrit = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 600;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
            ultraCrit = Main.rand.NextFloat(0f, 1f) < ChaosArbiter.UltraCritChance;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 5)
                MogModUtils.HomeInOnNPC(Projectile, false, 900f, 10f, 25f);
            if (Projectile.ai[0] == 3)
                Projectile.extraUpdates = 1;
            if (Projectile.ai[0] == 4)
            {
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                float maxSpeed = 8;
                float currentSpeed = Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y;
                if (Projectile.timeLeft > 500)
                    Projectile.velocity *= 0.95f;
                if (Projectile.timeLeft <= 500)
                {
                    MogModUtils.HomeInOnNPC(Projectile, true, 1200f, 15f, 20f);
                    Projectile.ai[1]++;
                    if (currentSpeed < maxSpeed * maxSpeed)
                        Projectile.velocity *= 1.15f;
                }
                if (Projectile.ai[1] <= 10)
                    Projectile.extraUpdates = 0;
                if (Projectile.ai[1] >= 11)
                    Projectile.extraUpdates = 3;
                if (Projectile.ai[1] >= 20)
                    Projectile.ai[1] = 0;
            }

            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -Projectile.velocity.X;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -Projectile.velocity.Y;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * (i * 0.25f);
                    Projectile.alpha = 255;
                    int dagonDust = Dust.NewDust(projPos, Projectile.width, Projectile.height, DustID.CrimsonTorch, 0f, 0f, 0, default, 0.75f);
                    Main.dust[dagonDust].noGravity = true;
                    Main.dust[dagonDust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dagonDust].velocity *= 0.2f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] != 3)
                Projectile.Kill();
            if (Projectile.ai[0] == 5 && target.type != NPCID.TargetDummy)
            {
                int heal = 2;
                Owner.HealLifestealMult(heal);
            }

            var source = Projectile.GetSource_OnHit(target);
            if (ultraCrit && hit.Crit)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.UltraCritTextSync);
                    packet.Write(target.lastInteraction);
                    packet.Write(target.whoAmI);
                    packet.Send();
                }
                else
                    target.MogMod().UltraCritFX(target);
                int randNumProjectiles = Main.rand.Next(1, 4);
                for (int i = 0; i < randNumProjectiles; i++)
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ChaosBladeProj>(), Projectile.damage, 0f, Projectile.owner, false, 0f);

                if (target.type != NPCID.TargetDummy)
                {
                    int heal = Main.rand.Next(1, 3);
                    Owner.HealLifestealMult(heal);
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.ai[0] != 3)
                Projectile.Kill();
            if (Projectile.ai[0] == 5)
            {
                int heal = 2;
                Owner.HealLifestealMult(heal);
            }
            var source = Projectile.GetSource_OnHit(target);
            if (ultraCrit)
            {
                SoundEngine.PlaySound(MogModGlobalNPC.UltraCritSFX, target.Center);
                Rectangle r = new((int)target.Hitbox.X, (int)target.Hitbox.Y - 50, target.Hitbox.Width, target.Hitbox.Height);
                Color textColor = new(255, 0, 0);
                MogModUtils.TextEffect(MiscUtils.GetText("Status.Proc.UltraCrit").ToNetworkText(), r, textColor, true);
                for (int i = 0; i < 30; i++)
                {
                    Vector2 randPos = Main.rand.NextVector2CircularEdge(r.Width / 2f, r.Height / 2f);
                    Dust telegraphDust = Dust.NewDustPerfect(target.Center + randPos, ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, target.DirectionFrom(target.Center + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    telegraphDust.noGravity = true;
                }
                for (int n = 0; n < 6; n++)
                {
                    float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                    Vector2 swirlPos = target.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 20f;
                    Vector2 swirlVelocity = Vector2.Normalize(swirlPos - target.Center).RotatedBy(MathHelper.ToRadians(20)) * 2f;
                    Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.GemRuby, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    swirlDust.noGravity = true;
                    swirlDust.fadeIn = .6f;
                }

                int randNumProjectiles = Main.rand.Next(2, 8);
                for (int i = 0; i < randNumProjectiles; i++)
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ChaosBladeProj>(), Projectile.damage, 0f, Projectile.owner, false, 0f);

                int heal = Main.rand.Next(1, 3);
                Owner.HealLifestealMult(heal);
            }
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
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            if (Projectile.ai[0] == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChaosBoltBoom>(), (int)(Projectile.damage * .75f), Projectile.knockBack, Projectile.owner);
                for (int k = 0; k < 5; k++)
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 218, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            else if (Projectile.ai[0] == 2)
            {
                Random spawnNumb = new Random();
                int[] amount = { 2, 3, 4 };
                int choice = amount[spawnNumb.Next(amount.Length)];

                float offset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < choice; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / choice) - offset).ToRotationVector2() * (choice / 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<ChaosBoltHomingProj>(), (int)(Projectile.damage * .75f), Projectile.knockBack, Projectile.owner);
                }
            }
            else if (Projectile.ai[0] == 5 && Main.rand.Next(0, 10) == 9)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EvilAssChaosStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}