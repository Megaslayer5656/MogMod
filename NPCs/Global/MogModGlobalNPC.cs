using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Items.Ammo.SorcerySpells.Carian;
using MogMod.Items.Ammo.SorcerySpells.Death;
using MogMod.Items.Ammo.SorcerySpells.Glintstone;
using MogMod.Items.Armor.Hellfire;
using MogMod.Items.Armor.WhiteMaskSet;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Pets;
using MogMod.Items.Placeable.Ores;
using MogMod.Items.Tools;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using MogMod.NPCs.Enemies;
using MogMod.NPCs.ProjectileEnemies;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.EnemyProjectiles;
using MogMod.Projectiles.Melee;
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
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static AssGen.Assets;
using static MogMod.Common.Systems.MogModNetcode;
using static System.Net.Mime.MediaTypeNames;

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
        public bool blazingDebuff;
        public bool toxicDebuff;
        public bool healingDisabledDebuff;

        public NPC.HitInfo hitInfo;
        public int maxBlood = 150;
        public int currentBlood = 0;

        public int toxicDamage = 0;

        // debuff stat changes
        public const int skadiNumb = 25;
        public static float skadiMult = 1 - skadiNumb / 100f;
        public const int jidiNumb = 20;
        public const int shivaNumb = 15;
        public static float shivaMult = 1 - shivaNumb / 100f;

        // damage caps
        public const int bashCap = GiantsMaul.DamageCap;
        public const int shivCap = SerratedShiv.DamageCap;
        public const int hellfireCap = HellfireMask.DamageCap;

        public bool markedByMarker;

        // procs
        public bool bashProc = false;
        public bool shivProc = false;

        Random rand = new Random();

        public int cooldownTimer = 5;
        public int currentCoins = 0;
        public int maxCoins = 5;

        public int overloadingRegenCooldown = OverloadingAspect.EnemyRegenWaitTime;
        public override bool InstancePerEntity => true;

        public static readonly SoundStyle BloodSFX = new($"{nameof(MogMod)}/Sounds/SE/BloodCrit")
        {
            Volume = .7f,
            PitchVariance = .2f,
            MaxInstances = 3
        };
        public static readonly SoundStyle BashSFX = new($"{nameof(MogMod)}/Sounds/SE/SkullBash")
        {
            Volume = 1.3f,
            PitchVariance = .2f,
            MaxInstances = 3
        };
        public static readonly SoundStyle UltraCritSFX = new($"{nameof(MogMod)}/Sounds/SE/UltraCrit")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 3
        };

        // elites
        public bool hellEpstein = false;
        public bool overloadingElite = false;
        public static int overloadingOwner = -1;
        public bool fireElite = false;
        public bool goldElite = false;
        public bool healingElite = false;
        public bool toxicElite = false;
        public static LocalizedText FaeOreText { get; private set; }
        public static LocalizedText HellfireEssenceText { get; private set; }
        public override void SetStaticDefaults()
        {
            FaeOreText = Mod.GetLocalization($"WorldGen.{nameof(FaeOreText)}");
            HellfireEssenceText = Mod.GetLocalization($"WorldGen.{nameof(HellfireEssenceText)}");
        }
        public override void SetDefaults(NPC npc)
        {
            ApplyEliteEffects(npc, npc.GetSource_FromThis("parent"));
        }
        #endregion

        #region Shops
        // modifies vanilla npc shop
        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.SkeletonMerchant:
                    shop.InsertAfter(ItemID.Rope, ModContent.ItemType<AstrologersStaff>(), (Condition.MoonPhasesEven));
                    shop.InsertAfter(ItemID.Rope, ModContent.ItemType<LabGerminator>(), (Condition.MoonPhasesOdd));
                    break;
                case NPCID.Merchant:
                    shop.InsertAfter(ItemID.Glowstick, ModContent.ItemType<Crown>(), Condition.DownedEyeOfCthulhu, Condition.HappyEnoughToSellPylons);
                    break;
                case NPCID.Dryad:
                    shop.InsertAfter(ItemID.DirtRod, ModContent.ItemType<ForceStaff>(), Condition.HappyEnoughToSellPylons);
                    break;
                case NPCID.Demolitionist:
                    shop.InsertAfter(ItemID.Grenade, ModContent.ItemType<GasGrenade>(), Condition.HappyEnoughToSellPylons);
                    shop.InsertAfter(ItemID.Grenade, ModContent.ItemType<BloodGrenade>(), Condition.BloodMoon);
                    break;
                case NPCID.Pirate:
                    shop.InsertAfter(ItemID.Sail, ModContent.ItemType<TidalWave>(), Condition.InBeach);
                    break;
            }
        }
        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            if (Main.moonPhase >= 3 && Main.moonPhase <= 5)
                shop[nextSlot++] = ModContent.ItemType<GasGrenade>();
            if (Main.moonPhase % 2 == 0)
                shop[nextSlot++] = ModContent.ItemType<CarianSlicer>();
            if (Main.hardMode && Main.moonPhase % 2 == 1)
                shop[nextSlot++] = ModContent.ItemType<CarianGreatsword>();
        }
        #endregion

        #region NPC Drops
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            int aspectChance = 2000;
            LeadingConditionRule overloadingDrop = new(DropHelper.OverloadingEliteCondition);
            LeadingConditionRule blazingDrop = new(DropHelper.BlazingEliteCondition);
            LeadingConditionRule gildedDrop = new(DropHelper.GildedEliteCondition);
            LeadingConditionRule mendingDrop = new(DropHelper.MendingEliteCondition);
            LeadingConditionRule toxicDrop = new(DropHelper.ToxicEliteCondition);

            overloadingDrop.Add(ModContent.ItemType<OverloadingAspect>(), aspectChance);
            blazingDrop.Add(ModContent.ItemType<BlazingAspect>(), aspectChance);
            gildedDrop.Add(ModContent.ItemType<GildedAspect>(), aspectChance);
            mendingDrop.Add(ModContent.ItemType<MendingAspect>(), aspectChance);
            toxicDrop.Add(ModContent.ItemType<NoxiousAspect>(), aspectChance);

            globalLoot.Add(overloadingDrop);
            globalLoot.Add(blazingDrop);
            globalLoot.Add(gildedDrop);
            globalLoot.Add(mendingDrop);
            globalLoot.Add(toxicDrop);

            globalLoot.Add(new CommonDrop(ModContent.ItemType<LedX>(), 10000, 1, 1, 1));
            globalLoot.Add(new CommonDrop(ModContent.ItemType<RedX>(), 100000, 1, 1, 1));

        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            #region Setup
            LeadingConditionRule postEye = npcLoot.DefineConditionalDropSet(DropHelper.PostEye());
            LeadingConditionRule postSkele = npcLoot.DefineConditionalDropSet(DropHelper.PostSkele());
            LeadingConditionRule postOneMech = npcLoot.DefineConditionalDropSet(DropHelper.PostOneMech());
            LeadingConditionRule postAllMech = npcLoot.DefineConditionalDropSet(DropHelper.PostAllMech());
            LeadingConditionRule postPlant = npcLoot.DefineConditionalDropSet(DropHelper.PostPlant());
            LeadingConditionRule postFish = npcLoot.DefineConditionalDropSet(DropHelper.PostFish());
            LeadingConditionRule postEoL = npcLoot.DefineConditionalDropSet(DropHelper.PostEoL());
            #endregion
            switch (npc.type)
            {
                #region Surface
                case NPCID.Ghost:
                    postEye.Add(ModContent.ItemType<SpiritShard>(), 3, 1, 3);
                    break;
                case NPCID.RainbowSlime:
                case NPCID.LightMummy:
                    postEoL.Add(ModContent.ItemType<FaeOre>(), 2, 12, 20);
                    break;
                #endregion
                #region Underground
                case NPCID.Tim:
                    npcLoot.RemoveWhere(rule => true, false);
                    npcLoot.Add(ItemDropRule.OneFromOptions(1, ItemID.WizardHat, ModContent.ItemType<GlintstoneArc>()));
                    break;
                case NPCID.CrimsonAxe:
                case NPCID.CursedHammer:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExplosiveGhostflame>(), 15, 1, 1));
                    break;
                case NPCID.Salamander:
                case NPCID.Salamander2:
                case NPCID.Salamander3:
                case NPCID.Salamander4:
                case NPCID.Salamander5:
                case NPCID.Salamander6:
                case NPCID.Salamander7:
                case NPCID.Salamander8:
                case NPCID.Salamander9:
                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LabGerminator>(), 50));
                    break;
                case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                case NPCID.PigronHallow:
                    postFish.Add(ModContent.ItemType<BrinyRind>(), 4, 3, 5);
                    break;
                case NPCID.BloodCrawler:
                case NPCID.BloodCrawlerWall:
                case NPCID.LihzahrdCrawler:
                case NPCID.WallCreeper:
                case NPCID.WallCreeperWall:
                case NPCID.BlackRecluse:
                case NPCID.BlackRecluseWall:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<DaturaLash>(), 20, 1, 1));
                    break;
                #endregion
                #region Ocean
                case NPCID.Shark:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 8, 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OceanHeart>(), 100, 1, 1));
                    postFish.Add(ModContent.ItemType<BrinyRind>(), 4, 3, 5);
                    break;
                #endregion
                #region Dungeon Enemies
                case NPCID.CursedSkull:
                case NPCID.GiantCursedSkull:
                    postSkele.Add(ModContent.ItemType<FiasMist>(), 20, 1, 1);
                    break;
                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlinkDagger>(), 20, 1, 1));
                    postPlant.Add(ModContent.ItemType<RingsOfSpectralLight>(), 20, 1, 1);
                    break;
                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                    postPlant.Add(ModContent.ItemType<RingsOfSpectralLight>(), 20, 1, 1);
                    break;
                #endregion
                #region Goblins
                case NPCID.GoblinSorcerer:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SearingSignet>(), 20));
                    break;
                case NPCID.GoblinSummoner:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SearingSignet>(), 5));
                    break;
                #endregion
                #region Frost Moon
                // low level goons
                case NPCID.GingerbreadMan:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gingy>(), 50, 1, 1));
                    postPlant.Add(ModContent.ItemType<FrostEssence>(), 5, 1, 3);
                    break;
                case NPCID.PresentMimic:
                case NPCID.Flocko:
                case NPCID.ZombieElf:
                case NPCID.ZombieElfBeard:
                case NPCID.ZombieElfGirl:
                case NPCID.ElfArcher:
                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.ElfCopter:
                    postPlant.Add(ModContent.ItemType<FrostEssence>(), 5, 1, 3);
                    break;
                // high level goons
                case NPCID.Yeti:
                case NPCID.Krampus:
                    postPlant.Add(ModContent.ItemType<FrostEssence>(), 3, 2, 4);
                    break;
                // bosses
                case NPCID.Everscream:
                case NPCID.SantaNK1:
                case NPCID.IceQueen:
                    postPlant.Add(ModContent.ItemType<FrostEssence>(), 1, 3, 5);
                    break;
                #endregion
                #region Pumpkin Moon
                // low level goons
                case NPCID.Scarecrow1:
                case NPCID.Scarecrow2:
                case NPCID.Scarecrow3:
                case NPCID.Scarecrow4:
                case NPCID.Scarecrow5:
                case NPCID.Scarecrow6:
                case NPCID.Scarecrow7:
                case NPCID.Scarecrow8:
                case NPCID.Scarecrow9:
                case NPCID.Scarecrow10:
                case NPCID.Splinterling:
                case NPCID.Hellhound:
                case NPCID.Poltergeist:
                    postPlant.Add(ModContent.ItemType<SpookyEssence>(), 1, 3, 5);
                    break;
                // high level goons
                case NPCID.HeadlessHorseman:
                    postPlant.Add(ModContent.ItemType<SpookyEssence>(), 1, 3, 5);
                    break;
                // bosses
                case NPCID.MourningWood:
                case NPCID.Pumpking:
                    postPlant.Add(ModContent.ItemType<SpookyEssence>(), 1, 3, 5);
                    break;
                #endregion
                #region Bosses
                case NPCID.Golem:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LizhardBloodVial>(), 1, 1, 2));
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BrinyRind>(), 1, 7, 14));
                    break;
                #endregion
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
            Vector2 dustNumb = new(1.6f, 2f);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                /*
                if (hellEpstein)
                {
                    npc.netUpdate = true;
                }
                */
                if (overloadingElite)
                {
                    overloadingRegenCooldown--;
                    //npc.netUpdate = true;
                }
                /*
                if (fireElite)
                {
                    npc.netUpdate = true;
                }
                if (goldElite)
                {
                    npc.reflectsProjectiles = true;
                    npc.netUpdate = true;
                }
                if (healingElite)
                {
                    npc.netUpdate = true;
                }
                if (toxicElite)
                {
                    npc.netUpdate = true;
                }
                */
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            #region Setup
            MogGlobalItem globalItem = item.GetGlobalItem<MogGlobalItem>();
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (item.MogMod().bloodDamage > 0)
                {
                    // Tell server that this NPC was hit with this item
                    //Main.NewText($"calling multiplayer client blood", Color.IndianRed);
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.AddBloodFromItem);
                    packet.Write(npc.whoAmI);
                    packet.Write(player.whoAmI);
                    packet.Write(item.type);
                    packet.Send();
                }
                if (mogPlayer.wearingToxic)
                {
                    //Main.NewText($"calling multiplayer client toxic", Color.MediumPurple);
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.AddToxicFromItem);
                    packet.Write(npc.whoAmI);
                    packet.Write(player.whoAmI);
                    packet.Send();
                }
            }
            else
            {
                if (item.MogMod().bloodDamage > 0)
                {
                    //Main.NewText($"calling server blood", Color.LightGoldenrodYellow);
                    AddItemBlood(npc, player, item);
                }
                if (mogPlayer.wearingToxic)
                {
                    //Main.NewText($"calling server toxic", Color.Purple);
                    AddItemToxic(player);
                }
            }
            int itemDamage = player.HeldItem.damage;
            int enemyMaxHP = npc.lifeMax;
            int hellfireDamage = MogModUtils.DamageSoftCap(damageDone * HellfireMask.DamageMult, hellfireCap);
            int overloadingDamage = (int)(damageDone * OverloadingAspect.DamageMult);
            var source = player.GetSource_OnHit(npc);
            bashProc = rand.Next(7) == 0;
            shivProc = rand.Next(5) == 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
                overloadingRegenCooldown = OverloadingAspect.EnemyRegenWaitTime;
            #endregion

            #region Weapon Effects
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
            #endregion

            #region Accessory Effects
            // skull basher
            if (bashProc && mogPlayer.wearingGiantsMaul && mogPlayer.bashCooldown <= 0)
            {
                mogPlayer.bashCooldown = cooldownTimer;
                ApplyBashProc(npc, player, damageDone);
            }

            // serrated shiv
            if (shivProc && mogPlayer.wearingSerratedShiv && mogPlayer.shivCooldown <= 0)
            {
                mogPlayer.shivCooldown = cooldownTimer;
                ApplyTrueStrikeProc(npc, player);
            }

            // atg and plasma shrimp
            if (mogPlayer.atgActive || mogPlayer.plasmaActive)
            {
                mogPlayer.doATG(damageDone);
            }

            // polylute
            if (mogPlayer.polyluteActive)
            {
                Vector2 kirk = new Vector2(-10, 10).RotatedByRandom(MathHelper.ToRadians(360));
                int procChance = rand.Next(1, 6);

                if (procChance == 5)
                    Projectile.NewProjectile(source, npc.Center, kirk, ModContent.ProjectileType<PolyluteProj>(), Convert.ToInt32(damageDone * .3f) + 1, 3, player.whoAmI);
            }

            if (mogPlayer.wearingOverloading && mogPlayer.overloadingCooldown <= 0)
            {
                mogPlayer.overloadingCooldown = cooldownTimer;
                Projectile orb = Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<OverloadingOrbProj>(), overloadingDamage, 0f, player.whoAmI, ai2: 1f);
                orb.DamageType = player.HeldItem.DamageType;
            }
            if (Main.rand.NextBool(3) && mogPlayer.wearingGilded && mogPlayer.gildedCoinDropCooldown <= 0 && !npc.SpawnedFromStatue && npc.type != NPCID.TargetDummy && !npc.boss && currentCoins <= maxCoins)
            {
                mogPlayer.gildedCoinDropCooldown = cooldownTimer;
                currentCoins++;
                Rectangle npcBox = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int coinChance = Main.rand.Next(0, 101);
                int coin = ItemID.CopperCoin;
                switch (coinChance)
                {
                    case < 60:
                        break;
                    case < 90:
                        coin = ItemID.SilverCoin;
                        break;
                    case < 100:
                        coin = ItemID.GoldCoin;
                        break;
                    case >= 100:
                        coin = ItemID.PlatinumCoin;
                        break;
                }
                Item.NewItem(source, npcBox, coin);
            }
            #endregion

            #region Armor Effects
            // hellfire armor
            if (mogPlayer.wearingHellfireArmor && mogPlayer.hellfireCooldown <= 0)
            {
                if (Main.zenithWorld)
                {
                    if (damageDone <= 100)
                    {
                        mogPlayer.hellfireCooldown = cooldownTimer * 72;
                        int hellfire = Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), (int)(hellfireDamage * 0.1f), 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                    }
                }
                if (damageDone >= 100)
                {
                    mogPlayer.hellfireCooldown = cooldownTimer * 72;
                    int hellfire = Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), hellfireDamage, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                }
            }
            #endregion
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            #region Setup
            Player player = Main.player[projectile.owner];
            MogPlayer mogPlayer = player.MogMod();
            var source = player.GetSource_OnHit(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                overloadingRegenCooldown = OverloadingAspect.EnemyRegenWaitTime;
            if (Main.rand.NextBool(3) && mogPlayer.wearingGilded && mogPlayer.gildedCoinDropCooldown <= 0 && !npc.SpawnedFromStatue && npc.type != NPCID.TargetDummy && currentCoins <= maxCoins)
            {
                mogPlayer.gildedCoinDropCooldown = cooldownTimer;
                currentCoins++;
                Rectangle npcBox = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int coinChance = Main.rand.Next(0, 101);
                int coin = ItemID.CopperCoin;
                switch (coinChance)
                {
                    case < 60:
                        break;
                    case < 90:
                        coin = ItemID.SilverCoin;
                        break;
                    case < 100:
                        coin = ItemID.GoldCoin;
                        break;
                    case >= 100:
                        coin = ItemID.PlatinumCoin;
                        break;
                }
                Item.NewItem(source, npcBox, coin);
            }

            //Main.NewText($"proj blood damage = {projectile.MogMod().bloodDamage}", Color.Red);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (projectile.MogMod().bloodDamage > 0)
                {
                    ModPacket packet = Mod.GetPacket();
                    //Main.NewText($"calling multiplayer client proj blood", Color.IndianRed);
                    packet.Write((byte)MogModMessageType.AddBloodFromProjectile);
                    packet.Write(npc.whoAmI);
                    packet.Write(player.whoAmI);
                    packet.Write(projectile.MogMod().bloodDamage);
                    packet.Send();
                }
                if (mogPlayer.wearingToxic)
                {
                    ModPacket packet = Mod.GetPacket();
                    //Main.NewText($"calling multiplayer client proj toxic", Color.MediumPurple);
                    packet.Write((byte)MogModMessageType.AddToxicFromProjectile);
                    packet.Write(npc.whoAmI);
                    packet.Write(player.whoAmI);
                    packet.Send();
                }
                return;
            }
            if (projectile.MogMod().bloodDamage > 0)
            {
                //Main.NewText($"calling server proj blood", Color.Red);
                AddProjectileBlood(npc, player, projectile.MogMod().bloodDamage);
            }
            if (mogPlayer.wearingToxic)
            {
                //Main.NewText($"calling server proj toxic", Color.Purple);
                AddProjectileToxic(player);
            }
            #endregion
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
        #region Effects
        #region Blood
        public void AddItemBlood(NPC npc, Player player, Item item)
        {
            MogGlobalItem globalItem = item.MogMod();

            maxBlood = (int)(npc.lifeMax * .05 + npc.defense);

            if (maxBlood < 150)
                maxBlood = 150;
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"second item max blood is {maxBlood}"), Color.GhostWhite, player.whoAmI);

            int bloodToAdd = globalItem.bloodDamage;

            currentBlood += BloodEquipEffects(player, bloodToAdd);
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"current item blood is {currentBlood}"), Color.IndianRed, player.whoAmI);

            if (currentBlood >= maxBlood)
            {
                ApplyBleedProc(npc, player);
            }
        }
        public void AddProjectileBlood(NPC npc, Player player, int blood)
        {
            maxBlood = (int)(npc.lifeMax * .05 + npc.defense);

            if (maxBlood < 150)
                maxBlood = 150;
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"second proj max blood is {maxBlood}"), Color.GhostWhite, player.whoAmI);

            currentBlood += BloodEquipEffects(player, blood);
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"current proj blood is {currentBlood}"), Color.IndianRed, player.whoAmI);

            if (currentBlood >= maxBlood)
            {
                ApplyBleedProc(npc, player);
            }
        }
        public int BloodEquipEffects(Player player, int blood)
        {
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"adding {blood} blood damage"), Color.LimeGreen, player.whoAmI);
            MogPlayer mogPlayer = player.MogMod();
            if (mogPlayer.exultationEquipped)
                blood = (int)(blood * LordOfBloodsExultation.BloodMult);

            if (mogPlayer.mercyBladeEquipped)
                blood = (int)(blood * BladeOfMercy.BloodMult);

            if (mogPlayer.wearingWhiteArmor)
                blood = (int)(blood * (WhiteMask.BloodMult + 1));

            if (mogPlayer.wearingFlayersBota)
                blood = (int)(blood * (FlayersBota.BloodMult + 1));
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"added {blood} blood damage"), Color.Lime, player.whoAmI);
            return blood;
        }

        public void ApplyBleedProc(NPC npc, Player player)
        {
            MogPlayer mogPlayer = player.MogMod();
            NPC.HitInfo hitInfo = new()
            {
                Damage = Convert.ToInt32(npc.lifeMax * 0.085f) + 50,
                Knockback = 0,
                HitDirection = 0,
                Crit = false,
                DamageType = DamageClass.Generic
            };

            npc.StrikeNPC(hitInfo);
            NetMessage.SendStrikeNPC(npc, hitInfo);

            // TODO: fix this not working in multiplayer
            if (mogPlayer.wearingFlayersBota && npc.type != NPCID.TargetDummy)
            {
                //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"going to heal {player.name}, mogName = {mogPlayer.Name}"), Color.Green, player.whoAmI);
                int heal = (int)(hitInfo.Damage / 100) + 1;
                player.HealLifestealMult(heal);
                //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"healed {player.name} for {heal}"), Color.Green, player.whoAmI);
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.BleedProcTextSync);
                packet.Write(npc.whoAmI);
                packet.Write(hitInfo.Damage);
                packet.Send(-1);
            }
            else
            {
                BloodFX(npc, hitInfo.Damage);
            }
            currentBlood = 0;
        }
        public void BloodFX(NPC npc, int damage)
        {
            //Main.NewText("called blood FX");
            Rectangle r = new((int)npc.Hitbox.X, (int)npc.Hitbox.Y - 50, npc.Hitbox.Width, npc.Hitbox.Height);
            CombatText.NewText(r with { Y = npc.Hitbox.Y }, Color.Red, damage, true);
            CombatText.NewText(r, Color.Red, MiscUtils.GetText("Status.Proc.Bleed").ToString(), true);

            SoundEngine.PlaySound(BloodSFX, npc.Center);

            for (int i = 0; i < 80; i++)
            {
                int dust = Dust.NewDust(npc.Center, r.Width, r.Height, DustID.Blood, 0f, 0f, 0, default, 2f);
                Main.dust[dust].noGravity = false;
            }
        }
        #endregion
        #region Toxic
        public void AddItemToxic(Player player)
        {
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"adding: {toxicDamage} to item"), Color.Magenta, player.whoAmI);
            MogPlayer mogPlayer = player.MogMod();
            if (mogPlayer.wearingToxic && mogPlayer.toxicCooldown <= 0)
            {
                mogPlayer.toxicCooldown = 2;
                toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin - 6, NoxiousAspect.DamageMax - 20);
                if (Main.hardMode)
                    toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin, NoxiousAspect.DamageMax);
                //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"sd phonk item damage is: {toxicDamage}"), Color.MediumPurple, player.whoAmI);
            }
        }
        public void AddProjectileToxic(Player player)
        {
            //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"adding: {toxicDamage} to proj"), Color.DarkMagenta, player.whoAmI);
            MogPlayer mogPlayer = player.MogMod();
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (mogPlayer.wearingToxic && mogPlayer.toxicCooldown <= 0)
            {
                mogPlayer.toxicCooldown = 2;
                toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin - 6, NoxiousAspect.DamageMax - 20);
                if (Main.hardMode)
                    toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin, NoxiousAspect.DamageMax);
                //ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"sd phonk proj damage is: {toxicDamage}"), Color.Purple, player.whoAmI);
            }
        }
        public void ToxicFX(NPC npc)
        {
            //Main.NewText("called toxic FX");
            Rectangle r = new((int)npc.Hitbox.X, (int)npc.Hitbox.Y - 50, npc.Hitbox.Width, npc.Hitbox.Height);
            CombatText.NewText(r with { Y = npc.Hitbox.Y }, Color.Purple, npc.MogMod().toxicDamage, true);
            CombatText.NewText(r, Color.Purple, MiscUtils.GetText("Status.Proc.Toxic").ToString(), true);

            for (int i = 0; i < 15; i++)
            {
                float scale = Main.rand.NextFloat(0.5f, 1f);
                if (Main.rand.NextBool(5))
                    scale *= 1.4f;
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                Dust d = Dust.NewDustPerfect(npc.Center, Main.rand.NextBool(3) ? DustID.Venom : DustID.Poisoned, velocity, 100, default, scale);
                d.fadeIn += 1.2f;
                d.noGravity = true;
                if (Main.rand.NextBool(4))
                    d.noGravity = false;
            }
        }
        #endregion
        #region True Strike
        public void ApplyTrueStrikeProc(NPC npc, Player player)
        {
            MogPlayer mogPlayer = player.MogMod();
            NPC.HitInfo hitInfo = new()
            {
                Damage = MogModUtils.DamageHardCap(Convert.ToInt32(npc.lifeMax * 0.005) + 50, shivCap),
                Knockback = 0,
                HitDirection = 0,
                Crit = false,
                DamageType = DamageClass.Default
            };

            npc.StrikeNPC(hitInfo);
            NetMessage.SendStrikeNPC(npc, hitInfo);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.TrueStrikeProcTextSync);
                packet.Write(npc.lastInteraction);
                packet.WriteVector2(npc.Center);
                packet.Send();
            }
            else
            {
                TrueStrikeFX(npc.Center);
            }
        }
        public static void TrueStrikeFX(Vector2 position)
        {
            //Main.NewText("called true strike FX");
            Rectangle r = new((int)position.X - 10, (int)position.Y - 50, 20, 20);
            Color textColor = new(210, 180, 140);
            CombatText.NewText(r, textColor, MiscUtils.GetText("Status.Proc.Strike").ToString(), true);
            SoundEngine.PlaySound(SoundID.NPCDeath56, position);
            for (int i = 0; i < 40; i++)
            {
                int strike = Dust.NewDust(position, 20, 20, DustID.CopperCoin, 0, 0, 100, default, 2f);
                Main.dust[strike].velocity.Y *= 1.05f;
                Main.dust[strike].noGravity = true;
            }
        }
        #endregion
        #region Bash
        public void ApplyBashProc(NPC npc, Player player, int damage)
        {
            MogPlayer mogPlayer = player.MogMod();
            NPC.HitInfo hitInfo = new()
            {
                Damage = MogModUtils.DamageSoftCap(damage * GiantsMaul.DamageMult, bashCap),
                Knockback = 0,
                HitDirection = 0,
                Crit = false,
                DamageType = DamageClass.MeleeNoSpeed
            };

            npc.StrikeNPC(hitInfo);
            NetMessage.SendStrikeNPC(npc, hitInfo);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.BashProcTextSync);
                packet.Write(npc.lastInteraction);
                packet.Write(npc.whoAmI);
                packet.Send();
            }
            else
            {
                BashFX(npc);
            }
        }
        /// <summary>my lazy ass didnt want to copy and paste this everywhere</summary>
        /// <param name="npc">tagila</param>
        public void BashFX(NPC npc)
        {
            //Main.NewText("called bash FX");
            SoundEngine.PlaySound(BashSFX, npc.Center);
            Rectangle r = new((int)npc.Hitbox.X, (int)npc.Hitbox.Y - 50, npc.Hitbox.Width, npc.Hitbox.Height);
            Color textColor = new(255, 0, 100);
            CombatText.NewText(r, textColor, MiscUtils.GetText("Status.Proc.Bash").ToString(), true);
            for (int n = 0; n < 40; n++)
            {
                float scale = Main.rand.NextFloat(1.1f, 1.4f);
                var color = Main.rand.NextBool() ? Color.DarkRed : Color.Red;
                if (Main.rand.NextBool(5))
                    scale *= 1.4f;
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                Dust bash = Dust.NewDustPerfect(npc.Center, ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, velocity, 100, color, scale);
                bash.fadeIn += 1.2f;
                bash.velocity.Y *= 0.98f;
                bash.noGravity = true;
                if (Main.rand.NextBool(4))
                    bash.noGravity = false;
            }
        }
        #endregion
        #region Ultra Crit
        /// <summary>my lazy ass didnt want to copy and paste this everywhere</summary>
        /// <param name="npc">tigz</param>
        public void UltraCritFX(NPC npc)
        {
            //Main.NewText("called ultra crit FX");
            SoundEngine.PlaySound(UltraCritSFX, npc.Center);
            Rectangle r = new((int)npc.Hitbox.X, (int)npc.Hitbox.Y - 50, npc.Hitbox.Width, npc.Hitbox.Height);
            Color textColor = new(255, 0, 0);
            CombatText.NewText(r, textColor, MiscUtils.GetText("Status.Proc.UltraCrit").ToString(), true);
            for (int i = 0; i < 30; i++)
            {
                Vector2 randPos = Main.rand.NextVector2CircularEdge(r.Width, r.Height);
                Dust telegraphDust = Dust.NewDustPerfect(npc.Center + randPos, ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, npc.DirectionFrom(npc.Center + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                telegraphDust.noGravity = true;
                telegraphDust.fadeIn = 0.6f;
            }
            for (int n = 0; n < 6; n++)
            {
                float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                Vector2 swirlPos = npc.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 20f;
                Vector2 swirlVelocity = Vector2.Normalize(swirlPos - npc.Center).RotatedBy(MathHelper.ToRadians(20)) * 2f;
                Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.GemRuby, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                swirlDust.noGravity = true;
                swirlDust.fadeIn = 0.6f;
            }
        }
        #endregion
        #endregion
        #endregion

        #region On Kill && On Spawn
        public override void OnKill(NPC npc)
        {
            Player player = Main.LocalPlayer;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            void MechLoot()
            {
                // downed one mech
                if (!NPC.downedMechBossAny)
                {

                }
                // downed two mechs
                else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                {

                }
                // downed three mechs
                else
                {
                    HellfireEssenceText = Mod.GetLocalization($"WorldGen.{nameof(HellfireEssenceText)}");
                    WorldGeneration.BroadcastLocalizedText(HellfireEssenceText.Value, Color.Orange);

                    SyncWorld();
                }
            }
            switch (npc.type)
            {
                #region Bosses
                case NPCID.HallowBoss:
                    if (!NPC.downedEmpressOfLight)
                    {
                        FaeOreText = Mod.GetLocalization($"WorldGen.{nameof(FaeOreText)}");
                        WorldGeneration.SpawnOre(ModContent.TileType<FaeOreT>(), 16E-05, 0.35f, .8f, 7, 12, TileID.Pearlstone, TileID.HallowedIce, TileID.HallowSandstone, TileID.HallowHardenedSand);

                        WorldGeneration.BroadcastLocalizedText(FaeOreText.Value, Color.HotPink);
                        SyncWorld();
                    }
                    break;
                case NPCID.TheDestroyer:
                    if (!NPC.downedMechBoss1) MechLoot();
                    break;
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    if (!NPC.downedMechBoss2) MechLoot();
                    break;
                case NPCID.SkeletronPrime:
                    if (!NPC.downedMechBoss3) MechLoot();
                    break;
                #endregion

                #region Enemies
                case NPCID.Hellbat:
                case NPCID.LavaSlime:
                case NPCID.FireImp:
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                case NPCID.DemonTaxCollector:
                case NPCID.Lavabat:
                case NPCID.RedDevil:
                    if (!hellEpstein || !Condition.DownedMechBossAll.IsMet())
                        break;
                    var entitySource = npc.GetSource_FromAI();
                    NPC fireball = NPC.NewNPCDirect(entitySource, (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HellfireSpirit>(), npc.whoAmI);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: fireball.whoAmI);
                    break;
                #endregion
            }
            // if an enemy is killed in one shot from a freezing weapon it doesnt shoot out the projectiles
            // i think this is because it doesnt apply the buff before killing them, so it cant run this line <--- Hey Will it's me (Megaslayer), I think we could do this in onhitbyitem, check if the npc's health is below zero (or if damagedealt was greater than the npc's current health) and if they were hit by frozen spear, and if those conditions are true, spawn the projectiles
            if (npc.HasBuff<FreezingDebuff>() || mogPlayer.wearingFrostArmor)
            {
                int numSplits = Main.hardMode ? 10 : 6; // might cause lag
                //int rotation = Main.hardMode ? 27 : 45;
                int damage = Main.hardMode ? 150 : 50;
                float angleVariance = MathHelper.TwoPi / numSplits;
                Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.ToRadians(45));

                for (int i = 0; i < numSplits; ++i)
                {
                    projVec = projVec.RotatedBy(angleVariance);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, projVec, ProjectileID.Blizzard, damage, 1f, Main.myPlayer);
                }
            }
            if (fireElite)
            {
                if (NPCID.Sets.ProjectileNPC[npc.type] == true)
                    return;
                int damage = (int)(Main.masterMode ? npc.damage * 0.03f : Main.expertMode ? npc.damage * 0.03f : npc.damage * 0.04f);
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BlazingExplosion>(), damage, 1f, Main.myPlayer);
            }
            if (goldElite && Main.rand.NextBool(4))
            {
                if (NPCID.Sets.ProjectileNPC[npc.type] == true)
                    return;
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ProjectileID.CoinPortal, 0, 1f, Main.myPlayer);
            }
            int mendingOrb = ModContent.NPCType<HealingOrb>();
            if (healingElite)
            {
                if (NPCID.Sets.ProjectileNPC[npc.type] == true)
                    return;
                var entitySource = npc.GetSource_FromAI();
                NPC healingOrb = NPC.NewNPCDirect(entitySource, (int)npc.Center.X, (int)npc.Center.Y, mendingOrb, npc.whoAmI);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: healingOrb.whoAmI);
            }
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
        }
         //TODO: fix enemy life changes not syncing in multiplayer
        private void ApplyEliteEffects(NPC npc, IEntitySource source)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            // hellfire enemy spawning
            if (Main.rand.NextBool(4))
            {
                if (Condition.DownedMechBossAll.IsMet())
                {
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
                            MakeHellEpstein(npc);
                            return;
                    }
                }
            }
            // elite enemy spawning
            // TODO: fix bosses (like destroyer) spawning elites
            // consists of enemies children becoming elites, doesnt work for now since there is no source parent
            if (source is EntitySource_Parent { Entity: NPC parent })
            {
                if (parent.boss || parent.type is NPCID.TheDestroyerBody || parent.type is NPCID.TheDestroyerTail)
                    return;
                if (parent.MogMod().overloadingElite)
                {
                    npc.value *= 1.5f; // all elites drop extra money
                    MakeOverloading(npc);
                    return;
                }
                if (parent.MogMod().fireElite)
                {
                    npc.value *= 1.5f; // all elites drop extra money
                    MakeBlazing(npc);
                    return;
                }
                if (parent.MogMod().goldElite)
                {
                    npc.value *= 1.5f; // all elites drop extra money
                    MakeGilded(npc);
                    return;
                }
                if (parent.MogMod().healingElite && npc.type != ModContent.NPCType<HealingOrb>())
                {
                    npc.value *= 1.5f; // all elites drop extra money
                    MakeMending(npc);
                    return;
                }
                if (parent.MogMod().toxicElite)
                {
                    npc.value *= 1.5f; // all elites drop extra money
                    MakeToxic(npc);
                    return;
                }
            }
            int chance = Main.zenithWorld ? 3 : 10;
            if (Main.rand.NextBool(chance) && MogServerConfig.Instance.EliteEnemySpawning)
            {
                if (npc.friendly || npc.CountsAsACritter || npc.boss || npc.dontCountMe || NPCID.Sets.ProjectileNPC[npc.type] || NPCID.Sets.ShouldBeCountedAsBoss[npc.type] || npc.type == NPCID.TargetDummy)
                    return;
                npc.value *= 1.5f; // all elites drop extra money
                switch (Main.rand.Next(0, 5))
                {
                    // overloading (spawn sticky orb, double health)
                    case 0:
                        MakeOverloading(npc);
                        return;
                    // fire (ignite, explode on kill)
                    case 1:
                        MakeBlazing(npc);
                        return;
                    // gold (reflect proj, drop more gold)
                    case 2:
                        MakeGilded(npc);
                        return;
                    // mending (heal other enemies, health regen)
                    case 3:
                        MakeMending(npc);
                        return;
                    // poison (stacking poison debuff that kirks you when it ends (think SD kid phonk))
                    case 4:
                        MakeToxic(npc);
                        break;
                }
            }
        }
        public void MakeHellEpstein(NPC npc)
        {
            hellEpstein = true;

            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.5f, 3f));
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * 1.5f);
            npc.defDamage = npc.damage;
            npc.knockBackResist *= 0.2f;

            npc.netUpdate = true;
            npc.netAlways = true;
        }
        public void MakeOverloading(NPC npc)
        {
            overloadingElite = true;

            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(Main.zenithWorld ? 3f : 1.5f, Main.zenithWorld ? 5f : 3f));
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * 1.3f);
            npc.defDamage = npc.damage;
            npc.defense = (int)(npc.defense * 1.4f);
            npc.defDefense = npc.defense;
            npc.scale *= Main.rand.NextFloat(1.2f, 1.5f);
            npc.knockBackResist *= 0.2f;

            npc.netUpdate = true;
            npc.netAlways = true;
        }
        public void MakeBlazing(NPC npc)
        {
            fireElite = true;

            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.2f, 2f));
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * (Main.zenithWorld ? 3f : 1.8f));
            npc.defDamage = npc.damage;

            npc.netUpdate = true;
            npc.netAlways = true;
        }
        public void MakeGilded(NPC npc)
        {
            goldElite = true;

            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.6f, 2.2f));
            npc.life = npc.lifeMax;
            npc.defense = (int)(npc.defense * 1.6f);
            npc.defDefense = npc.defense;
            npc.value *= Main.zenithWorld ? 15f : 5f;
            npc.MaxFallSpeedMultiplier *= 2f;
            npc.reflectsProjectiles = true;
            npc.chaseable = false;

            npc.netUpdate = true;
            npc.netAlways = true;
        }
        public void MakeMending(NPC npc)
        {
            healingElite = true;

            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(Main.zenithWorld ? 0.2f : 0.5f, Main.zenithWorld ? 0.5f : 0.8f));
            npc.life = npc.lifeMax;
            npc.scale *= Main.rand.NextFloat(Main.zenithWorld ? 0.2f : 0.6f, Main.zenithWorld ? 0.5f : 0.9f);
            npc.defense = (int)(npc.defense * 0.8f);
            npc.defDefense = npc.defense;
            npc.knockBackResist *= 1.5f;

            npc.netUpdate = true;
            npc.netAlways = true;
        }
        public void MakeToxic(NPC npc)
        {
            toxicElite = true;

            npc.damage = (int)(npc.damage * 0.7f);
            npc.defDamage = npc.damage;
            npc.defense = (int)(npc.defense * 0.7f);
            npc.defDefense = npc.defense;

            npc.netUpdate = true;
            npc.netAlways = true;
        }
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(overloadingElite);
            binaryWriter.Write(fireElite);
            binaryWriter.Write(goldElite);
            binaryWriter.Write(healingElite);
            binaryWriter.Write(toxicElite);
            binaryWriter.Write(hellEpstein);

            binaryWriter.Write(currentBlood);
            binaryWriter.Write(toxicDamage);
            binaryWriter.Write(currentCoins);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            overloadingElite = binaryReader.ReadBoolean();
            fireElite = binaryReader.ReadBoolean();
            goldElite = binaryReader.ReadBoolean();
            healingElite = binaryReader.ReadBoolean();
            toxicElite = binaryReader.ReadBoolean();
            hellEpstein = binaryReader.ReadBoolean();

            currentBlood = binaryReader.ReadInt32();
            toxicDamage = binaryReader.ReadInt32();
            currentCoins = binaryReader.ReadInt32();
        }
        public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            if (overloadingElite)
                typeName = MiscUtils.GetText("Prefixes.NPC.Overloading").Format(typeName);
            if (fireElite)
                typeName = MiscUtils.GetText("Prefixes.NPC.Blazing").Format(typeName);
            if (goldElite)
                typeName = MiscUtils.GetText("Prefixes.NPC.Gilded").Format(typeName);
            if (healingElite)
                typeName = MiscUtils.GetText("Prefixes.NPC.Mending").Format(typeName);
            if (toxicElite)
                typeName = MiscUtils.GetText("Prefixes.NPC.Toxic").Format(typeName);
            if (hellEpstein)
                typeName = MiscUtils.GetText("Prefixes.NPC.Hellfire").Format(typeName);
        }
        #endregion

        #region Debuffs
        // actual debuff effect
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (healingElite && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.lifeRegen += Main.zenithWorld ? MendingAspect.LifeRegen * 21 : Main.hardMode ? MendingAspect.LifeRegen * 3 : MendingAspect.LifeRegen;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    int size = 10;
                    Rectangle healingBox = new((int)(npc.Center.X - npc.width * size / 2), (int)(npc.Center.Y - npc.height * size / 2), npc.Hitbox.Width * size, npc.Hitbox.Height * size);
                    if (!target.active || target.friendly || !healingBox.Intersects(target.Hitbox) || 
                        npc.dontCountMe || NPCID.Sets.ProjectileNPC[npc.type] || NPCID.Sets.ShouldBeCountedAsBoss[npc.type] || 
                        npc.type == NPCID.TargetDummy || target.MogMod().healingElite || target.life >= target.lifeMax || !Main.hardMode || healingDisabledDebuff)
                        continue;
                    int heal = Main.zenithWorld ? 3 : 1;
                    target.life += heal;
                    target.HealEffect(heal);
                    if (target.life > target.lifeMax)
                        target.life = target.lifeMax;
                }
            }
            if (overloadingRegenCooldown <= 0)
                npc.lifeRegen += Main.zenithWorld ? OverloadingAspect.EnemyLifeRegenBoost * 9 : Main.hardMode ? OverloadingAspect.EnemyLifeRegenBoost * 3 : OverloadingAspect.EnemyLifeRegenBoost;
            if (divineDebuff)
                ApplyDPSDebuff(600, 100, ref npc.lifeRegen, ref damage);
            if (aghDebuff)
                ApplyDPSDebuff(480, 80, ref npc.lifeRegen, ref damage);
            if (wingsOfLightDebuff)
                ApplyDPSDebuff(300, 15, ref npc.lifeRegen, ref damage);
            if (blackBladeDebuff)
                ApplyDPSDebuff(200, 20, ref npc.lifeRegen, ref damage);
            if (ghostflameDebuff)
                ApplyDPSDebuff(170, 7, ref npc.lifeRegen, ref damage);
            if (infernoDebuff)
                ApplyDPSDebuff(500, 50, ref npc.lifeRegen, ref damage);
            if (blazingDebuff)
                ApplyDPSDebuff(255, 15, ref npc.lifeRegen, ref damage);
            if (healingDisabledDebuff)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.friendlyRegen = 0;

                if (npc.lifeRegenCount > 0)
                    npc.lifeRegenCount = 0;
            }
            //if (jidiDebuff)
            //{
            //    ApplyDPSDebuff(180, 8, ref npc.lifeRegen, ref damage);
            //}
        }

        // movement changes
        public override void PostAI(NPC npc)
        {
            // debuffs
            if (skadiDebuff)
            {
                npc.velocity.X *= 0.98f;
                npc.velocity.Y *= 0.98f;
            }
            if (freezingDebuff)
            {
                float reduction = Main.hardMode ? 0.96f : 0.97f;
                npc.velocity.X *= reduction;
                npc.velocity.Y *= reduction;
            }
            if (shivasDebuff)
            {
                npc.velocity.X *= 0.97f;
                npc.velocity.Y *= 0.97f;
            }

            // elites
            if (overloadingElite)
            {
                if (NPCID.Sets.ProjectileNPC[npc.type] == true)
                    return;
                npc.velocity.X *= 0.985f;
                npc.velocity.Y *= 0.985f;
            }
            if (goldElite)
            {
                if (NPCID.Sets.ProjectileNPC[npc.type] == true)
                    return;
                npc.velocity.X *= 0.975f;
                npc.velocity.Y *= 0.975f;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            MogPlayer mogPlayer = target.MogMod();
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
                npc.damage = (int)(npc.defDamage * .9f);
            else
                npc.damage = npc.defDamage;
            if (overloadingElite)
            {
                int overloadingType = ModContent.ProjectileType<HostileOverloadingOrbProj>();
                int damage = (int)(Main.masterMode ? npc.damage * 0.05f : Main.expertMode ? npc.damage * 0.05f : npc.damage * 0.05f);
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, overloadingType, damage, 2f, Main.myPlayer, ai1: 1f);
            }
            if (fireElite)
            {
                int buffType = Main.hardMode ? ModContent.BuffType<BlazingDebuff>() : BuffID.OnFire;
                int duration = Main.hardMode ? 240 : 180;
                target.AddBuff(buffType, duration);
            }
            if (toxicElite)
            {
                if (!target.HasBuff<ToxicDebuff>())
                    target.AddBuff(ModContent.BuffType<ToxicDebuff>(), 360);
                mogPlayer.toxicDamage += Main.rand.Next(ToxicDebuff.DamageMin, ToxicDebuff.DamageMax);
                if (Main.hardMode)
                    mogPlayer.toxicDamage += Main.rand.Next(ToxicDebuff.DamageMin + 20, ToxicDebuff.DamageMax + 20);
            }
            if (healingElite)
            {
                int duration = Main.hardMode ? 1080 : 720;
                target.AddBuff(ModContent.BuffType<HealingDisabledDebuff>(), duration);
            }
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
                modifiers.Defense *= skadiMult;
            if (jidiDebuff)
                modifiers.Defense.Flat -= jidiNumb;
            if (shivasDebuff)
                modifiers.Defense *= shivaMult;
            if (wingsOfLightDebuff)
                modifiers.CritDamage *= 1.1f;
            if (aghDebuff)
                modifiers.CritDamage *= 1.2f;
            if (blazingDebuff)
                modifiers.FinalDamage *= BlazingAspect.DamageMult + 1;
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
            if (blazingDebuff)
            {
                BlazingDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.OrangeRed;
            }
            if (toxicDebuff)
            {
                ToxicDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.Magenta;
            }
            if (healingDisabledDebuff)
            {
                HealingDisabledDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.DarkGreen;
            }    
            static void DrawDust(NPC npc, int rareDust, int commonDust, float size)
            {
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(3) ? rareDust : commonDust, npc.velocity.X * 0.04f, npc.velocity.Y * 0.04f, 100, default, size);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.65f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
            }
            if (overloadingElite)
            {
                DrawDust(npc, 161, DustID.MagnetSphere, 2.2f);
                drawColor = Color.AliceBlue;
            }
            if (fireElite)
            {
                DrawDust(npc, DustID.Lava, DustID.Flare, 2.2f);
                drawColor = Color.Orange;
            }
            if (goldElite)
            {
                DrawDust(npc, DustID.GoldCoin, DustID.Enchanted_Gold, 2.2f);
                drawColor = Color.Goldenrod;
            }
            if (healingElite)
            {
                DrawDust(npc, DustID.Terra, DustID.PoisonStaff, 2.2f);
                drawColor = Color.LimeGreen;
            }
            if (toxicElite)
            {
                DrawDust(npc, DustID.Venom, DustID.Poisoned, 2.2f);
                drawColor = Color.MediumPurple;
            }
            if (hellEpstein)
            {
                Lighting.AddLight(npc.Center, Color.OrangeRed.ToVector3());
                for (int n = 0; n < 6; n++)
                {
                    float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                    Vector2 swirlPos = npc.Center + Vector2.UnitX.RotatedBy(swirlRotation) * npc.width;
                    Vector2 swirlVelocity = Vector2.Normalize(swirlPos - npc.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f;
                    Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.CopperCoin, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    swirlDust.noGravity = true;
                }
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
            blazingDebuff = false;
            toxicDebuff = false;
            healingDisabledDebuff = false;
            overloadingOwner = npc.whoAmI;
        }
        #endregion
    }
}
