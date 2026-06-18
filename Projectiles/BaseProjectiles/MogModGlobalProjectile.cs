using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.ClasslessProjectiles;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Projectiles.SummonerProjectiles;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        // exists for projectile utils hopefully
        private Random random = new Random();
        public NPC.HitInfo hitInfo;
        public bool CanSplit = true;
        public bool radiantProc = false;
        public bool gunpowderProc = false;
        public bool shivProc = false;
        public bool jidiProc = false;

        public bool fireBullet = false;
        public bool iceBullet = false;
        public bool deathBullet = false;
        public bool daybreakBullet = false;

        // damage caps
        public int gunpowderCap = 40;
        public int shivCap = 400;
        public const int hellfireCap = 600;

        public const int bashCap = 50;
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
            ModContent.ProjectileType<BlackBladeHoldout>(),
            ModContent.ProjectileType<AnchorHoldout>(),
            ModContent.ProjectileType<WyvernJawbladeHoldout>(),
        ];
        // Amount of extra updates that are set in SetDefaults.
        public int defExtraUpdates = -1;
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
            if (fireBullet)
                if (projectile.timeLeft > 200)
                    for (int i = 0; i < 2; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, Main.rand.NextBool() ? DustID.Flare : DustID.Torch, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    }
            if (iceBullet)
                if (projectile.timeLeft > 200)
                    for (int i = 0; i < 2; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, Main.rand.NextBool() ? DustID.Frost : DustID.IceRod, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    }
            if (deathBullet)
                if (projectile.timeLeft > 200)
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
                if (projectile.timeLeft > 200)
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
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                MogPlayer mogPlayer = target.GetModPlayer<MogPlayer>();
                mogPlayer.doParry(target, target.Center);
                modifiers.Cancel();

                int originalOwner = projectile.owner;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    mogPlayer.SyncProjParry(false, originalOwner, target, projectile);
                }

                ParryProjectile(projectile, target.whoAmI);
            }
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
            var source = player.GetSource_OnHit(target);
            int itemDamage = player.HeldItem.damage;
            int enemyMaxHP = target.lifeMax;
            int shivDamage = 0;
            //int gunpowderDamage = 0;
            int gunpowderDamage = gunpowderCap;
            int hellfireDamage = hellfireCap;

            //if (itemDamage <= gunpowderCap)
            //    gunpowderDamage = itemDamage;
            //else
            //    gunpowderDamage = gunpowderCap;
            if (Convert.ToInt32(enemyMaxHP * 0.01) <= shivCap)
                shivDamage = Convert.ToInt32(enemyMaxHP * 0.005) + 50;
            else
                shivDamage = shivCap;

            radiantProc = random.Next(2) == 0;
            gunpowderProc = random.Next(5) == 0;
            jidiProc = random.Next(4) == 0;

            // TODO: fix enemies proccing player accessories
            if (radiantProc)
            {
                if (projectile.owner == player.whoAmI && modPlayer.wearingRadiantArmor && projectile.type != ProjectileType<RadiantBeamProj>() && projectile.DamageType == DamageClass.Magic && modPlayer.radiantCooldown <= 0)
                {
                    modPlayer.radiantCooldown = cooldownTimer;
                    MogModUtils.ProjectileRain(source, target.Center, 100f, 50f, 1500f, 1500f, 10f, ModContent.ProjectileType<RadiantBeamProj>(), Convert.ToInt32(projectile.damage / .75f), projectile.knockBack, projectile.owner);
                }
            }

            if (gunpowderProc)
            {
                if (projectile.owner == player.whoAmI && modPlayer.wearingGunpowderGauntlet && projectile.type != ProjectileType<GunpowderProj>() && projectile.DamageType == DamageClass.Magic && modPlayer.gunpowderCooldown <= 0)
                {
                    modPlayer.gunpowderCooldown = cooldownTimer;
                    int gunpowderProc = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ProjectileType<GunpowderProj>(), gunpowderDamage, 0f, projectile.owner);
                }
            }

            if (jidiProc)
            {
                if (projectile.owner == player.whoAmI && modPlayer.wearingJidiPollenBag && projectile.type != ProjectileType<JidiPollenExplosion>() && (projectile.DamageType == DamageClass.Summon || projectile.DamageType == DamageClass.SummonMeleeSpeed) && modPlayer.jidiPollenCooldown <= 0)
                {
                    modPlayer.jidiPollenCooldown = cooldownTimer;
                    int jidiProc = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ProjectileType<JidiPollenExplosion>(), 1, 0f, projectile.owner);
                }
            }

            shivProc = random.Next(5) == 0;
            if (projectile.owner == player.whoAmI && shivProc && modPlayer.wearingSerratedShiv && modPlayer.shivCooldown <= 0)
            {
                modPlayer.shivCooldown = cooldownTimer;
                hitInfo = new NPC.HitInfo
                {
                    Damage = shivDamage,
                    Knockback = 0,
                    HitDirection = 0,
                    Crit = false,
                    DamageType = DamageClass.Default
                };
                target.StrikeNPC(hitInfo);
                NetMessage.SendStrikeNPC(target, hitInfo);
                Rectangle r = new Rectangle((int)target.position.X, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(210, 180, 140);
                CombatText.NewText(r, textColor, "Strike!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.TrueStrikeProcTextSync);
                    packet.Write(target.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
                doTrueStrikeFX(target.Center);
            }

            // skull basher (melee holdout projectiles only)
            bashProc = Main.rand.Next(7) == 0;
            if (bashProc && modPlayer.wearingGiantsMaul && modPlayer.bashCooldown <= 0 && MeleeHoldouts.Contains(projectile.type))
            {
                modPlayer.bashCooldown = cooldownTimer;
                int bash = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ModContent.ProjectileType<SkullBashProjectile>(), bashCap, 0f, player.whoAmI);
                Rectangle r = new Rectangle((int)target.position.X, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(255, 0, 100);
                CombatText.NewText(r, textColor, "Bash!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.BashProcTextSync);
                    packet.Write(target.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
            }

            // atg and plasma shrimp
            if (projectile.owner == player.whoAmI && (modPlayer.atgActive || modPlayer.plasmaActive) && !voidItems.Contains(projectile.type))
            {
                modPlayer.doATG(damageDone);
            }

            if (projectile.owner == player.whoAmI && modPlayer.polyluteActive && !voidItems.Contains(projectile.type))
            {
                Vector2 kirk = new Vector2(10, 10).RotatedByRandom(MathHelper.ToRadians(360));
                int procChance = random.Next(1, 6);

                if (procChance == 5)
                    Projectile.NewProjectile(source, target.Center, kirk, ModContent.ProjectileType<PolyluteProj>(), Convert.ToInt32(damageDone * .3f) + 1, 3, player.whoAmI);
            }

            if (target.type != NPCID.TargetDummy)
            {
                if (modPlayer.wearingDamascus2 && hit.Crit)
                {
                    int heal = 1;
                    heal *= Convert.ToInt32(player.lifeSteal * 0.01);
                    player.statLife += heal;
                    player.HealEffect(heal);
                    if (player.statLife > player.statLifeMax2)
                        player.statLife = player.statLifeMax2;
                }
                if (modPlayer.wearingSatanic && player.HasBuff(ModContent.BuffType<SatanicBuff>()) && modPlayer.satanicAccCooldown <= 0)
                //if (modPlayer.wearingSatanic) // for testing
                {
                    modPlayer.satanicAccCooldown = cooldownTimer * 2;
                    int heal = (int)(damageDone / 100) + 1;
                    heal *= Convert.ToInt32(player.lifeSteal * 0.01);
                    player.statLife += heal;
                    player.HealEffect(heal);
                    if (player.statLife > player.statLifeMax2)
                        player.statLife = player.statLifeMax2;
                }
            }

            // hellfire armor
            if (modPlayer.wearingHellfireArmor && modPlayer.hellfireCooldown <= 0)
            {
                if (damageDone >= 100)
                {
                    modPlayer.hellfireCooldown = cooldownTimer * 72;
                    int hellfire = Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), hellfireDamage, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                }
            }
        }

        // stolen STRAIGHT from fargos souls mod
        // makes fishing rods spawn more bobbers when wearing certain accessories
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer modPlayer = player.MogMod();


            if (projectile.bobber && CanSplit && source is EntitySource_ItemUse)
            {
                int splitCount = 0;
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
        public static void doTrueStrikeFX(Vector2 position)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath56, position);
            for (int i = 0; i < 40; i++)
            {
                int strike = Dust.NewDust(position, 20, 20, DustID.CopperCoin, 0, 0, 100, default, 2f);
                Main.dust[strike].velocity.Y *= 1.05f;
                Main.dust[strike].noGravity = true;
            }
        }
        #endregion
    }
}