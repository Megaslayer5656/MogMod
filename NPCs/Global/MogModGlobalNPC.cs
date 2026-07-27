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
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
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

        public NPC.HitInfo hitInfo;
        public int maxBlood = 150;
        public int currentBlood = 0;

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
        public int toxicDamage = 0;
        public int currentCoins = 0;
        public int maxCoins = 5;

        public int overloadingRegenCooldown = OverloadingAspect.EnemyRegenWaitTime;
        public override bool InstancePerEntity => true;

        public static readonly SoundStyle BloodCrit = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/BloodCrit")
        {
            Volume = .7f,
            PitchVariance = .2f,
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
        #endregion

        #region Shops
        // modifies vanilla npc shop
        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.SkeletonMerchant:
                    shop.InsertAfter(ItemID.Rope, ModContent.ItemType<AstrologersStaff>(), (Condition.MoonPhasesHalf0));
                    shop.InsertAfter(ItemID.Rope, ModContent.ItemType<LabGerminator>(), (Condition.MoonPhasesHalf1));
                    break;
                case NPCID.Merchant:
                    shop.InsertAfter(ItemID.Glowstick, ModContent.ItemType<Crown>(), Condition.DownedEyeOfCthulhu, Condition.HappyEnoughToSellPylons);
                    break;
                case NPCID.Dryad:
                    shop.InsertAfter(ItemID.DirtRod, ModContent.ItemType<ForceStaff>(), Condition.HappyEnoughToSellPylons);
                    break;
                case NPCID.Demolitionist:
                    shop.InsertAfter(ItemID.Grenade, ModContent.ItemType<GasGrenade>(), Condition.HappyEnoughToSellPylons);
                    break;
            }
        }
        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            shop[nextSlot++] = ModContent.ItemType<CarianSlicer>();
            //if (Main.hardMode)
            //    shop[nextSlot++] = ModContent.ItemType<CarianSlicer>();
        }
        #endregion

        #region NPC Drops
        // LEDX and REDX chance to drop
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            int aspectChance = 1000;
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
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LabGerminator>(), 50, 1, 1));
                    break;
                case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                case NPCID.PigronHallow:
                    postFish.Add(ModContent.ItemType<BrinyRind>(), 4, 3, 5);
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
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SearingSignet>(), 20, 1, 1));
                    break;
                case NPCID.GoblinSummoner:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SearingSignet>(), 5, 1, 1));
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
            Vector2 dustNumb = new(1.6f, 2f);
            if (overloadingElite)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(5) ? DustID.GemSapphire : DustID.MagnetSphere, npc.velocity.X, npc.velocity.Y, 100, default, 1f);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                    dust.velocity *= 1.1f;
                }
                overloadingRegenCooldown--;
            }
            if (goldElite)
                npc.MaxFallSpeedMultiplier *= 2f;
            if (toxicElite)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(5) ? DustID.Venom : DustID.Poisoned, npc.velocity.X, npc.velocity.Y, 100, default, 1f);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(dustNumb.X, dustNumb.Y);
                    dust.velocity *= 1.1f;
                }
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            #region Setup
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
            int itemDamage = player.HeldItem.damage;
            int enemyMaxHP = npc.lifeMax;
            int shivDamage = MogModUtils.DamageHardCap(Convert.ToInt32(enemyMaxHP * 0.005) + 50, shivCap);
            int hellfireDamage = MogModUtils.DamageSoftCap(damageDone * HellfireMask.DamageMult, hellfireCap);
            int bashDamage = MogModUtils.DamageSoftCap(damageDone * GiantsMaul.DamageMult, bashCap);
            int overloadingDamage = (int)(damageDone * OverloadingAspect.DamageMult);
            var source = player.GetSource_OnHit(npc);
            bashProc = rand.Next(7) == 0;
            shivProc = rand.Next(5) == 0;

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
                //MessageID.CombatTextInt
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

            // polylute
            if (mogPlayer.polyluteActive)
            {
                Vector2 kirk = new Vector2(-10, 10).RotatedByRandom(MathHelper.ToRadians(360));
                int procChance = rand.Next(1, 6);

                if (procChance == 5)
                    Projectile.NewProjectile(source, npc.Center, kirk, ModContent.ProjectileType<PolyluteProj>(), Convert.ToInt32(damageDone * .3f) + 1, 3, player.whoAmI);
            }

            if (mogPlayer.wearingToxic && mogPlayer.toxicCooldown <= 0)
            {
                mogPlayer.toxicCooldown = 2;
                toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin - 6, NoxiousAspect.DamageMax - 20);
                if (Main.hardMode)
                    toxicDamage += Main.rand.Next(NoxiousAspect.DamageMin, NoxiousAspect.DamageMax);
                //Main.NewText($"sd phonk damage is: {toxicDamage}");
            }
            if (mogPlayer.wearingOverloading && mogPlayer.overloadingCooldown <= 0)
            {
                mogPlayer.overloadingCooldown = cooldownTimer;
                Projectile orb = Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<OverloadingOrbProj>(), overloadingDamage, 0f, player.whoAmI, ai2: 1f);
                orb.DamageType = player.HeldItem.DamageType;
            }
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
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            var source = player.GetSource_OnHit(npc);
            int bloodToAdd = projectile.GetGlobalProjectile<MogModGlobalProjectile>().bloodDamage;
            // add another blood accessory
            if (mogPlayer.exultationEquipped)
                bloodToAdd = (int)(bloodToAdd * LordOfBloodsExultation.BloodMult);

            if (mogPlayer.mercyBladeEquipped)
                bloodToAdd = (int)(bloodToAdd * BladeOfMercy.BloodMult);

            if (mogPlayer.wearingWhiteArmor)
                bloodToAdd = (int)(bloodToAdd * (WhiteMask.BloodMult + 1));

            if (mogPlayer.wearingFlayersBota)
                bloodToAdd = (int)(bloodToAdd * FlayersBota.BloodMult);

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

            if (Main.netMode == NetmodeID.MultiplayerClient) //If you do anything else in this method do it before here bc this returns sometimes
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MogModMessageType.AddBloodFromProjectile);
                packet.Write(npc.whoAmI);
                packet.Write(bloodToAdd);
                packet.Send();
                return;
            }
            AddProjectileBlood(npc, bloodToAdd, player);
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
            MogPlayer mogPlayer = player.MogMod();

            maxBlood = (int)(npc.lifeMax * .05 + npc.defense);

            if (maxBlood < 150)
                maxBlood = 150;

            int bloodToAdd = globalItem.bloodDamage;

            // add another blood accessory
            if (mogPlayer.exultationEquipped)
                bloodToAdd = (int)(bloodToAdd * LordOfBloodsExultation.BloodMult);

            if (mogPlayer.mercyBladeEquipped)
                bloodToAdd = (int)(bloodToAdd * BladeOfMercy.BloodMult);

            if (mogPlayer.wearingWhiteArmor)
                bloodToAdd = (int)(bloodToAdd * (WhiteMask.BloodMult + 1));

            if (mogPlayer.wearingFlayersBota)
                bloodToAdd = (int)(bloodToAdd * FlayersBota.BloodMult);

            currentBlood += bloodToAdd;

            if (currentBlood >= maxBlood)
            {
                ApplyBleedProc(npc, player);
            }
        }
        public void AddProjectileBlood(NPC npc, int bloodToAdd, Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            currentBlood += bloodToAdd;

            if (currentBlood >= maxBlood)
            {
                ApplyBleedProc(npc, player);
            }
        }
        public void ApplyBleedProc(NPC npc, Player player)
        {
            MogPlayer mogPlayer = player.MogMod();
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
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (mogPlayer.wearingFlayersBota && npc.type != NPCID.TargetDummy)
                {
                    mogPlayer.satanicAccCooldown = cooldownTimer * 2;
                    int heal = (int)(hitInfo.Damage / 100) + 1;
                    heal *= Convert.ToInt32(player.lifeSteal * 0.01);
                    player.statLife += heal;
                    player.HealEffect(heal);
                    if (player.statLife > player.statLifeMax2)
                        player.statLife = player.statLifeMax2;
                }
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
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                    if (!Condition.DownedMechBossAll.IsMet() && (!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                    {
                        HellfireEssenceText = Mod.GetLocalization($"WorldGen.{nameof(HellfireEssenceText)}");
                        WorldGeneration.BroadcastLocalizedText(HellfireEssenceText.Value, Color.Orange);

                        SyncWorld();
                    }
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
                            hellEpstein = true;
                            npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.5f, 3f));
                            npc.life = npc.lifeMax;
                            npc.defDamage = (int)(npc.damage * 1.5f);
                            npc.knockBackResist *= 0.2f;
                            return;
                    }
                }
            }
            // elite enemy spawning
            if (source is EntitySource_Parent { Entity: NPC parent })
            {
                if (parent.MogMod().overloadingElite)
                {
                    overloadingElite = true;
                    return;
                }
                if (parent.MogMod().fireElite)
                {
                    fireElite = true;
                    return;
                }
                if (parent.MogMod().goldElite)
                {
                    goldElite = true;
                    return;
                }
                if (parent.MogMod().healingElite && npc.type != ModContent.NPCType<HealingOrb>())
                {
                    healingElite = true;
                    return;
                }
                if (parent.MogMod().toxicElite)
                {
                    toxicElite = true;
                    return;
                }
            }
            int chance = Main.zenithWorld ? 3 : 10;
            if (Main.rand.NextBool(chance) && MogServerConfig.Instance.EliteEnemySpawning)
            {
                if (npc.friendly || npc.boss || npc.dontCountMe || NPCID.Sets.ProjectileNPC[npc.type] || NPCID.Sets.ShouldBeCountedAsBoss[npc.type] || npc.type == NPCID.TargetDummy)
                    return;
                npc.value *= 1.5f; // all elites drop extra money
                switch (Main.rand.Next(0, 5))
                {
                    // overloading (spawn sticky orb, double health)
                    case 0:
                        overloadingElite = true;
                        npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(Main.zenithWorld ? 3f : 1.5f, Main.zenithWorld ? 5f : 3f));
                        npc.life = npc.lifeMax;
                        npc.defDamage = (int)(npc.damage * 1.3f);
                        npc.scale *= Main.rand.NextFloat(1.2f, 1.5f);
                        npc.knockBackResist *= 0.2f;
                        return;
                    // fire (ignite, explode on kill)
                    case 1:
                        fireElite = true;
                        npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.2f, 2f));
                        npc.life = npc.lifeMax;
                        npc.defDamage = (int)(npc.damage * (Main.zenithWorld ? 3f : 1.8f));
                        return;
                    // gold (reflect proj, drop more gold)
                    case 2:
                        goldElite = true;
                        npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(1.6f, 2.2f));
                        npc.life = npc.lifeMax;
                        npc.reflectsProjectiles = true;
                        npc.value *= Main.zenithWorld ? 15f : 5f;
                        return;
                    // mending (heal other enemies, health regen)
                    case 3:
                        healingElite = true;
                        npc.lifeMax = (int)(npc.lifeMax * Main.rand.NextFloat(Main.zenithWorld ? 0.2f : 0.5f, Main.zenithWorld ? 0.5f : 0.8f));
                        npc.life = npc.lifeMax;
                        npc.scale *= Main.rand.NextFloat(Main.zenithWorld ? 0.2f : 0.6f, Main.zenithWorld ? 0.5f : 0.9f);
                        npc.knockBackResist *= 1.5f;
                        return;
                    // poison (stacking poison debuff that kirks you when it ends (think SD kid phonk))
                    case 4:
                        toxicElite = true;
                        npc.defDamage = (int)(npc.damage * 0.7f);
                        break;
                }
            }
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
            if (healingElite)
            {
                npc.lifeRegen += Main.zenithWorld ? MendingAspect.LifeRegen * 21 : Main.hardMode ? MendingAspect.LifeRegen * 3 : MendingAspect.LifeRegen;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    int size = 10;
                    Rectangle healingBox = new((int)(npc.Center.X - npc.width * size / 2), (int)(npc.Center.Y - npc.height * size / 2), npc.Hitbox.Width * size, npc.Hitbox.Height * size);
                    if (!target.active || target.friendly || !healingBox.Intersects(target.Hitbox) || 
                        npc.dontCountMe || NPCID.Sets.ProjectileNPC[npc.type] || NPCID.Sets.ShouldBeCountedAsBoss[npc.type] || 
                        npc.type == NPCID.TargetDummy || target.MogMod().healingElite || target.life >= target.lifeMax)
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
            overloadingOwner = npc.whoAmI;
        }
        #endregion
    }
}
