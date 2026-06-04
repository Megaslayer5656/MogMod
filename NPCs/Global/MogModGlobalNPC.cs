using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Ammo;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Ores;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Items.Weapons.Melee;
using MogMod.NPCs.Enemies;
using MogMod.NPCs.ProjectileEnemies;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.ClasslessProjectiles;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Tiles.Ores;
using MogMod.Utilities;
using MogMod.World;
using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.NPCs.Global
{
    public class MogModGlobalNPC : GlobalNPC
    {
        #region Setup
        // debuffs ID
        public bool divineDebuff;
        public bool skadiDebuff;
        public bool freezingDebuff;
        public bool aghDebuff;
        public bool wingsOfLightDebuff;
        public bool ghostflameDebuff;
        public bool jidiDebuff;
        public bool blackBladeDebuff;
        public bool shivasDebuff;
        public bool infernoDebuff;

        public NPC.HitInfo hitInfo;
        public int maxBlood = 150;
        public int currentBlood = 0;

        // debuff stat changes
        public const int skadiNumb = 25;
        public static float skadiMult = 1 - skadiNumb / 100f;
        public const int jidiNumb = 10;
        public const int shivaNumb = 15;
        public static float shivaMult = 1 - shivaNumb / 100f;

        // damage caps
        public const int bashCap = 50;
        public const int shivCap = 400;
        public const int hellfireCap = 600;

        public bool markedByMarker;

        // procs
        public bool bashProc = false;
        public bool shivProc = false;

        Random rand = new Random();

        public int cooldownTimer = 5;
        public override bool InstancePerEntity => true;

        public static readonly SoundStyle BloodCrit = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/BloodCrit")
        {
            Volume = .7f,
            PitchVariance = .2f,
        };

        // 
        public bool hellEpstein = false;
        public static LocalizedText FaeOreText { get; private set; }
        public static LocalizedText HellfireEssenceText { get; private set; }
        public override void SetStaticDefaults()
        {
            FaeOreText = Mod.GetLocalization($"WorldGen.{nameof(FaeOreText)}");
            HellfireEssenceText = Mod.GetLocalization($"WorldGen.{nameof(HellfireEssenceText)}");
        }
        #endregion
        // modifies vanilla npc shop
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.SkeletonMerchant)
                shop.Add(new Item(ModContent.ItemType<AstrologersStaff>()));
        }

        #region NPC Drops
        // LEDX and REDX chance to drop
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new CommonDrop(ModContent.ItemType<LedX>(), 10000, 1, 1, 1));
            globalLoot.Add(new CommonDrop(ModContent.ItemType<RedX>(), 100000, 1, 1, 1));
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LeadingConditionRule postFish = npcLoot.DefineConditionalDropSet(DropHelper.PostFish());
            LeadingConditionRule postEoL = npcLoot.DefineConditionalDropSet(DropHelper.PostEoL());
            LeadingConditionRule postOneMech = npcLoot.DefineConditionalDropSet(DropHelper.PostOneMech());
            LeadingConditionRule postAllMech = npcLoot.DefineConditionalDropSet(DropHelper.PostAllMech());
            switch (npc.type)
            {
                case NPCID.Tim:
                case NPCID.RuneWizard:
                    npcLoot.RemoveWhere(rule => true, false);
                    ItemDropRule.OneFromOptions(1, ItemID.WizardHat, ModContent.ItemType<GlintstoneArc>());
                    break;
                case NPCID.CrimsonAxe:
                case NPCID.CursedHammer:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExplosiveGhostflame>(), 15, 1, 1));
                    break;
                case NPCID.Golem:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LizhardBloodVial>(), 1, 1, 2));
                    break;
                case NPCID.Shark:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 5, 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OceanHeart>(), 100, 1, 1));
                    postFish.Add(ModContent.ItemType<BrinyRind>(), 4, 3, 5);
                    break;
                case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                case NPCID.PigronHallow:
                    postFish.Add(ModContent.ItemType<BrinyRind>(), 4, 3, 5);
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BrinyRind>(), 1, 7, 14));
                    break;
                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlinkDagger>(), 10, 1, 1));
                    break;
                case NPCID.GoblinSorcerer:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SearingSignet>(), 20, 1, 1));
                    break;
                case NPCID.GoblinSummoner:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SearingSignet>(), 5, 1, 1));
                    break;
                case NPCID.RainbowSlime:
                case NPCID.LightMummy:
                    postEoL.Add(ModContent.ItemType<FaeOre>(), 2, 12, 20);
                    break;
            }
        }
        #endregion

        #region AI && On Hit Effects
        public override void AI(NPC npc)
        {
            maxBlood = Convert.ToInt32(npc.lifeMax * .05 + npc.defense); //(This scaling still could change, especially for different difficulties)
            if (maxBlood < 150) //Sets lower bound of possible max blood
            {
                maxBlood = 150;
            }
            if (hellEpstein)
            {
                Lighting.AddLight(npc.Center, Color.OrangeRed.ToVector3());
                for (int n = 0; n < 6; n++)
                {
                    float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                    Vector2 swirlPos = npc.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 220f;
                    Vector2 swirlVelocity = Vector2.Normalize(swirlPos - npc.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f;
                    Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.CopperCoin, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    swirlDust.noGravity = true;
                }
            }

        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            MogGlobalItem globalItem = item.GetGlobalItem<MogGlobalItem>();
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Tell server that this NPC was hit with this item
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.AddBloodFromItem);
                packet.Write(npc.whoAmI);
                packet.Write(player.whoAmI);
                packet.Write(item.type);
                packet.Send();
            }
            else
            {
                // Server or singleplayer
                AddItemBlood(npc, player, item);
            }

            if (item.type == ModContent.ItemType<TheMarker>())
            {
                Vector2 velocity = new Vector2(20f, 20f).RotatedByRandom(MathHelper.ToRadians(360));
                velocity.Normalize();
                velocity *= 10f;
                float rotation = velocity.ToRotation();

                // Spawn locally immediately
                SpawnMarkerProjectile(npc, player, item, velocity, rotation);

                // Send packet to server for syncing
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    mogPlayer.SyncMarkerProj(false, npc, player, item, velocity, rotation);
                }
            }

            int itemDamage = player.HeldItem.damage;
            int enemyMaxHP = npc.lifeMax;
            //int bashDamage = 0;
            int bashDamage = bashCap;
            int shivDamage = 0;
            //if (itemDamage <= bashCap)
            //    bashDamage = itemDamage;
            //else
            //    bashDamage = bashCap;
            if (Convert.ToInt32(enemyMaxHP * 0.01) <= shivCap)
                shivDamage = Convert.ToInt32(enemyMaxHP * 0.005) + 50;
            else
                shivDamage = shivCap;
            int hellfireDamage = hellfireCap;

            // skull basher
            var source = player.GetSource_OnHit(npc);
            bashProc = rand.Next(7) == 0;
            if (bashProc && mogPlayer.wearingGiantsMaul && mogPlayer.bashCooldown <= 0)
            {
                mogPlayer.bashCooldown = cooldownTimer;
                int bash = Projectile.NewProjectile(source, npc.Center, new Vector2(10f, 10f), ModContent.ProjectileType<SkullBashProjectile>(), bashDamage, 0f, player.whoAmI);
                Rectangle r = new Rectangle((int)npc.position.X, (int)npc.position.Y - 50, npc.width, npc.height);
                Color textColor = new Color(255, 0, 100);
                CombatText.NewText(r, textColor, "Bash!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.BashProcTextSync);
                    packet.Write(npc.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
            }

            // serrated shiv
            shivProc = rand.Next(5) == 0;
            if (shivProc && mogPlayer.wearingSerratedShiv && mogPlayer.shivCooldown <= 0)
            {
                mogPlayer.shivCooldown = cooldownTimer;
                hitInfo = new NPC.HitInfo
                {
                    Damage = shivDamage,
                    Knockback = 0,
                    HitDirection = 0,
                    Crit = false,
                    DamageType = DamageClass.Default
                };
                npc.StrikeNPC(hitInfo);
                NetMessage.SendStrikeNPC(npc, hitInfo);
                Rectangle r = new Rectangle((int)npc.position.X, (int)npc.position.Y - 50, npc.width, npc.height);
                Color textColor = new Color(210, 180, 140);
                CombatText.NewText(r, textColor, "Strike!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.TrueStrikeProcTextSync);
                    packet.Write(npc.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
                doTrueStrikeFX(npc.Center);
            }

            // atg and plasma shrimp
            if (mogPlayer.atgActive || mogPlayer.plasmaActive)
            {
                mogPlayer.doATG(damageDone);
            }

            if (mogPlayer.polyluteActive)
            {
                Vector2 kirk = new Vector2(-10, 10).RotatedByRandom(MathHelper.ToRadians(360));
                int procChance = rand.Next(1, 6);

                if (procChance == 5)
                    Projectile.NewProjectile(source, npc.Center, kirk, ModContent.ProjectileType<PolyluteProj>(), Convert.ToInt32(damageDone * .3f) + 1, 3, player.whoAmI);
            }

            // hellfire armor
            if (mogPlayer.wearingHellfireArmor && mogPlayer.hellfireCooldown <= 0)
            {
                if (damageDone >= 100)
                {
                    mogPlayer.hellfireCooldown = cooldownTimer * 72;
                    int hellfire = Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), hellfireDamage, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                }
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            MogPlayer mogPlayer = Main.player[projectile.owner].GetModPlayer<MogPlayer>();

            //if (projectile.type == ModContent.ProjectileType<FrozenSpearProjectile>() || projectile.type == ModContent.ProjectileType<DreadsProj>() || projectile.type == ModContent.ProjectileType<DrowRangerArrow>() || mogPlayer.wearingFrostArmor)
            //{
            //   if (npc.life < 1)
            //    {
            //        if (npc.HasBuff(ModContent.BuffType<FreezingDebuff>())) //This gives errors sometimes but still works. I'll look into it later. Also this whole thing is needed bc of some funky stuff going on with when the npc's debuffs get removed when they die.
            //        {
            //            if (Main.netMode != NetmodeID.MultiplayerClient)
            //            {
            //                npc.DelBuff(ModContent.BuffType<FreezingDebuff>());
            //            }
            //        }
            //        int numSplits = 6;
            //        float angleVariance = MathHelper.TwoPi / numSplits;
            //        Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.ToRadians(45));
            //
            //       for (int i = 0; i < numSplits; ++i)
            //        {
            //            projVec = projVec.RotatedBy(angleVariance);
            //            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, projVec, ProjectileID.Blizzard, 50, 1f, Main.myPlayer);
            //        }
            //    }
            //}

            int bloodToAdd = projectile.GetGlobalProjectile<MogModGlobalProjectileBleed>().bloodDamage;
            

            // add another blood accessory
            if (mogPlayer.exultationEquipped)
            {
                bloodToAdd = (int)(bloodToAdd * 1.15f);
            }

            if (mogPlayer.mercyBladeEquipped)
                bloodToAdd = (int)(bloodToAdd * 1.2f);

            if (mogPlayer.wearingWhiteArmor)
            {
                bloodToAdd = (int)(bloodToAdd * 1.2f);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient) //If you do anything else in this method do it before here bc this returns sometimes
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.AddBloodFromProjectile);
                packet.Write(npc.whoAmI);
                packet.Write(bloodToAdd);
                packet.Send();
                return;
            }
                AddProjectileBlood(npc, bloodToAdd);
            
        }
        public static void SpawnMarkerProjectile(NPC target, Player player, Item item, Vector2 velocity, float rotation)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (target.type == NPCID.TargetDummy) return;
            if (mogPlayer.markerProjOut) return;

            int proj = Projectile.NewProjectile(target.GetSource_FromAI(), target.Center, velocity, ModContent.ProjectileType<MarkerTargetProj>(), (int)(item.damage * 1.45f), 0f, player.whoAmI, rotation); //Might have to change the source if problems arise

            if (proj >= 0)
            {
                Main.projectile[proj].netUpdate = true;
            }

            mogPlayer.markerProjOut = true;

            foreach (NPC other in Main.npc)
            {
                if (other.active && other.TryGetGlobalNPC<MogModGlobalNPC>(out var g))
                {
                    if (g.markedByMarker && other != target)
                    {
                        g.markedByMarker = false;
                    }
                }
            }

            target.GetGlobalNPC<MogModGlobalNPC>().markedByMarker = true;
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
        public void AddItemBlood(NPC npc, Player player, Item item)
        {
            MogGlobalItem globalItem = item.GetGlobalItem<MogGlobalItem>();
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();

            maxBlood = Convert.ToInt32(npc.lifeMax * .05 + npc.defense);

            if (maxBlood < 150)
                maxBlood = 150;

            int bloodToAdd = globalItem.bloodDamage;

            if (mogPlayer.exultationEquipped)
            {
                bloodToAdd = (int)(bloodToAdd * 1.15f);
            }

            if (mogPlayer.mercyBladeEquipped)
                bloodToAdd = (int)(bloodToAdd * 1.2f);

            if (mogPlayer.wearingWhiteArmor)
            {
                bloodToAdd = (int)(bloodToAdd * 1.2f);
            }

            currentBlood += bloodToAdd;

            if (currentBlood >= maxBlood)
            {
                ApplyBleedProc(npc);
            }
        }
        public void AddProjectileBlood(NPC npc, int bloodToAdd)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            currentBlood += bloodToAdd;

            if (currentBlood >= maxBlood)
            {
                ApplyBleedProc(npc);
            }
        }
        public void ApplyBleedProc(NPC npc)
        {
            NPC.HitInfo hitInfo = new NPC.HitInfo
            {
                Damage = Convert.ToInt32(npc.lifeMax * 0.085f) + 50,
                Knockback = 0,
                HitDirection = 0,
                Crit = false,
                DamageType = DamageClass.Generic
            };

            npc.StrikeNPC(hitInfo);
            NetMessage.SendStrikeNPC(npc, hitInfo);

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.BleedProcTextSync);
                packet.WriteVector2(npc.Center);
                packet.Send(-1);
            }
            
            doBloodFX(npc.Center);
            
            currentBlood = 0;
        }
        public static void doBloodFX(Vector2 position)
        {
            Rectangle r = new Rectangle((int)position.X - 10, (int)position.Y - 50, 20, 20);
            CombatText.NewText(r, Color.Red, "Bleed!", true);

            SoundEngine.PlaySound(BloodCrit, position);

            for (int i = 0; i < 80; i++)
            {
                int dust = Dust.NewDust(position, 20, 20, DustID.Blood, 0f, 0f, 0, default, 2f);
                Main.dust[dust].noGravity = false;
            }
        }
        public static void HandleBleedProcText(BinaryReader reader)
        {
            Vector2 position = reader.ReadVector2();

            if (Main.netMode != NetmodeID.Server)
            {
                doBloodFX(position);
            }
        }
        #endregion

        #region On Kill && On Spawn
        public override void OnKill(NPC npc)
        {
            Player player = Main.LocalPlayer;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (npc.type == NPCID.HallowBoss)
                if (!NPC.downedEmpressOfLight)
                {
                    FaeOreText = Mod.GetLocalization($"WorldGen.{nameof(FaeOreText)}");
                    WorldGeneration.SpawnOre(ModContent.TileType<FaeOreT>(), 16E-05, 0.35f, .8f, 7, 12, TileID.Pearlstone, TileID.HallowedIce, TileID.HallowSandstone, TileID.HallowHardenedSand);

                    WorldGeneration.BroadcastLocalizedText(FaeOreText.Value, Color.HotPink);
                    SyncWorld();
                }
            if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyer)
            {
                if (!Condition.DownedMechBossAll.IsMet())
                {
                    HellfireEssenceText = Mod.GetLocalization($"WorldGen.{nameof(HellfireEssenceText)}");
                    WorldGeneration.BroadcastLocalizedText(HellfireEssenceText.Value, Color.Orange);
                    SyncWorld();
                }
            }
            // if an enemy is killed in one shot from a freezing weapon it doesnt shoot out the projectiles
            // i think this is because it doesnt apply the buff before killing them, so it cant run this line <--- Hey Will it's me (Megaslayer), I think we could do this in onhitbyitem, check if the npc's health is below zero (or if damagedealt was greater than the npc's current health) and if they were hit by frozen spear, and if those conditions are true, spawn the projectiles
            if (npc.HasBuff<FreezingDebuff>() || mogPlayer.wearingFrostArmor)
            {
                int numSplits = 6;
                float angleVariance = MathHelper.TwoPi / numSplits;
                Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.ToRadians(45));

                for (int i = 0; i < numSplits; ++i)
                {
                    projVec = projVec.RotatedBy(angleVariance);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, projVec, ProjectileID.Blizzard, 50, 1f, Main.myPlayer);
                }
            }
            if (Condition.DownedMechBossAll.IsMet())
            {
                if (hellEpstein)
                    switch (npc.type)
                    {
                        case NPCID.Hellbat:
                        case NPCID.LavaSlime:
                        case NPCID.FireImp:
                        case NPCID.Demon:
                        case NPCID.VoodooDemon:
                        case NPCID.DemonTaxCollector:
                        case NPCID.Lavabat:
                        case NPCID.RedDevil:
                                var entitySource = npc.GetSource_FromAI();
                                NPC fireball = NPC.NewNPCDirect(entitySource, (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HellfireSpirit>(), npc.whoAmI);
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, number: fireball.whoAmI);
                            break;
                    }
            }
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (Condition.DownedMechBossAll.IsMet())
                if (Main.rand.Next(0, 4) == 0)
                    switch (npc.type)
                    {
                        case NPCID.Hellbat:
                        case NPCID.LavaSlime:
                        case NPCID.FireImp:
                        case NPCID.Demon:
                        case NPCID.VoodooDemon:
                        case NPCID.DemonTaxCollector:
                        case NPCID.Lavabat:
                        case NPCID.RedDevil:
                            hellEpstein = true;
                            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.5f, 3f));
                            npc.life = npc.lifeMax;
                            npc.defDamage = (int)(npc.damage * 1.5f);
                            npc.knockBackResist *= 0.2f;
                            break;
                    }
        }
        #endregion

        #region Debuffs
        // actual debuff effect
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (divineDebuff)
            {
                ApplyDPSDebuff(600, 100, ref npc.lifeRegen, ref damage);
            }
            if (skadiDebuff)
            {
                ApplyDPSDebuff(200, 40, ref npc.lifeRegen, ref damage);
            }
            if (aghDebuff)
            {
                ApplyDPSDebuff(480, 80, ref npc.lifeRegen, ref damage);
            }
            if (wingsOfLightDebuff)
            {
                ApplyDPSDebuff(300, 15, ref npc.lifeRegen, ref damage);
            }
            if (blackBladeDebuff)
            {
                ApplyDPSDebuff(200, 20, ref npc.lifeRegen, ref damage);
            }
            if (ghostflameDebuff)
            {
                ApplyDPSDebuff(170, 7, ref npc.lifeRegen, ref damage);
            }
            if (infernoDebuff)
            {
                ApplyDPSDebuff(500, 50, ref npc.lifeRegen, ref damage);
            }
            //if (jidiDebuff)
            //{
            //    ApplyDPSDebuff(180, 8, ref npc.lifeRegen, ref damage);
            //}
        }

        // movement changes
        public override void PostAI(NPC npc)
        {
            if (skadiDebuff)
            {
                npc.velocity.X *= 0.98f;
                npc.velocity.Y *= 0.98f;
            }
            if (freezingDebuff)
            {
                npc.velocity.X *= 0.97f;
                npc.velocity.Y *= 0.97f;
            }
            if (shivasDebuff)
            {
                npc.velocity.X *= 0.97f;
                npc.velocity.Y *= 0.97f;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                hurtInfo = new Player.HurtInfo
                {
                    Damage = 1,
                    Knockback = 0,
                    HitDirection = 0,
                    Dodgeable = false,
                    SoundDisabled = true
                };

                var hitInfo = new NPC.HitInfo
                {
                    Damage = 20,
                    Knockback = 5,
                    HitDirection = target.direction,
                    Crit = false,
                    DamageType = DamageClass.Generic
                };
                npc.StrikeNPC(hitInfo); //Must use this instead of modifying the npc's life stat
                NetMessage.SendStrikeNPC(npc, hitInfo);
            }

            if (wingsOfLightDebuff)
            {
                npc.damage = (int)(npc.defDamage * .9f);
            }
            else
                npc.damage = npc.defDamage;
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                MogPlayer mogPlayer = target.GetModPlayer<MogPlayer>();
                mogPlayer.doParry(target, target.Center);
                modifiers.Cancel();

                var hitInfo = new NPC.HitInfo
                {
                    Damage = 20,
                    Knockback = 5,
                    HitDirection = target.direction,
                    Crit = false,
                    DamageType = DamageClass.Generic
                };
                npc.StrikeNPC(hitInfo); //Must use this instead of modifying the npc's life stat
                NetMessage.SendStrikeNPC(npc, hitInfo);
            }
        }

        // lower defense from buffs (taken from example mod)
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (skadiDebuff)
            {
                modifiers.Defense *= skadiMult;
            }
            if (jidiDebuff)
            {
                modifiers.Defense.Flat -= jidiNumb;
            }
            if (shivasDebuff)
            {
                modifiers.Defense *= shivaMult;
            }
            if (wingsOfLightDebuff)
            {
                modifiers.CritDamage *= 1.1f;
            }
            if (aghDebuff)
            {
                modifiers.CritDamage *= 1.2f;
            }
        }

        // debuff visual effects
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (divineDebuff)
            {
                DivineMightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.NavajoWhite;
            }
            if (skadiDebuff)
            {
                EyeOfSkadiDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.DarkSlateBlue;
            }
            if (freezingDebuff)
            {
                FreezingDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.LightBlue;
            }
            if (aghDebuff)
            {
                AghanimHexDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.BlueViolet;
            }
            if (wingsOfLightDebuff)
            {
                WingsOfLightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.LightGoldenrodYellow;
            }
            if (blackBladeDebuff)
            {
                BlackBladeDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.DarkRed;
            }
            if (ghostflameDebuff)
            {
                GhostflameDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.WhiteSmoke;
            }
            if (jidiDebuff)
            {
                JidiPollenBagDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.LimeGreen;
            }
            if (markedByMarker) //TODO: Give this a custom effect
            {
                WingsOfLightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.Gold;
            }
            if (shivasDebuff)
            {
                ShivasEnemyDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.LightSkyBlue;
            }
            if (infernoDebuff)
            {
                InfernoDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.OrangeRed;
            }
        }

        // debuff damage (how often it applies damage and how much damage is dealt)
        public void ApplyDPSDebuff(int lifeRegenValue, int damageValue, ref int lifeRegen, ref int damage)
        {
            if (lifeRegen > 0)
                lifeRegen = 0;

            lifeRegen -= lifeRegenValue;

            if (damage < damageValue)
                damage = damageValue;
        }
        public override void ResetEffects(NPC npc)
        {
            divineDebuff = false;
            skadiDebuff = false;
            freezingDebuff = false;
            aghDebuff = false;
            wingsOfLightDebuff = false;
            ghostflameDebuff = false;
            jidiDebuff = false;
            blackBladeDebuff = false;
            shivasDebuff = false;
            infernoDebuff = false;
        }
        #endregion
    }
}
