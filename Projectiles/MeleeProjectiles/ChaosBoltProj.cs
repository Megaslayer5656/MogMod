using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class ChaosBoltProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
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
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] != 3)
                Projectile.Kill();
            if (Projectile.ai[0] == 5 && target.type != NPCID.TargetDummy)
            {
                int heal = 1;
                heal *= Convert.ToInt32(player.lifeSteal * 0.04);
                player.statLife += heal;
                player.HealEffect(heal);
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
            }

            var source = Projectile.GetSource_OnHit(target);
            if (Main.rand.Next(0, 10) == 0) // 1 in 10 chance
            {
                Rectangle r = new Rectangle((int)target.position.X, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(255, 0, 0);
                CombatText.NewText(r, textColor, "Ultra Crit!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.UltraCritTextSync);
                    packet.Write(player.whoAmI);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }

                int randNumProjectiles = Main.rand.Next(1, 4);
                int randDamage = Main.rand.Next(ChaosArbiter.strikeMin, ChaosArbiter.strikeMax);
                for (int i = 0; i < randNumProjectiles; i++)
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ChaosBladeProj>(), randDamage, 0f, Projectile.owner, false, 0f);

                if (target.type != NPCID.TargetDummy)
                {
                    int heal = Main.rand.Next(1, 3);
                    // for SOME REASON player has a default of 70 lifesteal
                    heal *= Convert.ToInt32(player.lifeSteal * 0.08);
                    player.statLife += heal;
                    player.HealEffect(heal);
                    // so we dont go over max life
                    if (player.statLife > player.statLifeMax2)
                        player.statLife = player.statLifeMax2;
                }

                // TODO: make phantom spawns take up empty slots instead of going 0 -> 3
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ChaosArbiterClone>()] <= 3)
                {
                    if (ChaosArbiter.numb <= 3)
                        ChaosArbiter.numb++;
                    else
                        ChaosArbiter.numb = 0;
                    Projectile clone = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ChaosArbiterClone>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ChaosArbiter.numb);
                    clone.OriginalCritChance = Main.rand.Next(10, 30);
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] != 3)
                Projectile.Kill();
            if (Projectile.ai[0] == 5)
            {
                int heal = 1;
                heal *= Convert.ToInt32(player.lifeSteal * 0.04);
                player.statLife += heal;
                player.HealEffect(heal);
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
            }

            var source = Projectile.GetSource_OnHit(target);
            if (Main.rand.Next(0, 10) == 0) // 1 in 10 chance
            {
                Rectangle r = new Rectangle((int)target.position.X, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(255, 0, 0);
                CombatText.NewText(r, textColor, "Ultra Crit!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.UltraCritTextSync);
                    packet.Write(player.whoAmI);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }

                int randNumProjectiles = Main.rand.Next(2, 8);
                int randDamage = Main.rand.Next(ChaosArbiter.strikeMin, ChaosArbiter.strikeMax);
                for (int i = 0; i < randNumProjectiles; i++)
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ChaosBladeProj>(), randDamage, 0f, Projectile.owner, false, 0f);

                int heal = Main.rand.Next(1, 3);
                // for SOME REASON player has a default of 70 lifesteal
                heal *= Convert.ToInt32(player.lifeSteal * 0.08);
                player.statLife += heal;
                player.HealEffect(heal);
                // so we dont go over max life
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;

                // TODO: make phantom spawns take up empty slots instead of random
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ChaosArbiterClone>()] <= 3)
                {
                    ChaosArbiter.numb = Main.rand.Next(0, 4);
                    Projectile clone = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ChaosArbiterClone>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ChaosArbiter.numb);
                    clone.OriginalCritChance = Main.rand.Next(10, 30);
                }
            }
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