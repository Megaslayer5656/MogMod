using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Classes;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Items.Armor.Damascus;
using MogMod.Items.Armor.FrostMaiden;
using MogMod.Items.Armor.Hellfire;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Items.Weapons.Melee;
using MogMod.NPCs.Global;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.EnemyProjectiles;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Projectiles.Summon;
using MogMod.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static MogMod.Common.Systems.MogModNetcode;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        private Random random = new Random();
        public NPC.HitInfo hitInfo;
        public bool CanSplit = true;
        public bool radiantProc = false;
        public bool gunpowderProc = false;
        public bool shivProc = false;
        public bool jidiProc = false;
        public bool ultraCrit = false;

        public bool fireBullet = false;
        public bool iceBullet = false;
        public bool deathBullet = false;
        public bool daybreakBullet = false;

        public bool gelmirSpell = false;
        public bool meteoriteSpell = false;
        public bool crystalSpell = false;
        public bool deathSpell = false;

        public bool overloadingProj = false;
        public bool fireProj = false;
        public bool toxicProj = false;
        public bool mendingProj = false;

        public bool lusatSpell = false;
        public bool azurSpell = false;

        public int Time = 0;
        private bool doubleDamage = false;
        public Color StarColor = Color.White;

        // damage caps
        public int gunpowderCap = GunpowderGauntlet.DamageCap;
        public int shivCap = SerratedShiv.DamageCap;
        public const int hellfireCap = HellfireMask.DamageCap;

        // for melee holdouts only
        public const int bashCap = GiantsMaul.DamageCap;
        public bool bashProc = false;

        public int cooldownTimer = 5;
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        private static readonly List<int> voidItems =
        [
            ModContent.ProjectileType<PolyluteProj>(),
            ModContent.ProjectileType<PlasmaShrimpProj>()
        ];
        public static readonly List<int> MeleeHoldouts =
        [
            ModContent.ProjectileType<GunlanceHoldout>(),
            ModContent.ProjectileType<GunlanceSpear>(),
            ModContent.ProjectileType<OversizedAnchorHoldout>(),
            ModContent.ProjectileType<WyvernJawbladeHoldout>(),
            ModContent.ProjectileType<BlackBladeHoldout>(),
            ModContent.ProjectileType<EchoSabreHoldout>(),
            ModContent.ProjectileType<SkullBasherHoldout>(),
            ModContent.ProjectileType<AbyssalBladeHoldout>(),
            ModContent.ProjectileType<SangeHoldout>(),
            ModContent.ProjectileType<RiversOfBloodHoldout>(),
            //ModContent.ProjectileType<BladeOfSelvesHoldout>(),
        ];
        // Amount of extra updates that are set in SetDefaults.
        public int defExtraUpdates = -1;
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            SendBloodAI(projectile, bitWriter, binaryWriter);
            binaryWriter.Write(overloadingProj);
            binaryWriter.Write(fireProj);
            binaryWriter.Write(mendingProj);
            binaryWriter.Write(toxicProj);

            binaryWriter.Write(azurSpell);
            binaryWriter.Write(crystalSpell);
            binaryWriter.Write(deathSpell);
            binaryWriter.Write(gelmirSpell);
            binaryWriter.Write(lusatSpell);
            binaryWriter.Write(meteoriteSpell);

            binaryWriter.Write(daybreakBullet);
            binaryWriter.Write(deathBullet);
            binaryWriter.Write(fireBullet);
            binaryWriter.Write(iceBullet);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            ReceiveBloodAI(projectile, bitReader, binaryReader);
            overloadingProj = binaryReader.ReadBoolean();
            fireProj = binaryReader.ReadBoolean();
            mendingProj = binaryReader.ReadBoolean();
            toxicProj = binaryReader.ReadBoolean();

            azurSpell = binaryReader.ReadBoolean();
            crystalSpell = binaryReader.ReadBoolean();
            deathSpell = binaryReader.ReadBoolean();
            gelmirSpell = binaryReader.ReadBoolean();
            lusatSpell = binaryReader.ReadBoolean();
            meteoriteSpell = binaryReader.ReadBoolean();

            daybreakBullet = binaryReader.ReadBoolean();
            deathBullet = binaryReader.ReadBoolean();
            fireBullet = binaryReader.ReadBoolean();
            iceBullet = binaryReader.ReadBoolean();
        }
        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            // faster hook ai
            if (mogPlayer.wearingBoneArmor && projectile.aiStyle == ProjAIStyleID.Hook)
            {
                int cap = projectile.type == ProjectileID.QueenSlimeHook ? 4 : 1;
                if (projectile.extraUpdates < cap)
                    projectile.extraUpdates += cap;
            }
            return true;
        }
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            Time++;
            if (mogPlayer.wearingChaosDice) ultraCrit = Main.rand.NextBool(ChaosDice.UltraCritChance);

            BloodAI(projectile);
            if (projectile.Opacity > 0 && projectile.scale > 0.01f)
            {
                if (fireBullet)
                    for (int i = 0; i < 2; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, Main.rand.NextBool() ? DustID.Flare : DustID.Torch, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    }
                if (iceBullet)
                    for (int i = 0; i < 2; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, Main.rand.NextBool() ? DustID.Frost : DustID.IceRod, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    }
                if (deathBullet)
                {
                    float helixOffset = (float)Math.Sin(projectile.timeLeft / 25f * MathHelper.TwoPi) * -8f;
                    Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation);

                    for (int i = 0; i < 2; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, Main.rand.NextBool() ? DustID.DesertTorch : DustID.CrimsonSpray, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    }
                }
                if (daybreakBullet)
                {
                    float helixOffset = (float)Math.Sin(projectile.timeLeft / 25f * MathHelper.TwoPi) * -8f;
                    Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation);

                    for (int i = 0; i < 2; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, Main.rand.NextBool() ? 174 : DustID.SolarFlare, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    }
                }
            }
            Vector2 dustNumb = new(1.6f, 2f);
            if (gelmirSpell)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? DustID.Lava : DustID.Flare, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                dust.velocity *= 0.1f;
            }
            if (meteoriteSpell)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.PurpleCrystalShard, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                dust.velocity *= 0.1f;
            }
            if (crystalSpell)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? 67 : DustID.BlueCrystalShard, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                dust.velocity *= 0.1f;
            }
            if (deathSpell)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? DustID.Asphalt : DustID.MothronEgg, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                dust.velocity *= 0.1f;
            }
            if (lusatSpell)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? DustID.Shadowflame : DustID.HallowedPlants, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                dust.velocity *= 0.1f;
            }
            if (azurSpell)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? DustID.MagnetSphere : DustID.ApprenticeStorm, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                dust.velocity *= 0.1f;
            }
            /* doubles damage if channeled for 3 seconds (idk what to use this for)
            if (player.channel && Time >= 180 && !doubleDamage)
            {
                doubleDamage = true;
                projectile.damage = (int)(projectile.damage * 1.5f);
            }
            */
            if (projectile.CountsAsClass(DamageClass.Magic) || projectile.CountsAsClass(DamageClass.MagicSummonHybrid))
            {
                if (mogPlayer.wearingFrostMagic)
                {
                    if (Time % 10 == 0)
                    {
                        if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.NorthPoleSnowflake] < FrostMaidenMagic.ShardMax && projectile.type != ProjectileID.NorthPoleSnowflake)
                        {
                            int crystalDamage = MogModUtils.DamageSoftCap(projectile.damage * FrostMaidenMagic.ShardDamage, FrostMaidenMagic.ShardCap);

                            int shard = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileID.NorthPoleSnowflake, crystalDamage, 1f, projectile.owner);
                            Main.projectile[shard].DamageType = projectile.DamageType;
                        }
                    }
                }
            }
            if (projectile.CountsAsClass<MeleeDamageClass>() && mogPlayer.wearingAghGauntlet && mogPlayer.aghGauntletVisual)
            {
                if (Main.rand.NextBool(3))
                {
                    int aghs = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.RainbowMk2, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 100, Color.BlueViolet, 1.25f);
                    Main.dust[aghs].noGravity = true;
                }
            }
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            MogPlayer mogPlayer = target.MogMod();
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                mogPlayer.doParry(target, target.Center);
                modifiers.Cancel();

                int originalOwner = projectile.owner;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    mogPlayer.SyncProjParry(false, originalOwner, target, projectile);
                }

                ParryProjectile(projectile, target.whoAmI);
            }

            int addedDamage = 0;
            if (mogPlayer.wearingVanguard && (Main.rand.NextFloat(0f, 1f) < Vanguard.DamageBlockChance))
            {
                mogPlayer.ApplyDamageReducedProc();
                addedDamage -= Vanguard.SelfDamageReduction;
            }
            if (mogPlayer.wearingCrimsonGuard && (Main.rand.NextFloat(0f, 1f) < CrimsonGuard.DamageBlockChance))
            {
                mogPlayer.ApplyDamageReducedProc();
                addedDamage -= CrimsonGuard.SelfDamageReduction;
                //modifiers.Cancel();
            }
            modifiers.FinalDamage.Flat += addedDamage;
        }
        public static void ParryProjectile(Projectile projectile, int newOwner)
        {
            projectile.velocity *= -1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.damage *= 5;
            projectile.owner = newOwner;
            projectile.netUpdate = true;
            projectile.netUpdate2 = true;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            MogModGlobalNPC mogNPC = target.MogMod();
            var source = player.GetSource_OnHit(target);
            int enemyMaxHP = target.lifeMax;
            int hellfireDamage = MogModUtils.DamageSoftCap(damageDone * HellfireMask.DamageMult, hellfireCap);
            int gunpowderDamage = MogModUtils.DamageSoftCap(damageDone * GunpowderGauntlet.DamageMult, gunpowderCap);
            int overloadingDamage = (int)(damageDone * OverloadingAspect.DamageMult);

            radiantProc = random.Next(2) == 0;
            gunpowderProc = random.Next(5) == 0;
            jidiProc = random.Next(4) == 0;

            if (hit.Damage <= 0)
                return;
            if (projectile.owner == player.whoAmI)
            {
                if (!projectile.npcProj && !projectile.trap && !projectile.hostile)
                {
                    if (radiantProc)
                    {
                        if (modPlayer.wearingRadiantArmor && projectile.type != ProjectileType<RadiantBeamProj>() && (projectile.DamageType == DamageClass.Magic || projectile.DamageType == SorceryDamageClass.Instance) && modPlayer.radiantCooldown <= 0)
                        {
                            modPlayer.radiantCooldown = cooldownTimer;
                            MogModUtils.ProjectileRain(source, target.Center, 100f, 50f, 1500f, 1500f, 10f, ModContent.ProjectileType<RadiantBeamProj>(), Convert.ToInt32(projectile.damage / .75f), projectile.knockBack, projectile.owner);
                        }
                    }

                    if (gunpowderProc)
                    {
                        if (modPlayer.wearingGunpowderGauntlet && projectile.type != ProjectileType<GunpowderProj>() && (projectile.DamageType == DamageClass.Magic || projectile.DamageType == SorceryDamageClass.Instance) && modPlayer.gunpowderCooldown <= 0)
                        {
                            modPlayer.gunpowderCooldown = cooldownTimer;
                            int gunpowderProc = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ProjectileType<GunpowderProj>(), gunpowderDamage, 0f, projectile.owner);
                        }
                    }

                    if (jidiProc)
                    {
                        if (modPlayer.wearingJidiPollenBag && projectile.type != ProjectileType<JidiPollenExplosion>() && (projectile.DamageType == DamageClass.Summon || projectile.DamageType == DamageClass.SummonMeleeSpeed) && modPlayer.jidiPollenCooldown <= 0)
                        {
                            modPlayer.jidiPollenCooldown = cooldownTimer;
                            int jidiProc = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ProjectileType<JidiPollenExplosion>(), 1, 0f, projectile.owner);
                        }
                    }

                    shivProc = random.Next(5) == 0;
                    if (shivProc && modPlayer.wearingSerratedShiv && modPlayer.shivCooldown <= 0)
                    {
                        modPlayer.shivCooldown = cooldownTimer * 4;
                        mogNPC.ApplyTrueStrikeProc(target, player);
                    }

                    // skull basher (melee holdout projectiles only)
                    bashProc = Main.rand.Next(7) == 0;
                    if (bashProc && modPlayer.wearingGiantsMaul && modPlayer.bashCooldown <= 0 && MeleeHoldouts.Contains(projectile.type))
                    {
                        modPlayer.bashCooldown = cooldownTimer;
                        target.MogMod().ApplyBashProc(target, player, damageDone);
                    }

                    // atg and plasma shrimp
                    if (modPlayer.atgActive || modPlayer.plasmaActive)
                    {
                        if (Main.zenithWorld)
                            modPlayer.doATG(damageDone);
                        else if (!voidItems.Contains(projectile.type))
                            modPlayer.doATG(damageDone);
                    }

                    if (modPlayer.polyluteActive && !voidItems.Contains(projectile.type))
                    {
                        Vector2 kirk = new Vector2(10, 10).RotatedByRandom(MathHelper.ToRadians(360));
                        int procChance = random.Next(1, 6);

                        if (procChance == 5)
                            Projectile.NewProjectile(source, target.Center, kirk, ModContent.ProjectileType<PolyluteProj>(), Convert.ToInt32(damageDone * .3f) + 1, 3, player.whoAmI);
                    }

                    // hellfire armor
                    if (modPlayer.wearingHellfireArmor && modPlayer.hellfireCooldown <= 0)
                    {
                        if (Main.zenithWorld)
                        {
                            if (damageDone <= HellfireMask.DamageCheck)
                            {
                                modPlayer.hellfireCooldown = cooldownTimer * (HellfireMask.BoomCooldown / 5);
                                int hellfire = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), (int)(hellfireDamage * 0.1f), 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                            }
                        }
                        else if (damageDone >= HellfireMask.DamageCheck)
                        {
                            modPlayer.hellfireCooldown = cooldownTimer * (HellfireMask.BoomCooldown / 5);
                            int hellfire = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), hellfireDamage, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                        }
                    }

                    if (modPlayer.wearingToxic && modPlayer.toxicCooldown <= 0)
                    {
                        modPlayer.toxicCooldown = 2;
                        mogNPC.toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin - 6, NoxiousAspect.DamageMax - 20);
                        if (Main.hardMode)
                            mogNPC.toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin, NoxiousAspect.DamageMax);
                        //Main.NewText($"sd phonk damage is: {mogNPC.toxicDamage}");
                    }
                    int overloadingType = ModContent.ProjectileType<OverloadingOrbProj>();
                    if (modPlayer.wearingOverloading && modPlayer.overloadingCooldown <= 0 && projectile.type != overloadingType)
                    {
                        modPlayer.overloadingCooldown = cooldownTimer;
                        int overloading = Projectile.NewProjectile(source, target.Center, Vector2.Zero, overloadingType, overloadingDamage, 0f, player.whoAmI, ai2: 1f);
                    }

                    if (target.type != NPCID.TargetDummy)
                    {
                        if (modPlayer.wearingDamascus2 && hit.Crit && Main.zenithWorld)
                        {
                            player.Hurt(PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.Damascus").ToNetworkText(player.name)), 5, -player.direction, false, false, -1, false, 9999, 0, 0);
                            player.immune = false;
                            player.immuneTime = 0;
                        }
                        else if (modPlayer.wearingDamascus2 && hit.Crit)
                        {
                            int heal = 1;
                            player.HealLifestealMult(heal);
                        }
                        if (modPlayer.wearingSatanic && player.HasBuff(ModContent.BuffType<SatanicBuff>()) && modPlayer.satanicAccCooldown <= 0)
                        //if (modPlayer.wearingSatanic) // for testing
                        {
                            modPlayer.satanicAccCooldown = cooldownTimer * 2;
                            int heal = (int)(damageDone / 100) + 1;
                            player.HealLifestealMult(heal);
                        }
                        /*
                        if (((modPlayer.wearingNihilumRanged && projectile.CountsAsClass(DamageClass.Ranged)) || (modPlayer.wearingNihilumMagic && projectile.CountsAsClass(DamageClass.Magic))) && modPlayer.VoniumLifeCooldown <= 0)
                        {
                            modPlayer.VoniumLifeCooldown = cooldownTimer * 4;
                            int heal = (int)(damageDone / 100) + 1;
                            heal *= Convert.ToInt32(player.lifeSteal * 0.01);
                            player.statLife += heal;
                            player.HealEffect(heal);
                            if (player.statLife > player.statLifeMax2)
                                player.statLife = player.statLifeMax2;
                        }
                        */
                    }
                    if (modPlayer.wearingChaosDice && ultraCrit && hit.Crit)
                    {
                        if (target.type != NPCID.TargetDummy)
                            player.HealLifestealMult(1);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            ModPacket packet = Mod.GetPacket();
                            packet.Write((byte)MogModMessageType.UltraCritTextSync);
                            packet.Write(target.lastInteraction);
                            packet.Write(target.whoAmI);
                            packet.Send();
                        }
                        else
                        {
                            target.MogMod().UltraCritFX(target);
                        }
                    }
                }
            }
        }
        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            MogPlayer mogPlayer = target.MogMod();
            MogModGlobalProjectile mogProj = projectile.MogMod();
            if (info.Damage <= 0)
                return;
            
            if (projectile.owner == Main.myPlayer)
            {
                int overloadingType = ModContent.ProjectileType<HostileOverloadingOrbProj>();
                if (overloadingProj && projectile.type != overloadingType)
                {
                    int damage = (int)(Main.masterMode ? projectile.damage * 0.3f : Main.expertMode ? projectile.damage * 0.2f : projectile.damage * 0.1f);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, overloadingType, damage, 2f, Main.myPlayer, ai1: 1f);
                }
                if (fireProj)
                {
                    int buffType = Main.hardMode ? ModContent.BuffType<BlazingDebuff>() : BuffID.OnFire;
                    int duration = Main.hardMode ? 240 : 180;
                    target.AddBuff(buffType, duration);
                }
                if (toxicProj)
                {
                    if (!target.HasBuff<ToxicDebuff>())
                        target.AddBuff(ModContent.BuffType<ToxicDebuff>(), 360);
                    mogPlayer.toxicDamage += Main.rand.Next(ToxicDebuff.DamageMin, ToxicDebuff.DamageMax);
                    if (Main.hardMode)
                        mogPlayer.toxicDamage += Main.rand.Next(ToxicDebuff.DamageMin + 20, ToxicDebuff.DamageMax + 20);
                }
                if (mendingProj)
                {
                    int duration = Main.hardMode ? 1080 : 720;
                    target.AddBuff(ModContent.BuffType<HealingDisabledDebuff>(), duration);
                }
                if (mogProj.fireBullet)
                    target.AddBuff(BuffID.OnFire3, 180);
                if (mogProj.iceBullet)
                    target.AddBuff(BuffID.Frostburn2, 180);
                if (mogProj.deathBullet)
                    target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), 180);
                if (mogProj.daybreakBullet)
                    target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 180);
                if (mogProj.gelmirSpell)
                    target.AddBuff(BuffID.OnFire3, 180);
            }
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (mogPlayer.wearingDamascus1 && Main.zenithWorld)
                modifiers.CritDamage *= DamascusHelm.GFBCritMult;
            else if (mogPlayer.wearingDamascus1)
                modifiers.CritDamage *= DamascusHelm.CritMult + 1;
            if (crystalSpell)
                modifiers.CritDamage *= 1.2f;
            if (mogPlayer.wearingChaosDice && ultraCrit)
                modifiers.CritDamage *= ChaosDice.CritMult;
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            int overloadingDamage = (int)(player.HeldItem.damage * OverloadingAspect.DamageMult);

            if (projectile.owner == Main.myPlayer)
            {
                int friendlyOverloadingType = ModContent.ProjectileType<OverloadingOrbProj>();
                int hostileOverloadingType = ModContent.ProjectileType<HostileOverloadingOrbProj>();
                if (!projectile.npcProj && !projectile.trap && !projectile.hostile)
                {
                    if (modPlayer.wearingOverloading && modPlayer.overloadingCooldown <= 0 && projectile.type != friendlyOverloadingType)
                    {
                        modPlayer.overloadingCooldown = cooldownTimer;
                        int overloading = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, friendlyOverloadingType, overloadingDamage, 0f, player.whoAmI);
                    }
                }
                if (overloadingProj && projectile.type != hostileOverloadingType)
                {
                    int damage = (int)(Main.masterMode ? projectile.damage * 0.3f : Main.expertMode ? projectile.damage * 0.2f : projectile.damage * 0.1f);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, hostileOverloadingType, damage, 2f, Main.myPlayer);
                }
            }
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //    return;
            Player player = Main.player[projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            
            bool CheckWeapon(int item)
            {
                if (player.HeldItem.type == item && source is EntitySource_ItemUse)
                    return true;
                return false;
            }

            if (projectile.DamageType == SorceryDamageClass.Instance)
            {
                projectile.netImportant = true;
                projectile.netUpdate = true;
                if (CheckWeapon(ModContent.ItemType<GelmirGlintstoneStaff>()))
                    gelmirSpell = true;
                if (CheckWeapon(ModContent.ItemType<MeteoriteStaff>()))
                    meteoriteSpell = true;
                if (CheckWeapon(ModContent.ItemType<CrystalStaff>()))
                    crystalSpell = true;
                if (CheckWeapon(ModContent.ItemType<PrinceOfDeathsStaff>()))
                    deathSpell = true;
                if (CheckWeapon(ModContent.ItemType<LusatsGlintstoneStaff>()))
                    lusatSpell = true;
                if (CheckWeapon(ModContent.ItemType<AzursGlintstoneStaff>()))
                    azurSpell = true;
                //NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
            }
            if (source is EntitySource_Parent { Entity: Projectile Owner })
            {
                projectile.netImportant = true;
                projectile.netUpdate = true;
                if (Owner.MogMod().gelmirSpell)
                    gelmirSpell = true;
                if (Owner.MogMod().meteoriteSpell)
                    meteoriteSpell = true;
                if (Owner.MogMod().crystalSpell)
                    crystalSpell = true;
                if (Owner.MogMod().deathSpell)
                    deathSpell = true;
                if (Owner.MogMod().lusatSpell)
                    lusatSpell = true;
                if (Owner.MogMod().azurSpell)
                    azurSpell = true;
                if (Owner.MogMod().overloadingProj)
                    overloadingProj = true;
                if (Owner.MogMod().fireProj)
                    fireProj = true;
                if (Owner.MogMod().toxicProj)
                    toxicProj = true;
                if (Owner.MogMod().mendingProj)
                    mendingProj = true;
                //NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
            }
            if (source is EntitySource_Parent { Entity: NPC npc })
            {
                MogModGlobalNPC mogNPC = npc.MogMod();
                if (!npc.friendly)
                {
                    projectile.netImportant = true;
                    projectile.netUpdate = true;
                    if (mogNPC.overloadingElite)
                        overloadingProj = true;
                    if (mogNPC.fireElite)
                        fireProj = true;
                    if (mogNPC.toxicElite)
                        toxicProj = true;
                    if (mogNPC.healingElite)
                        mendingProj = true;
                    //NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
                }
            }

            // stolen STRAIGHT from fargos souls mod
            // makes fishing rods spawn more bobbers when wearing certain accessories
            if (projectile.bobber && CanSplit && source is EntitySource_ItemUse)
            {
                int splitCount = 0;
                if (modPlayer.wearingScavVest)
                    splitCount += 2;
                if (modPlayer.wearingFishSlop1)
                    splitCount += 5;
                if (modPlayer.wearingFishSlop2)
                    splitCount += 10;
                if (player.whoAmI == Main.myPlayer && splitCount > 0)
                    SplitProj(projectile, splitCount, MathHelper.Pi / 3, 1);
            }

            ////spawn extra projectiles
            //if (projectile.type != ProjectileID.LastPrismLaser && CanSplit && source is EntitySource_ItemUse)
            //{
            //    int splitCount = 0;
            //    if (modPlayer.wearingWraithPact)
            //        splitCount += 3;
            //    // spawn projectiles in an arc
            //    if (player.whoAmI == Main.myPlayer && splitCount > 0)
            //        SplitProj(projectile, splitCount, MathHelper.Pi / 3, 1);
            //}
        }

        #region slop
        public static List<Projectile> SplitProj(Projectile projectile, int number, float maxSpread, float damageRatio, bool allowMoreSplit = false)
        {
            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }
            List<Projectile> projList = [];
            Projectile split;
            double spread = maxSpread / number;
            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = j == 0 ? 1 : -1;
                    split = NewProjectileDirectSafe(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    if (split != null)
                    {
                        split.ai[2] = projectile.ai[2];
                        split.localAI[0] = projectile.localAI[0];
                        split.localAI[1] = projectile.localAI[1];
                        split.localAI[2] = projectile.localAI[2];

                        split.friendly = projectile.friendly;
                        split.hostile = projectile.hostile;
                        split.timeLeft = projectile.timeLeft;
                        split.DamageType = projectile.DamageType;
                        projList.Add(split);
                    }
                }
            }

            return projList;
        }
        public static Projectile NewProjectileDirectSafe(IEntitySource spawnSource, Vector2 pos, Vector2 vel, int type, int damage, float knockback, int owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            int p = Projectile.NewProjectile(spawnSource, pos, vel, type, damage, knockback, owner, ai0, ai1);
            return p < Main.maxProjectiles ? Main.projectile[p] : null;
        }
        #endregion
    }
}