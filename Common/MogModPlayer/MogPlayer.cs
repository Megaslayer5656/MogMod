using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Accessories.Boots;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Items.Accessories.Rigs;
using MogMod.Items.Accessories.Wings;
using MogMod.Items.Ammo.SorcerySpells;
using MogMod.Items.Armor.Fae;
using MogMod.Items.Armor.Radiant;
using MogMod.Items.Armor.Seraphic;
using MogMod.Items.Other;
using MogMod.Items.Placeable.MusicBoxes;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using MogMod.NPCs.Global;
using MogMod.NPCs.ProjectileEnemies;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.EnemyProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Projectiles.Pets;
using MogMod.Utilities;
using MogMod.World;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.WorldBuilding;
using static AssGen.Assets;
using static System.Net.Mime.MediaTypeNames;

namespace MogMod.Common.MogModPlayer
{
    // this files a mess to look at
    public partial class MogPlayer : ModPlayer
    {
        #region Setup
        public bool mewing = false;
        public float mewingguide = 0;

        Random rand = new Random();
        public bool mouseRight = false;
        private bool oldMouseRight = false;
        public Vector2 mouseWorld => Player.MountedCenter + mouseWorldDeltaFromPlayer;
        public Vector2 mouseNormalFromPlayer => mouseRotationFromPlayer.ToRotationVector2();
        public Vector2 mouseWorldDeltaFromPlayer;
        public float mouseRotationFromPlayer;
        private Vector2 oldMouseWorldDeltaFromPlayer;
        private int mouseWorldPacketTimer = 0;
        private const int MouseWorldPacketInterval = 2;
        /// <summary>
        /// Set this to true if you need to receive updates on right clicks from players and sync them in multiplayer.<br/>
        /// Automatically resets itself after sending an update.
        /// </summary>
        public bool rightClickListener = false;
        /// <summary>
        /// Set this to true if you need to receive updates on the position of the player's mouse and sync them in multiplayer.<br/>
        /// Automatically resets itself after sending an update.<br/>
        /// This also update the rotation.
        /// </summary>
        public bool mouseWorldListener = false;
        /// <summary>
        /// Set this to true if you need to receive updates on the rotation of the mouse to the player. This sends updates less frequently the tighter the tolerance of mouseWorldListener.<br/>
        /// Automatically resets itself after sending an update.<br/>
        /// This does NOT update the position.
        /// </summary>
        public bool mouseRotationListener = false;
        public bool syncMousePosition = false;
        public bool syncMouseRotation = false;
        public bool syncMouseRightClick = false;

        /// <summary>
        /// General variable used for controlling the strength of screenshake this player is experiencing. Measured in pixels of offset that can be applied to the screen.<br/>
        /// When setting this, be sure to only set it if its current value is less than the value to set, to prevent overriding an ongoing stronger screenshake with a weaker one.<br/>
        /// A helper method exists which can automatically do this for you, <see cref="MogModUtils.SetScreenshake"/>.
        /// </summary>
        public float GeneralScreenShakePower = 0f;

        #region Accessories
        public bool wearingRigSlot;
        public bool isWearingGlimmerCape = false;
        public bool armletActive = false;
        public bool wearingManaBoots = false;
        public bool wearingSatanic = false;
        public bool wearingRefresherOrb = false;
        public bool wearingGigaManaBoots = false;
        public bool wearingMekansm = false;
        public bool wearingEyeOfSkadi = false;
        public bool wearingWingsOfLight = false;
        public bool wingsOfLightVisual = false;
        public bool wearingFishSlop1 = false;
        public bool wearingFishSlop2 = false;
        public bool wearingGiantsMaul = false;
        public bool wearingGunpowderGauntlet = false;
        public bool wearingDuelistGloves = false;
        public bool wearingWhisperDread = false;
        public bool wearingSerratedShiv = false;
        public bool wearingUndyingHelm = false;
        public bool wearingSearingSignet = false;
        public bool wearingVladimirs = false;
        public bool wearingWraithPact = false;
        public bool wearingJidiPollenBag = false;
        public bool wearingShadowAmulet = false;
        public bool shadowAmuletVisual = false;
        public bool wearingMendez;
        public bool plasmaVisual;
        public bool polyluteVisual;
        public bool wearingRuntyHorseshoe;
        public bool wearingAllegianceWings;
        public bool wearingSacrosanctAegis;
        public bool wearingSigmaCharm;
        public bool sigmaCharmVisual;
        public bool wearingAghGauntlet;
        public bool aghGauntletVisual;
        public int gloveLevel;
        public bool wearingElvenQuiver;
        public bool wearingEnchantedQuiver;
        public bool wearingFlayersBota;
        public bool wearingScavVest;
        public bool wearingTriton;
        public bool tritonActive;
        public bool wearingZhuk;
        public bool zhukActive;
        public bool wearingPowerTreads;
        public bool wearingTreadsLife;
        public bool wearingTreadsDamage;
        public bool wearingTreadsBuilding;
        public bool wearingOverloading;
        public bool overloadingVisual;
        public bool wearingBlazing;
        public bool blazingVisual;
        public bool wearingGilded;
        public bool gildedVisual;
        public bool wearingMending;
        public bool mendingVisual;
        public bool wearingToxic;
        public bool toxicVisual;
        public bool wearingChaosDice;

        public float ammoCost = 1f;

        public bool stopFallDamage;
        int fallDamageTimer = 0;

        public bool wraithActive = false;

        public int shadowTimer = 0;
        public const int shadowTimerMax = 240;

        public int locketCharges = 0;
        public static int maxLocketCharges = 20;
        public bool locketActive = false;

        public int wandCharges = 0;
        public static int maxWandCharges = 20;
        public bool wandActive = false;

        public int stickCharges = 0;
        public static int maxStickCharges = 10;
        public bool stickActive = false;

        public int armletTimer = 0;
        public int armletTimerMax = 120;

        public bool wearingHelmOfDominator;
        public bool wearingHelmOfOverlord;
        public bool wearingForceStaff;
        public bool wearingPike;

        public int duelistStacks = 0;
        public static int maxDuelistStacks = 3;

        public bool diademMinion = false;
        public bool dominatorMinion = false;
        public bool overlordMinion = false;

        public bool wearingShivasGuard = false;
        public int shivasSlowTimer = 0;
        public int shivasSlowTimerMax = 36000;
        public bool shivasAttack = false;

        public int wingsOfLightDust = 0;

        public int forceDirection = -1;

        public int DashDir = -1;
        public int FaeDashDelay = 0; // frames remaining till we can dash again
        public int FaeDashTimer = 0; // frames remaining in the dash

        public const int FaeDashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public const int FaeDashDuration = 35; // Duration of the dash afterimage effect in frames

        public int AegisDashDelay = 0;
        public int AegisDashTimer = 0;

        public const int AegisDashCooldown = 200;
        public const int AegisDashDuration = 40;

        public int ForceDashTimer = 0;
        //public const int ForceDashCooldown = 60;
        public const int ForceDashDuration = 20;

        public int PikeDashTimer = 0;
        //public const int PikeDashCooldown = 60;
        public const int PikeDashDuration = 60;

        public bool canDashUp;

        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;
        
        public const float FaeDashVelocity = 22f;
        public const float AegisDashVelocity = 30f;
        public const float ForceDashVelocity = 15f;
        public const float PikeDashVelocity = 25f;


        public bool atgActive = false;
        public bool plasmaActive = false;
        public bool icbmActive = false;
        public bool polyluteActive = false;

        public bool exultationEquipped = false;
        public bool mercyBladeEquipped = false;

        public int shivCooldown = 0;
        public int bashCooldown = 0;
        public int gunpowderCooldown = 0;
        public int radiantCooldown = 0;
        public int jidiPollenCooldown = 0;
        public int satanicAccCooldown = 0;
        public int toxicCooldown = 0;
        public int overloadingCooldown = 0;
        public int gildedReflectCooldown = 0;
        public int gildedCoinDropCooldown = 0;
        public int overloadingRegenCooldown = OverloadingAspect.RegenWaitTime;

        // dragon install
        public bool wearingFlameOfCorruption = false;
        public bool dragonInstallActive;

        public int cooldownReference;
        public enum MewingType
        {
            mewingguide = 0
        }
        public MewingType mewingType = MewingType.mewingguide;
        #endregion

        #region Armor
        public bool wearingBladeMail;
        public bool wearingFrostArmor;
        public bool wearingFrostMagic;
        public bool wearingFrostSummon;
        public bool wearingSpiritArmor;
        public bool wearingDamascus1;
        public bool wearingDamascus2;
        public bool wearingBoneArmor;
        public bool wearingRadiantArmor;
        public bool wearingUndyingArmor;
        public bool wearingHellfireArmor;
        public int hellfireCooldown = 0;
        public bool wearingTankyRizzler;
        public int tankyRizzlerHits = 0;
        public static int counterHelixDmg = 500;
        public bool wearingWhiteArmor;
        public bool wearingFaeArmor;
        public static int wraithDamage = 100;
        public bool wearingSeraphic;
        public int seraphicReviveCounter = 0;
        public bool canSeraphicRevive;
        public bool wearingNihilum;
        public bool wearingNihilumRanged;
        public int nihilumTimer = 0;
        public int nihilumTimerMax = 120;

        public int VoniumLifeCooldown = 0;
        #endregion

        #region Weapons
        public int essenceShiftLevel = 0;
        public static int essenceShiftLevelMax = HydrakanLatch.EssenceMax;

        public int fierySoulLevel = 0;
        public static int fierySoulLevelMax = 30;

        public bool holdingThrowingShade;
        public int shadowRealmLevel = 0;
        public static int shadowRealmLevelMax = 150;

        public int eSeraphCharge = 0;
        public int eSeraphMax = 100;
        public bool eSeraphSound;

        public bool chargeShot = false;
        public bool dpCharge = false;

        public bool inShadowRealm;
        public bool krakenBuff;

        public bool markerProjOut = false;

        public int hellfireOverheat = 0;

        //public float maxShotsMult = 1f;
        //public float reloadTimeMult = 1f;

        /*
        public static List<int> PlayerHurtWeapons =
        [
            ModContent.ItemType<BloodGrenade>(),
            ModContent.ItemType<WarriorsSpear>(),
            ModContent.ItemType<BerserkersSpear>()
        ];
        */
        #endregion

        #region Summons
        public bool fCrystal;
        public bool divinitasMinion;
        #endregion

        #region Buffs

        public bool infiniteFlight = false;

        // debuffs
        public bool divineDebuff;
        public bool skadiDebuff;
        public bool freezingDebuff;
        public bool aghHexDebuff;
        public bool wingsOfLightDebuff;
        public bool ghostflameDebuff;
        public bool jidiDebuff;
        public bool shivaDebuff;
        public bool infernoDebuff;
        public bool armletDebuff;
        public bool nulledDebuff;
        public bool blazingDebuff;
        public bool toxicDebuff;
        public bool deathDebuff;
        public bool healingDisabledDebuff;

        public int toxicDamage = 0;

        // auras
        public bool greavesAura = false;
        public bool wraithAura = false;
        public bool vladsAura = false;
        public bool headdressAura = false;
        public bool drumsAura = false;
        public bool shivasAura = false;
        public bool mendingAura = false;

        public float auraRange = 5000f;

        public bool satanicBuff;

        public int praporCooldown = 0;

        // pets
        public bool ahmodPet = false;
        public bool gingyPet = false;

        #endregion

        #region Sound Effects
        public static readonly SoundStyle WandUse = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Magic_Stick")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ArmletOnSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ArmletOn")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ArmletOffSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ArmletOff")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ShivasActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ShivasActivate")
        {
            Volume = .35f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ParrySound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ParrySfx")
        {
            Volume = .5f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle BladeMailActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/BladeMailActivate")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ForceStaffActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ForceStaffActivate")
        {
            Volume = .45f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle GlimmerActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/GlimmerActivate")
        {
            Volume = .7f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle GreavesActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/GreavesActivate")
        {
            Volume = .65f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ManaBootsActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ManaBootsActivate")
        {
            Volume = .65f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle MekansmActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/MekansmActivate")
        {
            Volume = .65f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle RefresherActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/RefresherActivate")
        {
            Volume = .35f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle SatanicActivateSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/SatanicActivate")
        {
            Volume = .35f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        #endregion

        #region Mod Buff ID/s
        int glimmerBuff = ModContent.BuffType<GlimmerCapeBuff>();
        int satanicBuffID = ModContent.BuffType<SatanicBuff>();
        int blademailBuff = ModContent.BuffType<BladeMailBuff>();

        // cooldowns
        int refresherCooldown = ModContent.BuffType<RefresherOrbDebuff>();
        int glimmerCooldown = ModContent.BuffType<GlimmerCapeDebuff>();
        int satanicCooldown = ModContent.BuffType<SatanicDebuff>();
        int manabootsCooldown = ModContent.BuffType<ArcaneBootsDebuff>();
        int guardianCooldown = ModContent.BuffType<GuardianGreavesDebuff>();
        int mekansmCooldown = ModContent.BuffType<MekansmDebuff>();
        int forceStaffCooldown = ModContent.BuffType<ForceStaffDebuff>();
        int blademailCooldown = ModContent.BuffType<BladeMailDebuff>();
        int ShivasCooldown = ModContent.BuffType<ShivasDebuff>();

        // armlet
        int armletToggled = ModContent.BuffType<ArmletOfMordiggianBuff>();
        int nihilumToggled = ModContent.BuffType<NulledDebuff>();

        // dragon install
        int dragonInstall = ModContent.BuffType<DragonInstallBuff>();
        int dragonInstallCooldown = ModContent.BuffType<DragonInstallCooldown>();
        #endregion

        #endregion

        #region In Game Checks

        public override void OnEnterWorld()
        {
            if (Main.rand.Next(0, 10) == 0)
                Main.NewText("enimga daedalus butternfly", 200, 250, 224);
            if (Main.rand.Next(0, 100) == 0)
                Main.NewText("Von would like to have a word with you...", new Color(Main.DiscoR / 5, (byte)(Main.DiscoG / 0f), (byte)(Main.DiscoB / 5f)));
        }

        #region On Hit Effects
        public void NPCDebuffs(NPC target, bool melee, bool ranged, bool magic, bool summon, bool whip, bool crit, bool proj = false, bool noFlask = false)
        {
            if (wearingEyeOfSkadi)
                target.AddBuff(ModContent.BuffType<EyeOfSkadiDebuff>(), 180);
            if (wearingSearingSignet && !melee)
                target.AddBuff(BuffID.ShadowFlame, 180);
            if (Player.HasBuff<DragonInstallBuff>())
                target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 600);
            if (wearingFrostArmor && magic)
                target.AddBuff(ModContent.BuffType<FreezingDebuff>(), 300);
            if (wearingHellfireArmor)
                target.AddBuff(BuffID.OnFire3, 180);
            if (melee && wearingAghGauntlet)
                target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 180);
            if (wearingBlazing)
                target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 300);
            if (wearingGilded)
                target.AddBuff(BuffID.Midas, 180);
            if (wearingToxic && !target.HasBuff<ToxicDebuff>())
                target.AddBuff(ModContent.BuffType<ToxicDebuff>(), 600);
            if (wearingMending)
                target.AddBuff(ModContent.BuffType<HealingDisabledDebuff>(), 600);
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            NPCDebuffs(target, item.CountsAsClass<MeleeDamageClass>(), item.CountsAsClass<RangedDamageClass>(), item.CountsAsClass<MagicDamageClass>(), item.CountsAsClass<SummonDamageClass>(), item.CountsAsClass<SummonMeleeSpeedDamageClass>(), hit.Crit);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            MogModGlobalProjectile mogProj = proj.MogMod();
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
            NPCDebuffs(target, proj.CountsAsClass<MeleeDamageClass>(), proj.CountsAsClass<RangedDamageClass>(), proj.CountsAsClass<MagicDamageClass>(), proj.CountsAsClass<SummonDamageClass>(), proj.CountsAsClass<ThrowingDamageClass>(), proj.CountsAsClass<SummonMeleeSpeedDamageClass>(), hit.Crit);
        }
        public void doATG(int damageDone)
        {
            float Spread = 0.3f;
            Vector2 kirk = new Vector2(0, Main.zenithWorld ? -1 : -7);
            Vector2 einstein = Main.MouseWorld - Player.Center;
            einstein.Normalize();

            Vector2 epstein = einstein * 20;

            int procChance = rand.Next(1, 11);
            if (atgActive && procChance == 5)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk, ModContent.ProjectileType<ATGProjectile>(), damageDone + 1, 3, Player.whoAmI);
                if (icbmActive)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk.RotatedBy(Spread), ModContent.ProjectileType<ATGProjectile>(), Convert.ToInt32(damageDone * .5f) + 1, 3, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk.RotatedBy(-Spread), ModContent.ProjectileType<ATGProjectile>(), Convert.ToInt32(damageDone * .5f) + 1, 3, Player.whoAmI);
                    if (Main.zenithWorld)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk.RotatedBy(Spread * 3), ModContent.ProjectileType<ATGProjectile>(), damageDone, 3, Player.whoAmI);
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk.RotatedBy(-Spread * 3), ModContent.ProjectileType<ATGProjectile>(), damageDone, 3, Player.whoAmI);
                    }
                }
                if (Main.zenithWorld)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk.RotatedBy(Spread * 2), ModContent.ProjectileType<ATGProjectile>(), damageDone, 3, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk.RotatedBy(-Spread * 2), ModContent.ProjectileType<ATGProjectile>(), damageDone, 3, Player.whoAmI);
                }
            }
            if (plasmaActive)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, epstein, ModContent.ProjectileType<PlasmaShrimpProj>(), Convert.ToInt32(damageDone * .1f) + 1, 1, Player.whoAmI);
        }
        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
            MogModGlobalNPC mogNPC = npc.MogMod();
            if (Player.HasItemInAnyInventory(ModContent.ItemType<HolyLocket>()))
            {
                locketCharges += 1;
                if (locketCharges > maxLocketCharges)
                {
                    locketCharges = maxLocketCharges;
                }
            }

            if (Player.HasItemInAnyInventory(ModContent.ItemType<MagicWand>()))
            {
                wandCharges += 1;
                if (wandCharges > maxWandCharges)
                {
                    wandCharges = maxWandCharges;
                }
            }

            if (Player.HasItemInAnyInventory(ModContent.ItemType<MagicStick>()))
            {
                stickCharges += 1;
                if (stickCharges > maxStickCharges)
                {
                    stickCharges = maxStickCharges;
                }
            }

            if (Player.HasItemInAnyInventory(ModContent.ItemType<BlinkDagger>()))
            {
                Player.AddBuff(ModContent.BuffType<BlinkDebuff>(), 600);
            }

            if (wearingTankyRizzler)
            {
                tankyRizzlerHits++;
                if (tankyRizzlerHits >= 2)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<CounterHelixProj>(), counterHelixDmg, 1, Player.whoAmI, 0);
                    tankyRizzlerHits = 0;
                }
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Player.HasItemInAnyInventory(ModContent.ItemType<HolyLocket>()))
            {
                locketCharges += 1;
                if (locketCharges > maxLocketCharges)
                {
                    locketCharges = maxLocketCharges;
                }
            }

            if (Player.HasItemInAnyInventory(ModContent.ItemType<MagicWand>()))
            {
                wandCharges += 1;
                if (wandCharges > maxWandCharges)
                {
                    wandCharges = maxWandCharges;
                }
            }

            if (Player.HasItemInAnyInventory(ModContent.ItemType<MagicStick>()))
            {
                stickCharges += 1;
                if (stickCharges > maxStickCharges)
                {
                    stickCharges = maxStickCharges;
                }
            }

            if (Player.HasItemInAnyInventory(ModContent.ItemType<BlinkDagger>()))
            {
                Player.AddBuff(ModContent.BuffType<BlinkDebuff>(), 600);
            }

            if (wearingTankyRizzler)
            {
                tankyRizzlerHits++;
                if (tankyRizzlerHits >= 2)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<CounterHelixProj>(), counterHelixDmg, 1, Player.whoAmI, 0);
                    tankyRizzlerHits = 0;
                }
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            int actualProjDamage = proj.damage;
            double dodgeDamageGateValuePercent = 0.05;
            int dodgeDamageGateValue = (int)Math.Round(Player.statLifeMax2 * dodgeDamageGateValuePercent);
            if (!proj.reflected && !ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[proj.type])
            {
                float damageMult = Main.GameModeInfo.EnemyDamageMultiplier;
                if (Main.GameModeInfo.IsJourneyMode)
                {
                    var power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
                    if (power.GetIsUnlocked())
                        damageMult = power.StrengthMultiplierToGiveNPCs;
                }

                // in real tML, the factor 2 is applied in Projectile.Damage()
                actualProjDamage = (int)Math.Floor(2 * damageMult * (float)actualProjDamage);
            }
            //Main.NewText($"damage done was {actualProjDamage}, damage needed is {dodgeDamageGateValue}");
            if (proj.active && proj.hostile && modifiers.Dodgeable && proj.damage > 0)
            {
                if (actualProjDamage >= dodgeDamageGateValue)
                {
                    if (wearingGilded && gildedReflectCooldown <= 0 && !MogModProjectileSets.ShouldNotBeReflected[proj.type] && !modifiers.PvP && !proj.friendly)
                    {
                        proj.hostile = false;
                        proj.friendly = true;
                        proj.damage = actualProjDamage;
                        proj.velocity *= -1f;
                        proj.penetrate = 1;

                        SoundEngine.PlaySound(SoundID.Item150, Player.Center);
                        int reflectIFrames = Player.ComputeReflectIFrames();
                        Player.GiveUniversalIFrames(reflectIFrames, true);
                        modifiers.Cancel();
                        gildedReflectCooldown = GildedAspect.ReflectCooldown;
                    }
                }
            }
        }
        #endregion

        // the big one
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            #region Accessory Checks
            // this works while dead for some reason
            if (Player.dead)
                return;
            // refresher orb
            if (KeybindSystem.RefresherOrbKeybind.JustPressed && wearingRefresherOrb && !Player.HasBuff(refresherCooldown))
            {
                // make it play a sound when activating (add any additional debuffs here)
                Player.ClearBuff(glimmerCooldown);
                Player.ClearBuff(satanicCooldown);
                Player.ClearBuff(manabootsCooldown);
                Player.ClearBuff(guardianCooldown);
                Player.ClearBuff(mekansmCooldown);
                Player.ClearBuff(forceStaffCooldown);
                Player.ClearBuff(blademailCooldown);
                Player.ClearBuff(ShivasCooldown);
                Player.ClearBuff(ModContent.BuffType<BlinkDebuff>());
                Player.ClearBuff(ModContent.BuffType<ButterflyCooldown>());
                Player.ClearBuff(ModContent.BuffType<LagunaBladeCooldown>());
                Player.ClearBuff(ModContent.BuffType<ParryCooldown>());
                //Don't add dragon install to this. It shouldn't be able to be refreshed by refresher as it is more of a different mechanic than a buff. Will if you see this stop playing Chen <-- Chen (pronounced "shen") has crazy micro and once i get good at him hes gonna be crazy. that one game was a loss no matter who i played. also it was mendez fault for picking IO

                Player.AddBuff(refresherCooldown, 9000);
                SoundEngine.PlaySound(RefresherActivateSound, Player.Center);
            }

            // glimmer cape
            if (KeybindSystem.GlimmerCapeKeybind.JustPressed && isWearingGlimmerCape && !Player.HasBuff(glimmerCooldown))
            {
                // give buff, 600 = 10 seconds
                Player.AddBuff(glimmerBuff, 1800);
                // give debuff cd
                Player.AddBuff(glimmerCooldown, 3600);
                // play sfx
                SoundEngine.PlaySound(GlimmerActivateSound, Player.Center);
            }

            // satanic
            if (KeybindSystem.SatanicKeybind.JustPressed && wearingSatanic && !Player.HasBuff(satanicCooldown))
            {
                Player.AddBuff(satanicBuffID, 480);
                Player.AddBuff(satanicCooldown, 4800);
                SoundEngine.PlaySound(SatanicActivateSound, Player.Center);
            }

            // blademail
            if (KeybindSystem.BladeMailKeybind.JustPressed && wearingBladeMail && !Player.HasBuff(blademailCooldown))
            {
                Player.AddBuff(blademailBuff, 600);
                Player.AddBuff(blademailCooldown, 3600);
                SoundEngine.PlaySound(BladeMailActivateSound, Player.Center);
            }

            // arcane boots
            if (KeybindSystem.BootsKeybind.JustPressed && wearingManaBoots && !Player.HasBuff(manabootsCooldown))
            {
                //for (int i = 0; i < Main.maxPlayers; i++)
                //{
                //    Terraria.Player targetPlayer = Main.player[i];
                //    if (targetPlayer.active && targetPlayer.team == targetPlayer.team && targetPlayer.team != 0)
                //    {
                //        targetPlayer.AddBuff(greavesHeal, 600);
                //        //if (Main.netMode == NetmodeID.Server) // Check if the game is in multiplayer server mode
                //        //{
                //        //    NetMessage.SendData(MessageID.PlayerBuffs, -1, -1, null, i, mekansmHeal, 600f, 0f, 0, 0, 0);
                //        //}
                //        for (int k = 0; k < 16; k++)
                //        {
                //            Dust dust2 = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.ManaRegeneration);
                //            dust2.scale = Main.rand.NextFloat(0.6f, 0.8f);
                //        }
                //    }
                //}
                Player.HealManaMult(ArcaneBoots.ManaHeal);
                Player.AddBuff(manabootsCooldown, 1800);
                SoundEngine.PlaySound(ManaBootsActivateSound, Player.Center);
            }

            // guardian greaves
            if (KeybindSystem.BootsKeybind.JustPressed && wearingGigaManaBoots && !Player.HasBuff(guardianCooldown))
            {
                Player.HealLifeMult(GuardianGreaves.LifeHeal);
                Player.HealManaMult(GuardianGreaves.ManaHeal);
                Player.AddBuff(guardianCooldown, 3600);
                SoundEngine.PlaySound(GreavesActivateSound, Player.Center);
            }

            // mekansm
            if (KeybindSystem.MekansmKeybind.JustPressed && wearingMekansm && !Player.HasBuff(mekansmCooldown))
            {
                // make it play a sound when activating
                Player.HealLifeMult(Mekansm.LifeHeal);
                Player.AddBuff(mekansmCooldown, 3600);
                SoundEngine.PlaySound(MekansmActivateSound, Player.Center);
            }

            // holy locket
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (locketActive && locketCharges > 0)
                {
                    int heal = HolyLocket.LifeHeal * locketCharges;
                    int mana = HolyLocket.ManaHeal * locketCharges;

                    Player.HealLifeMult(heal);
                    Player.HealManaMult(mana);

                    locketCharges = 0;
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            //Wand
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (wandActive && wandCharges > 0)
                {
                    int heal = MagicWand.LifeHeal * wandCharges;
                    int mana = MagicWand.ManaHeal * locketCharges;

                    Player.HealLifeMult(heal);
                    Player.HealManaMult(mana);

                    wandCharges = 0;
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            //Magic Stick
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (stickActive && stickCharges > 0)
                {
                    int heal = MagicStick.LifeHeal * wandCharges;
                    int mana = MagicStick.ManaHeal * locketCharges;

                    Player.HealLifeMult(heal);
                    Player.HealManaMult(mana);

                    stickCharges = 0;
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            //Shiva's Guard
            if (KeybindSystem.ShivasKeybind.JustPressed && wearingShivasGuard && !Player.HasBuff(ShivasCooldown))
            {
                doShivas(Player, Player.Center); //Does the thing.
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncShivas(false, Player.Center); //Netcode stuff, go to MogPlayerNetcode.cs to see what this does.
                }
            }

            // wings of light
            if (wearingWingsOfLight)
            {
                doWingsOfLight(Player, Player.Center); //Does the thing.
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncWingsOfLight(false, Player.Center); //Netcode stuff, go to MogPlayerNetcode.cs to see what this does.
                }
            }

            //Dragon Install
            if (wearingFlameOfCorruption && KeybindSystem.DragonInstallKeybind.JustPressed && !Player.HasBuff(dragonInstallCooldown))
            {
                Player.AddBuff(dragonInstall, 6000); //These values are temporary.
                Player.AddBuff(dragonInstallCooldown, 12000);
                for (int i = 0; i < 80; i++)
                {
                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 2;

                    int dustPos = 20;

                    int DI1 = Dust.NewDust(Player.Center, dustPos, dustPos, DustID.CrimsonTorch, dustVelocity.X * 3, dustVelocity.Y * 3, 0, default, 1f);
                    Main.dust[DI1].noGravity = true;
                    Main.dust[DI1].fadeIn = 2f;
                    Main.dust[DI1].velocity *= 3f;
                    int DI2 = Dust.NewDust(Player.Center, dustPos - 5, dustPos - 5, ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, dustVelocity.X * 2, dustVelocity.Y * 2, 0, Color.Red, 2f);
                    Main.dust[DI2].noGravity = true;
                    Main.dust[DI2].fadeIn = 2f;
                    Main.dust[DI2].velocity *= 3f;
                }
            }

            // armlet timer
            if (KeybindSystem.ArmletKeybind.JustPressed && armletActive)
            {
                if (armletTimer <= armletTimerMax)
                {
                    Player.AddBuff(armletToggled, 9999999);
                    armletTimer += 1;
                    SoundEngine.PlaySound(ArmletOnSound, Player.Center);
                } else if (armletTimer >= armletTimerMax)
                {
                    Player.ClearBuff(armletToggled);
                    armletTimer = 0;
                    SoundEngine.PlaySound(ArmletOffSound, Player.Center);
                }
            }

            if (!armletActive)
            {
                Player.ClearBuff(armletToggled);
                armletTimer = 0;
            }
            while (armletTimer >= 1 && armletTimer <= armletTimerMax + 1)
                armletTimer += 1;

            // null timer
            if (KeybindSystem.NulledKeybind.JustPressed && wearingNihilum)
            {
                if (nihilumTimer <= nihilumTimerMax)
                {
                    Player.AddBuff(nihilumToggled, 9999999);
                    nihilumTimer++;
                    SoundEngine.PlaySound(ArmletOnSound with { Pitch = -0.15f }, Player.Center);
                }
                else if (nihilumTimer >= nihilumTimerMax)
                {
                    Player.ClearBuff(nihilumToggled);
                    nihilumTimer = 0;
                    SoundEngine.PlaySound(ArmletOffSound with { Pitch = -0.15f }, Player.Center);
                }
            }

            if (!wearingNihilum)
            {
                Player.ClearBuff(nihilumToggled);
                nihilumTimer = 0;
            }
            while (nihilumTimer >= 1 && nihilumTimer <= nihilumTimerMax + 1)
                nihilumTimer++;

            // power treads
            if (KeybindSystem.BootsKeybind.JustPressed && wearingPowerTreads)
            {
                PowerTreads.CurrentStats++;
                if (PowerTreads.CurrentStats > 2)
                    PowerTreads.CurrentStats = 0;
                SoundEngine.PlaySound(SoundID.Item45, Player.Center);
                //Item.NetStateChanged();
            }

            // triton
            if (KeybindSystem.RigKeybind.JustPressed && wearingTriton)
            {
                Main.playerInventory = true;
                tritonActive = !tritonActive;
            }

            // zhuk
            if (KeybindSystem.RigKeybind.JustPressed && wearingZhuk)
            {
                Main.playerInventory = true;
                zhukActive = !zhukActive;
            }
            #endregion
        }

        #region Miscelanious Effects (spelt right)

        public override void PreUpdate()
        {
            if (infiniteFlight)
                Player.wingTime = Player.wingTimeMax;
            // Syncing mouse controls
            if (Main.myPlayer == Player.whoAmI)
            {
                mouseRight = PlayerInput.Triggers.Current.MouseRight;
                var worldPos = LockOnHelper.Enabled ? LockOnHelper.PredictedPosition : Main.MouseWorld;
                mouseWorldDeltaFromPlayer = worldPos - Player.MountedCenter;
                mouseRotationFromPlayer = mouseWorldDeltaFromPlayer.ToRotation();

                if (rightClickListener && mouseRight != oldMouseRight)
                {
                    oldMouseRight = mouseRight;
                    syncMouseRightClick = true;
                    rightClickListener = false;
                }

                if (mouseWorldListener && Vector2.Distance(mouseWorldDeltaFromPlayer, oldMouseWorldDeltaFromPlayer) > 5f)
                {
                    oldMouseWorldDeltaFromPlayer = mouseWorldDeltaFromPlayer;
                    syncMousePosition = true;
                    mouseWorldListener = false;
                }

                if (mouseRotationListener && Math.Abs(mouseWorldDeltaFromPlayer.ToRotation() - (oldMouseWorldDeltaFromPlayer).ToRotation()) > 0.15f)
                {
                    oldMouseWorldDeltaFromPlayer = mouseWorldDeltaFromPlayer;
                    syncMouseRotation = true;
                    mouseRotationListener = false;
                }
            }
        }
        // force staff movement
        public override void PreUpdateMovement()
        {
            #region Fae Dash
            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            if (wearingFaeArmor)
            {
                if (CanUseDash() && DashDir != -1 && FaeDashDelay == 0)
                {
                    Vector2 newVelocity = Player.velocity;

                    switch (DashDir)
                    {
                        // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                        case DashUp when Player.velocity.Y > -FaeDashVelocity && canDashUp:
                        case DashDown when Player.velocity.Y < FaeDashVelocity:
                            {
                                // Y-velocity is set here
                                // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                                // This adjustment is roughly 1.3x the intended dash velocity
                                canDashUp = false;
                                float dashDirection = DashDir == DashDown ? 1 : -1f;
                                newVelocity.Y = dashDirection * FaeDashVelocity;
                                break;
                            }
                        case DashLeft when Player.velocity.X > -FaeDashVelocity:
                        case DashRight when Player.velocity.X < FaeDashVelocity:
                            {
                                // X-velocity is set here
                                float dashDirection = DashDir == DashRight ? 1 : -1;
                                newVelocity.X = dashDirection * FaeDashVelocity;
                                break;
                            }
                        default:
                            return; // not moving fast enough, so don't start our dash
                    }

                    // start our dash
                    FaeDashDelay = Main.zenithWorld ? 0 : FaeDashCooldown;
                    FaeDashTimer = FaeDashDuration;
                    Player.velocity = newVelocity;

                    // Here you'd be able to set an effect that happens when the dash first activates
                    // Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
                    float dustLoopcheck = 16f;
                    int dustIncr = 0;
                    while (dustIncr < dustLoopcheck)
                    {
                        Vector2 dustRotate = Vector2.UnitX * 0f;
                        dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                        dustRotate = dustRotate.RotatedBy((double)Player.velocity.ToRotation(), default);
                        int bedman = Dust.NewDust(Player.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.LightPink, 1f);
                        Main.dust[bedman].scale = 1.5f;
                        Main.dust[bedman].noGravity = true;
                        Main.dust[bedman].position = Player.Center + dustRotate;
                        Main.dust[bedman].velocity = Player.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                        dustIncr++;
                    }
                }

                if (FaeDashDelay > 0)
                    FaeDashDelay--;

                if (FaeDashTimer > 0)
                {
                    // dash is active
                    // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
                    // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
                    // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
                    Player.eocDash = FaeDashTimer;
                    Player.armorEffectDrawShadowEOCShield = true;

                    // count down frames remaining
                    FaeDashTimer--;

                    // dash dust effects
                    for (int d = 0; d < 4; d++)
                    {
                        Dust faeDust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Player.velocity * 1.2f), DustID.EnchantedNightcrawler, -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        faeDust.noGravity = faeDust.type == 222 ? false : true;
                        faeDust.fadeIn = 0.5f;
                        faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);

                        Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(6, 6) - Player.velocity * 2, DustID.CrystalPulse2);
                        dust.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        dust.noGravity = true;
                    }
                }
            }
            #endregion

            #region Aegis Dash
            if (wearingSacrosanctAegis)
            {
                if (CanUseDash() && DashDir != -1 && AegisDashDelay == 0)
                {
                    Vector2 newVelocity = Player.velocity;

                    switch (DashDir)
                    {
                        case DashUp when Player.velocity.Y > -AegisDashVelocity && canDashUp:
                        case DashDown when Player.velocity.Y < AegisDashVelocity:
                            {
                                canDashUp = false;
                                float dashDirection = DashDir == DashDown ? 1 : -1f;
                                newVelocity.Y = dashDirection * AegisDashVelocity;
                                break;
                            }
                        case DashLeft when Player.velocity.X > -AegisDashVelocity:
                        case DashRight when Player.velocity.X < AegisDashVelocity:
                            {
                                float dashDirection = DashDir == DashRight ? 1 : -1;
                                newVelocity.X = dashDirection * AegisDashVelocity;
                                break;
                            }
                        default:
                            return;
                    }

                    AegisDashDelay = Main.zenithWorld ? 0 : AegisDashCooldown;
                    AegisDashTimer = AegisDashDuration;
                    Player.velocity = newVelocity;

                    // Here you'd be able to set an effect that happens when the dash first activates
                    // Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
                    float dustLoopcheck = 16f;
                    int dustIncr = 0;
                    while (dustIncr < dustLoopcheck)
                    {
                        Vector2 dustRotate = Vector2.UnitX * 0f;
                        dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                        dustRotate = dustRotate.RotatedBy((double)Player.velocity.ToRotation(), default);
                        int bedman = Dust.NewDust(Player.Center, 0, 0, DustID.GoldCoin, 0f, 0f, 0, default, 1f);
                        Main.dust[bedman].scale = 1.5f;
                        Main.dust[bedman].noGravity = true;
                        Main.dust[bedman].position = Player.Center + dustRotate;
                        Main.dust[bedman].velocity = Player.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                        dustIncr++;
                    }
                }

                if (AegisDashDelay > 0)
                    AegisDashDelay--;

                if (AegisDashTimer > 0)
                {
                    Player.SetImmuneTimeForAllTypes(AegisDashTimer);
                    Player.eocDash = AegisDashTimer;
                    Player.armorEffectDrawShadowEOCShield = true;

                    // count down frames remaining
                    AegisDashTimer--;

                    // dash dust effects
                    for (int d = 0; d < 4; d++)
                    {
                        Dust faeDust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Player.velocity * 1.2f), DustID.HallowSpray, -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        faeDust.noGravity = faeDust.type == 222 ? false : true;
                        faeDust.fadeIn = 0.5f;
                        faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);

                        Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(6, 6) - Player.velocity * 2, DustID.GoldCoin);
                        dust.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        dust.noGravity = true;
                    }
                }
            }
            #endregion

            #region Force Staff (Old)
            // if force staff isn't on cooldown and was equipped and player just pressed keybind
            if (wearingForceStaff && !Player.mount.Active &&  KeybindSystem.ForceStaffKeybind.JustPressed && !Player.HasBuff(forceStaffCooldown))
            {
                // change to force staff sound
                SoundEngine.PlaySound(ForceStaffActivateSound, Player.Center);
                Vector2 newVelocity = Player.velocity;

                switch (forceDirection)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case DashUp when Player.velocity.Y > -ForceDashVelocity:
                    case DashDown when Player.velocity.Y < ForceDashVelocity:
                            {
                            // Y-velocity is set here
                            // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                            // This adjustment is roughly 1.3x the intended dash velocity
                            float dashDirection = forceDirection == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * ForceDashVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -ForceDashVelocity:
                    case DashRight when Player.velocity.X < ForceDashVelocity:
                        {
                            // X-velocity is set here
                            float dashDirection = forceDirection == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * ForceDashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                }
                int bufftime = Main.zenithWorld ? 0 : 180;
                Player.velocity = newVelocity;
                Player.AddBuff(forceStaffCooldown, bufftime);
                Player.statMana -= 50;
                Player.ManaEffect(-50);

                ForceDashTimer = ForceDashDuration;
                float dustLoopcheck = 16f;
                int dustIncr = 0;
                while (dustIncr < dustLoopcheck)
                {
                    Vector2 dustRotate = Vector2.UnitX * 0f;
                    dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                    dustRotate = dustRotate.RotatedBy((double)Player.velocity.ToRotation(), default);
                    int bedman = Dust.NewDust(Player.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.LightGreen, 1f);
                    Main.dust[bedman].scale = 1.5f;
                    Main.dust[bedman].noGravity = true;
                    Main.dust[bedman].position = Player.Center + dustRotate;
                    Main.dust[bedman].velocity = Player.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                    dustIncr++;
                }
            }
            if (ForceDashTimer > 0)
            {
                Player.eocDash = ForceDashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                ForceDashTimer--;
                for (int d = 0; d < 4; d++)
                {
                    Dust faeDust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Player.velocity * 1.2f), DustID.Terra, -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                    faeDust.noGravity = faeDust.type == 222 ? false : true;
                    faeDust.fadeIn = 0.5f;
                    faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                    Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(6, 6) - Player.velocity * 2, DustID.GemEmerald);
                    dust.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                    dust.noGravity = true;
                }
            }
            #endregion

            #region Force Staff
            /*
            if (wearingForceStaff)
            {
                if (CanUseDash() && DashDir != -1 && ForceDashDelay == 0)
                {
                    Vector2 newVelocity = Player.velocity;
                    switch (DashDir)
                    {
                        case DashUp when Player.velocity.Y > -ForceDashVelocity && canDashUp:
                        case DashDown when Player.velocity.Y < ForceDashVelocity:
                            {
                                canDashUp = false;
                                float dashDirection = DashDir == DashDown ? 1 : -1f;
                                newVelocity.Y = dashDirection * ForceDashVelocity;
                                break;
                            }
                        case DashLeft when Player.velocity.X > -ForceDashVelocity:
                        case DashRight when Player.velocity.X < ForceDashVelocity:
                            {
                                float dashDirection = DashDir == DashRight ? 1 : -1;
                                newVelocity.X = dashDirection * ForceDashVelocity;
                                break;
                            }
                        default:
                            return;
                    }
                    ForceDashTimer = ForceDashDuration;
                    Player.velocity = newVelocity;
                    float dustLoopcheck = 16f;
                    int dustIncr = 0;
                    while (dustIncr < dustLoopcheck)
                    {
                        Vector2 dustRotate = Vector2.UnitX * 0f;
                        dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                        dustRotate = dustRotate.RotatedBy((double)Player.velocity.ToRotation(), default);
                        int bedman = Dust.NewDust(Player.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.LightGreen, 1f);
                        Main.dust[bedman].scale = 1.5f;
                        Main.dust[bedman].noGravity = true;
                        Main.dust[bedman].position = Player.Center + dustRotate;
                        Main.dust[bedman].velocity = Player.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                        dustIncr++;
                    }
                }
                if (FaeDashTimer > 0)
                {
                    Player.eocDash = FaeDashTimer;
                    Player.armorEffectDrawShadowEOCShield = true;
                    FaeDashTimer--;
                    for (int d = 0; d < 4; d++)
                    {
                        Dust faeDust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Player.velocity * 1.2f), DustID.Terra, -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        faeDust.noGravity = faeDust.type == 222 ? false : true;
                        faeDust.fadeIn = 0.5f;
                        faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                        Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(6, 6) - Player.velocity * 2, DustID.GemEmerald);
                        dust.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        dust.noGravity = true;
                    }
                }
            }
            */
            #endregion

            #region Hurricane Pike (Old)
            if (wearingPike && !Player.mount.Active && KeybindSystem.ForceStaffKeybind.JustPressed && !Player.HasBuff(forceStaffCooldown))
            {
                SoundEngine.PlaySound(ForceStaffActivateSound, Player.Center);
                Vector2 newVelocity = Player.velocity;

                switch (forceDirection)
                {
                    case DashUp when Player.velocity.Y > -PikeDashVelocity:
                    case DashDown when Player.velocity.Y < PikeDashVelocity:
                        {
                            float dashDirection = forceDirection == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * PikeDashVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -PikeDashVelocity:
                    case DashRight when Player.velocity.X < PikeDashVelocity:
                        {
                            float dashDirection = forceDirection == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * PikeDashVelocity;
                            break;
                        }
                    default:
                        return;
                }
                int bufftime = Main.zenithWorld ? 0 : 180;
                Player.velocity = newVelocity;
                Player.AddBuff(forceStaffCooldown, bufftime);
                Player.statMana -= 50;
                Player.ManaEffect(-50);

                PikeDashTimer = PikeDashDuration;
                float dustLoopcheck = 16f;
                int dustIncr = 0;
                while (dustIncr < dustLoopcheck)
                {
                    Vector2 dustRotate = Vector2.UnitX * 0f;
                    dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                    dustRotate = dustRotate.RotatedBy((double)Player.velocity.ToRotation(), default);
                    int bedman = Dust.NewDust(Player.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.LightYellow, 1f);
                    Main.dust[bedman].scale = 1.5f;
                    Main.dust[bedman].noGravity = true;
                    Main.dust[bedman].position = Player.Center + dustRotate;
                    Main.dust[bedman].velocity = Player.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                    dustIncr++;
                }
            }
            if (PikeDashTimer > 0)
            {
                Player.eocDash = PikeDashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                PikeDashTimer--;
                for (int d = 0; d < 4; d++)
                {
                    Dust faeDust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Player.velocity * 1.2f), DustID.Sandnado, -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                    faeDust.noGravity = faeDust.type == 222 ? false : true;
                    faeDust.fadeIn = 0.5f;
                    faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                    Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(6, 6) - Player.velocity * 2, DustID.YellowStarDust);
                    dust.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                    dust.noGravity = true;
                }
            }
            #endregion

            #region Hurricane Pike
            /*
            if (wearingPike)
            {
                if (CanUseDash() && DashDir != -1 && FaeDashDelay == 0)
                {
                    Vector2 newVelocity = Player.velocity;
                    switch (DashDir)
                    {
                        case DashUp when Player.velocity.Y > -PikeDashVelocity && canDashUp:
                        case DashDown when Player.velocity.Y < PikeDashVelocity:
                            {
                                canDashUp = false;
                                float dashDirection = DashDir == DashDown ? 1 : -1f;
                                newVelocity.Y = dashDirection * PikeDashVelocity;
                                break;
                            }
                        case DashLeft when Player.velocity.X > -PikeDashVelocity:
                        case DashRight when Player.velocity.X < PikeDashVelocity:
                            {
                                float dashDirection = DashDir == DashRight ? 1 : -1;
                                newVelocity.X = dashDirection * PikeDashVelocity;
                                break;
                            }
                        default:
                            return;
                    }
                    FaeDashDelay = PikeDashCooldown;
                    FaeDashTimer = PikeDashDuration;
                    Player.velocity = newVelocity;

                }
                if (FaeDashDelay > 0)
                    FaeDashDelay--;
                if (FaeDashTimer > 0)
                {
                    Player.eocDash = FaeDashTimer;
                    Player.armorEffectDrawShadowEOCShield = true;
                    FaeDashTimer--;
                    for (int d = 0; d < 4; d++)
                    {
                        Dust faeDust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Player.velocity * 1.2f), DustID.Sandnado, -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        faeDust.noGravity = faeDust.type == 222 ? false : true;
                        faeDust.fadeIn = 0.5f;
                        faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                        Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(6, 6) - Player.velocity * 2, DustID.YellowStarDust);
                        dust.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        dust.noGravity = true;
                    }
                }
            }
            */
            #endregion
        }
        private bool CanUseDash()
        {
            return !chargeShot
                && !dpCharge
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }
        public void MiscEffects()
        {
            #region Summon Accessories
            // checks if the player is wearing accessory, and if true, stops previous iterations of the accessory from benefitting the player
            if (overlordMinion)
            {
                Player.maxMinions += 3;
                Player.maxTurrets += 3;
            }
            else
            {
                if (dominatorMinion)
                {
                    Player.maxMinions += 2;
                    Player.maxTurrets += 2;
                }
                else
                    if (diademMinion)
                        Player.maxMinions++;
            }
            if (locketActive)
                Player.maxMinions += 2;
            if (wearingWraithPact)
                Player.maxTurrets += WraithPact.MaxSentries;
            else
                if (wearingVladimirs)
                    Player.maxTurrets += VladmirsOffering.MaxSentries;
            #endregion

            #region Weapon Buffs
            // essence shift stacking buff
            if (Player.HasBuff<EssenceShift>() && (Player.HeldItem.type == ModContent.ItemType<HydrakanLatch>() || Player.HeldItem.type == ModContent.ItemType<GoldenHydrakanLatch>() || Player.HeldItem.type == ModContent.ItemType<Megaslark>() || Player.HeldItem.type == ModContent.ItemType<Minislark>()))
            {
                float GFBMult = Main.zenithWorld ? 2f : 1f;
                if (Player.HeldItem.type == ModContent.ItemType<HydrakanLatch>() || Player.HeldItem.type == ModContent.ItemType<GoldenHydrakanLatch>())
                {
                    if (essenceShiftLevel > essenceShiftLevelMax)
                        essenceShiftLevel = essenceShiftLevelMax;
                    Player.GetAttackSpeed<MeleeDamageClass>() += (0.075f * GFBMult) * essenceShiftLevel;
                    Player.moveSpeed += (0.0125f * GFBMult) * essenceShiftLevel;
                    Player.accRunSpeed += Player.accRunSpeed * (0.0125f * GFBMult) * essenceShiftLevel;
                }
                if (Player.HeldItem.type == ModContent.ItemType<Megaslark>())
                {
                    if (essenceShiftLevel > essenceShiftLevelMax)
                        essenceShiftLevel = essenceShiftLevelMax;
                    Player.GetAttackSpeed(DamageClass.Ranged) += (0.1f * GFBMult) * essenceShiftLevel;
                    Player.GetArmorPenetration(DamageClass.Ranged) += essenceShiftLevel;
                    Player.moveSpeed += (0.025f * GFBMult) * essenceShiftLevel;
                    Player.accRunSpeed += Player.accRunSpeed * (0.025f * GFBMult) * essenceShiftLevel;
                }
                if (Player.HeldItem.type == ModContent.ItemType<Minislark>())
                {
                    if (essenceShiftLevel > essenceShiftLevelMax)
                        essenceShiftLevel = essenceShiftLevelMax;
                    Player.GetAttackSpeed(DamageClass.Ranged) += (0.05f * GFBMult) * essenceShiftLevel;
                    Player.GetArmorPenetration(DamageClass.Ranged) += (float)essenceShiftLevel / 3;
                    Player.moveSpeed += (0.015f * GFBMult) * essenceShiftLevel;
                    Player.accRunSpeed += Player.accRunSpeed * (0.015f * GFBMult) * essenceShiftLevel;
                }
            }
            else
            {
                essenceShiftLevel = 0;
                Player.ClearBuff(ModContent.BuffType<EssenceShift>());
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                    SyncEssenceShift(false);
            }
            if (Player.HeldItem.type == ModContent.ItemType<ThrowingShade>() || Player.HeldItem.type == ModContent.ItemType<ShadowRealm>())
            {
                float GFBMult = Main.zenithWorld ? 5f : 1f;
                if (Player.HeldItem.type == ModContent.ItemType<ThrowingShade>())
                    holdingThrowingShade = true;
                if ((shadowRealmLevel < (int)(shadowRealmLevelMax * GFBMult)) && Player.HasBuff<ShadowRealmBuff>())
                {
                    shadowRealmLevel++;
                    if (holdingThrowingShade)
                        shadowRealmLevel++;
                }
                if (shadowRealmLevel == (int)(shadowRealmLevelMax * GFBMult))
                {
                    shadowRealmLevel = (int)(shadowRealmLevelMax * GFBMult) + 1;
                    SoundEngine.PlaySound(SoundID.Item104, Player.Center); // might change to something else
                }
                if (!Player.HasBuff<ShadowRealmBuff>())
                    shadowRealmLevel = 0;
            }
            else
            {
                shadowRealmLevel = 0;
                Player.ClearBuff(ModContent.BuffType<ShadowRealmBuff>());
            }

            // fiery soul stacking buff
            if (Player.HasBuff<FierySoulStack>())
            {
                if (fierySoulLevel > fierySoulLevelMax)
                {
                    fierySoulLevel = fierySoulLevelMax;
                }
                Player.GetAttackSpeed<MagicDamageClass>() += .015f * fierySoulLevel;
                Player.manaCost -= .015f * fierySoulLevel;
                Player.moveSpeed += .0225f * fierySoulLevel;
                Player.accRunSpeed += Player.accRunSpeed * .0225f * fierySoulLevel;
            }
            else
            {
                fierySoulLevel = 0;
            }

            if (Player.HeldItem.type == ModContent.ItemType<Butterfly>())
            {
                doButterfly(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncButterfly(false); //TODO: Add a timer so this doesn't sync every tick
                }
            }

            if (Player.HeldItem.type == ModContent.ItemType<MG43MachineGun>())
                if (MG43MachineGun.rpm <= 2)
                {
                    if (Main.zenithWorld && MG43MachineGun.rpm > 1)
                        Player.GetAttackSpeed(DamageClass.Ranged) *= 0.05f;
                    else
                        Player.GetAttackSpeed(DamageClass.Ranged) += 0.2f;
                    if (MG43MachineGun.rpm <= 1)
                        Player.GetAttackSpeed(DamageClass.Ranged) += Main.zenithWorld ? 4f : 0.2f;
                }

            // more mines if holding techies mines
            if (Player.HeldItem.type == ModContent.ItemType<ProximityMines>() || Player.HeldItem.type == ModContent.ItemType<MADMine>())
                Player.maxTurrets += Main.zenithWorld ? 27: 2;

            // duelist gloves
            if (wearingDuelistGloves)
            {
                doDuelistGloves(Player, Player.Center); //Does the thing.
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncDuelistGloves(false, Player.Center); //Netcode stuff, go to MogPlayerNetcode.cs to see what this does.
                }
            }

            if (Player.HasBuff<DragonInstallBuff>() && wearingFlameOfCorruption)
            {
                enterDragonInstall(Player);
            }
            else
            {
                exitDragonInstall(Player);
            }

            if (!wearingFlameOfCorruption && Player.HasBuff<DragonInstallBuff>())
            {
                Player.ClearBuff(ModContent.BuffType<DragonInstallBuff>());
            }
            #endregion

            #region Wing Time Buffs
            // Flight time boosts
            double flightTimeMult = 1D +
                (wearingFaeArmor ? FaeMask.FlightTimeBoost : 
                wearingTreadsBuilding ? PowerTreads.FlightTimeBoost : 0D);

            if (Player.wingTimeMax > 0)
                Player.wingTimeMax = (int)(Player.wingTimeMax * flightTimeMult);
            #endregion

            #region Revives
            if (seraphicReviveCounter == SeraphicBreastplate.ReviveCooldown)
            {
                for (int i = 0; i < 80; i++)
                {
                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 3;

                    int dustPos = 10;
                    int seraphic = Dust.NewDust(Player.Center, dustPos, dustPos, Main.rand.NextBool(3) ? 180 : 178, dustVelocity.X * 2, dustVelocity.Y * 2, 0, Color.White, 9f);
                    Main.dust[seraphic].noGravity = true;
                    Main.dust[seraphic].fadeIn = 5f;
                    Main.dust[seraphic].velocity *= 3f;
                }
            }
            #endregion
        }
        public override void PostUpdateMiscEffects()
        {
            MiscEffects();
            OtherBuffEffects();
            CheckIfMouseItemIsSchematic();

            // Regularly sync player stats & mouse control info during multiplayer
            if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (syncMouseRightClick)
                {
                    syncMouseRightClick = false;
                    MouseRightClickSync();
                }

                mouseWorldPacketTimer = Math.Min(mouseWorldPacketTimer + 1, MouseWorldPacketInterval);
                if (mouseWorldPacketTimer >= MouseWorldPacketInterval)
                {
                    if (syncMousePosition)
                    {
                        mouseWorldPacketTimer = 0;
                        syncMousePosition = false;
                        syncMouseRotation = false; // Rotation also get update on position packet
                        MousePositionSync();
                    }

                    if (syncMouseRotation)
                    {
                        mouseWorldPacketTimer = 0;
                        syncMouseRotation = false;
                        MouseRotationSync();
                    }
                }
            }
        }
        public void CheckIfMouseItemIsSchematic()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            bool shouldSync = false;

            // ActiveItem doesn't need to be checked as the other possibility involves
            // the item in question already being in the inventory.
            if (Main.mouseItem != null && !Main.mouseItem.IsAir)
            {
                if (Main.mouseItem.type == ModContent.ItemType<GiantsMaul>() && !MogModWorld.HasFoundGiantsMaul)
                {
                    MogModWorld.HasFoundGiantsMaul = true;
                    shouldSync = true;
                }
            }

            if (shouldSync)
                MogModNetcode.SyncWorld();
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            string targetName = "Joise Stain";
            string targetName2 = "Balright Monster";
            string targetName3 = "Balright";
            string targetName4 = "Jpoel";
            string targetName5 = "SenorDragon";
            string targetName6 = "wPopOff";
            string targetName7 = "wPoopButt";
            static Item createItem(int type)
            {
                Item i = new Item();
                i.SetDefaults(type);
                return i;
            }

            // so you dont get these items on respawn in mediumcore
            if (!mediumCoreDeath)
            {
                yield return createItem(ModContent.ItemType<VonWarning>());
                if (Player.name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return createItem(ModContent.ItemType<Phasma>());
                    yield return createItem(ModContent.ItemType<BizarreMusicBox>());
                }
                if (Player.name.Equals(targetName2, StringComparison.OrdinalIgnoreCase))
                {
                    yield return createItem(ModContent.ItemType<TheDeck>());
                    yield return createItem(ModContent.ItemType<VonEvilIncarnateMusicBox>());
                }
                if (Player.name.Equals(targetName3, StringComparison.OrdinalIgnoreCase))
                {
                    yield return createItem(ModContent.ItemType<TheDeck>());
                    yield return createItem(ModContent.ItemType<KingVonMusicBox>());
                }
                if (Player.name.Equals(targetName4, StringComparison.OrdinalIgnoreCase) || Player.name.Equals(targetName5, StringComparison.OrdinalIgnoreCase))
                {
                    yield return createItem(ModContent.ItemType<ProximityMines>());
                    yield return createItem(ItemID.GenderChangePotion);
                }
                if (Player.name.Equals(targetName6, StringComparison.OrdinalIgnoreCase) || Player.name.Equals(targetName7, StringComparison.OrdinalIgnoreCase))
                {
                    yield return createItem(ModContent.ItemType<BizarreMusicBox>());
                    yield return createItem(ModContent.ItemType<VonEvilIncarnateMusicBox>());
                    yield return createItem(ModContent.ItemType<KingVonMusicBox>());
                }
            }
        }
        public override void PostUpdate()
        {
            if (Player.velocity.Y == Player.oldVelocity.Y)
                canDashUp = true;
            // if the player is wearing shadow amulet turn them invis after a set amount of time
            if (wearingShadowAmulet)
            {
                if (Player.velocity.X == 0f && Player.velocity.Y == 0f)
                {
                    // count down timer
                    shadowTimer++;
                    // give the player a buff when the timer is at max
                    if (shadowTimer >= shadowTimerMax)
                        Player.AddBuff(ModContent.BuffType<ShadowAmuletBuff>(), 2);
                    // dust effect
                    if (shadowAmuletVisual && shadowTimer < (shadowTimerMax / 2))
                    {
                        if (Main.rand.NextBool(2))
                        {
                            int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? 164 : 177, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 1f);
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
                    // stronger dust effect when halfway done
                    else if (shadowAmuletVisual && shadowTimer > (shadowTimerMax / 2) && shadowTimer < shadowTimerMax)
                    {
                        for (int n = 0; n < 2; n++)
                        {
                            int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? 164 : 177, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 1.3f);
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
                    // final BOOM
                    else if (shadowTimer == shadowTimerMax)
                    {
                        SoundEngine.PlaySound(SoundID.Item68, Player.Center);
                        if (shadowAmuletVisual)
                        {
                            for (int i = 0; i < 40; i++)
                            {
                                int strike = Dust.NewDust(Player.position - new Vector2(2f), Player.width * 2, Player.height * 2, Main.rand.NextBool(3) ? 164 : 177, 0, 0, 100, default, 2f);
                                Main.dust[strike].velocity.Y *= 1.05f;
                                Main.dust[strike].noGravity = true;
                            }
                        }
                    }
                }
                else if (Player.velocity.X != 0f || Player.velocity.Y != 0f)
                    shadowTimer = 0;
            }
            else
                shadowTimer = 0;
            if (wearingRuntyHorseshoe)
            {
                // code taken from calamity mods wulfrum acrobatics pack
                Vector2 checkedPlayerPosition = Player.position;
                bool imminentDanger = false;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 collisionVector = Collision.TileCollision(checkedPlayerPosition, Player.velocity, Player.width, Player.height, gravDir: (int)Player.gravDir);
                    if (collisionVector.Y < Player.velocity.Y)
                    {
                        imminentDanger = true;
                        checkedPlayerPosition += collisionVector;
                        //Main.NewText("player will die!", 255, 25, 24);
                        break;
                    }
                    checkedPlayerPosition += collisionVector;
                }

                int fallDistance = (int)(checkedPlayerPosition.Y / 16f) - Player.fallStart;
                int fallDmgThreshold = 5 + Player.extraFall;

                if (!imminentDanger)
                {
                    fallDamageTimer++;
                    if (fallDamageTimer == 5)
                    {
                        //Main.NewText("player can die!", 50, 55, 124);
                        stopFallDamage = false;
                    }
                    return;
                }

                if (fallDistance * Player.gravDir > fallDmgThreshold)
                {
                    fallDamageTimer = 0;
                    //Main.NewText("player died!", 155, 25, 24);
                    stopFallDamage = true;
                }
            }
            if (eSeraphCharge >= eSeraphMax)
            {
                if (!eSeraphSound)
                {
                    SoundEngine.PlaySound(SoundID.Item109, Player.Center);
                    eSeraphSound = true;
                }
                eSeraphCharge++;
                for (int n = 0; n < 2; n++)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? DustID.HallowSpray : DustID.YellowStarDust, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 1.3f);
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
            else
                eSeraphSound = false;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow != 0f || Player.dead)
                return;
            if (divineDebuff)
                DivineMightDebuff.DrawEffects(drawInfo);
            if (skadiDebuff)
                EyeOfSkadiDebuff.DrawEffects(drawInfo);
            if (freezingDebuff)
                FreezingDebuff.DrawEffects(drawInfo);
            if (aghHexDebuff)
                AghanimHexDebuff.DrawEffects(drawInfo);
            if (wingsOfLightDebuff)
                WingsOfLightDebuff.DrawEffects(drawInfo);
            if (ghostflameDebuff)
                GhostflameDebuff.DrawEffects(drawInfo);
            if (jidiDebuff)
                JidiPollenBagDebuff.DrawEffects(drawInfo);
            if (shivaDebuff)
                ShivasEnemyDebuff.DrawEffects(drawInfo);
            if (infernoDebuff)
                InfernoDebuff.DrawEffects(drawInfo);
            if (blazingDebuff)
                BlazingDebuff.DrawEffects(drawInfo);
            if (toxicDebuff)
                ToxicDebuff.DrawEffects(drawInfo);
            if (deathDebuff)
                BlackBladeDebuff.DrawEffects(drawInfo);
            if (healingDisabledDebuff)
                HealingDisabledDebuff.DrawEffects(drawInfo);

            float dim = .01f;
            if (wearingOverloading && overloadingVisual)
            {
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? 161 : DustID.MagnetSphere, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 2.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.65f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
                Lighting.AddLight(Player.Center, 49f * dim, 174f * dim, 230f * dim);
            }
            if (wearingBlazing && blazingVisual)
            {
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? DustID.Lava : DustID.Flare, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 2.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.65f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
                Lighting.AddLight(Player.Center, 255f * dim, 84f * dim, 24f * dim);
            }
            if (wearingGilded && gildedVisual)
            {
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? DustID.GoldCoin : DustID.Enchanted_Gold, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 2.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.65f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
                Lighting.AddLight(Player.Center, 255f * dim, 234f * dim, 29f * dim);
            }
            if (wearingMending && mendingVisual)
            {
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? DustID.Terra : DustID.PoisonStaff, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 2.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.65f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
                Lighting.AddLight(Player.Center, 114f * dim, 230f * dim, 49f * dim);
            }
            if (wearingToxic && toxicVisual)
            {
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? DustID.Venom : DustID.Poisoned, Player.velocity.X * 0.04f, Player.velocity.Y * 0.04f, 100, default, 2.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.65f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
                Lighting.AddLight(Player.Center, 219f * dim, 47f * dim, 237f * dim);
            }
        }
        // shivas effect and dust;
        public void doShivas(Player player, Vector2 center) //This needs to be its own method for netcode to work. See how I did it in MogModNetcode.cs and MogPlayerNetcode.cs
        {
            for (int i = 0; i < Main.maxNPCs; i++) //Every npc is in an index, this goes through all of them
            {
                NPC otherNPC = Main.npc[i]; //This sets the var otherNPC to the current npc we are targeting in the index
                if (otherNPC.active && otherNPC.townNPC == false && otherNPC.whoAmI != otherNPC.whoAmI - 1) //Makes shivas not hit inactive npcs, townNpcs, and not cast on the same npc twice.
                {
                    if (Microsoft.Xna.Framework.Vector2.Distance(center, otherNPC.Center) < 1200f)
                    {
                        var hitInfo = new NPC.HitInfo //Hit info used in otherNPC.StrikeNPC(hitInfo)
                        {
                            Damage = 200,
                            Knockback = 0,
                            HitDirection = Player.direction,
                            Crit = false,
                            DamageType = DamageClass.Generic
                        };
                        otherNPC.StrikeNPC(hitInfo); //Must use this instead of modifying the npc's life stat
                        NetMessage.SendStrikeNPC(otherNPC, hitInfo); //Vital for sending the hit to other clients (stops desync)
                        otherNPC.AddBuff(ModContent.BuffType<ShivasEnemyDebuff>(), 1800); //Removes 15% of enemy's defense (rounded)
                    }
                }
            }

            Player.AddBuff(ModContent.BuffType<ShivasDebuff>(), 3600);
            SoundEngine.PlaySound(ShivasActivateSound, center);

            for (int i = 0; i < 80; i++)
            {
                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustVelocity.Normalize();
                dustVelocity *= 6;

                int dustPos = 20;

                int shiva1 = Dust.NewDust(center, dustPos, dustPos, DustID.SnowSpray, dustVelocity.X * 3, dustVelocity.Y * 3, 0, default, 3f);
                Main.dust[shiva1].noGravity = true;
                Main.dust[shiva1].fadeIn = 5f;
                Main.dust[shiva1].velocity *= 3f;
                int shiva2 = Dust.NewDust(center, dustPos - 5, dustPos - 5, DustID.Snow, dustVelocity.X * 2, dustVelocity.Y * 2, 0, Color.White, 9f);
                Main.dust[shiva2].noGravity = true;
                Main.dust[shiva2].fadeIn = 5f;
                Main.dust[shiva2].velocity *= 3f;
            }
        }

        // wings of light effect and dust;
        public void doWingsOfLight(Player player, Vector2 center) // refer to shivas for how this works
        {
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC otherNPC = Main.npc[n];
                if (otherNPC.active && otherNPC.friendly == false && otherNPC.whoAmI != otherNPC.whoAmI - 1) // changed .townNPC to .friendly so it doesnt von out birds;
                {
                    if (Microsoft.Xna.Framework.Vector2.Distance(center, otherNPC.Center) < 180f)
                    {
                        otherNPC.AddBuff(ModContent.BuffType<WingsOfLightDebuff>(), 60);
                    }
                }
            }
            if (wingsOfLightVisual)
            {
                int wolSize = 64;
                wingsOfLightDust += 1;

                // ambient dust effect;
                if (Main.rand.NextBool())
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, DustID.GoldCoin, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                }

                // TODO; make this a spiral effect and give a unique debuff;
                Vector2 offset = Vector2.UnitX * 0f;
                offset += -Vector2.UnitY.RotatedBy((double)((float)wingsOfLightDust * (MathHelper.TwoPi / wolSize)), default) * new Vector2(158f, 30f);
                int dust2 = Dust.NewDust(player.Center, 0, 0, DustID.GoldCritter_LessOutline, 0f, 0f, 0, default, 1f);
                Main.dust[dust2].scale = 1.5f;
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].position = player.Center + offset;
                Main.dust[dust2].velocity = player.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;

                Vector2 value7 = new Vector2(5f, 10f);
                //Vector2 offset2 = Vector2.UnitX * -12f;
                //offset2 = -Vector2.UnitY.RotatedBy((double)(wingsOfLightDust * 0.1308997f + (float)wingsOfLightDust * 3.14159274f), default) * value7 - Vector2.UnitY.RotatedBy((double)((float)wingsOfLightDust * (MathHelper.TwoPi / wolSize))) * 10f;
                Vector2 offset2 = Vector2.UnitX * 0f;
                offset2 += -Vector2.UnitY.RotatedBy((double)((float)wingsOfLightDust * (MathHelper.TwoPi / wolSize)), default) * new Vector2(-158f, 30f);
                int dust3 = Dust.NewDust(player.Center, 0, 0, DustID.GoldCritter, 0f, 0f, 0, default, 1f);
                Main.dust[dust3].scale = 1.5f;
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].position = player.Center + offset2;
                Main.dust[dust3].velocity = player.velocity * 0f + offset2.SafeNormalize(Vector2.UnitY) * 1f;
                if (wingsOfLightDust >= wolSize)
                {
                    wingsOfLightDust = 0;
                }
            }
        }

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (item.CountsAsClass<MeleeDamageClass>())
            {
                if (wearingAghGauntlet && aghGauntletVisual)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust aghs = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RainbowMk2, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, Color.BlueViolet, 1.25f);
                        aghs.noGravity = true;
                    }
                }
            }
        }

        // helm of undying
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (wearingUndyingHelm)
                doUndying();
            if (wearingBlazing)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingExplosion>(), 500, 1f, Main.myPlayer, ai2: 1f);
            if (wearingMending)
            {
                NPC healingOrb = NPC.NewNPCDirect(Player.GetSource_FromThis(), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<HealingOrb>(), Player.whoAmI);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: healingOrb.whoAmI);
            }
            seraphicReviveCounter = 0;
            nihilumTimer = 0;
            armletTimer = 0;
            praporCooldown = 0;
            toxicDamage = 0;
        }
        public void doUndying()
        {
            float respawnTime = Main.zenithWorld ? 5f : 0.8f;
            Player.respawnTimer = Convert.ToInt32(Player.respawnTimer * respawnTime);
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<UndyingPortalProj>(), wraithDamage, 1, Player.whoAmI);
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (wearingRuntyHorseshoe && stopFallDamage)
            {
                SoundEngine.PlaySound(SoundID.Item37, Player.Center);
                fallDamageTimer = 0;
                if (Player.statLife < 1)
                    Player.statLife = 1;
                return false;
            }
            if ((wearingSeraphic && seraphicReviveCounter <= 0) || Player.HasBuff(ModContent.BuffType<SeraphicReviveBuff>()))
            {
                if (seraphicReviveCounter <= 0 && !canSeraphicRevive)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Player.Center);
                    Player.AddBuff(ModContent.BuffType<SeraphicReviveBuff>(), SeraphicBreastplate.ReviveDuration);
                    seraphicReviveCounter = SeraphicBreastplate.ReviveCooldown;
                }
                canSeraphicRevive = true;
                if (Player.statLife < 1)
                    Player.statLife = 1;
                return false;
            }
            if (wearingUndyingArmor && !Player.HasBuff(ModContent.BuffType<WraithBuff>()))
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52, Player.Center);
                Player.AddBuff(ModContent.BuffType<WraithBuff>(), 300);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<PlayerUndyingPortalProj>(), wraithDamage, 1, Player.whoAmI);
            }
            if (Player.HasBuff(ModContent.BuffType<WraithBuff>()))
                return false;
            if (wearingSatanic && Main.zenithWorld)
                damageSource = PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.Satanic").ToNetworkText(Player.name));
            if (armletDebuff)
                damageSource = PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.Armlet").ToNetworkText(Player.name));
            if (nulledDebuff)
                damageSource = PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.Nulled").ToNetworkText(Player.name));
            return true;
        }
        #endregion

        #region Player Buffs / Debuffs
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            double damageMult = 1D;
            if (wearingWhisperDread) // increases damage taken
                damageMult += 0.15;
            if (blazingDebuff)
                damageMult += BlazingAspect.DamageMult;
            if (aghHexDebuff)
                damageMult += 0.2;
            if (wingsOfLightDebuff)
                damageMult += 0.1;
            modifiers.SourceDamage *= (float)damageMult;
        }
        public override void PostHurt(Player.HurtInfo hurtinfo)
        {
            Player.ClearBuff(ModContent.BuffType<ClarityBuff>());
            Player.ClearBuff(ModContent.BuffType<HealingSalveBuff>());
            if (wearingOverloading)
                overloadingRegenCooldown = OverloadingAspect.RegenWaitTime;
            if (wearingRefresherOrb && Main.zenithWorld)
            {
                if (hurtinfo.CooldownCounter != -1)
                    Player.hurtCooldowns[hurtinfo.CooldownCounter] = 0;
                Player.immuneTime = 0;
                Player.immune = false;
            }
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            double healingMult = 1D +
                (wearingMending ? MendingAspect.LifeMult : 0D);
            healValue = (int)(healValue * healingMult);
        }
        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            double healingMult = 1D +
                (wearingRadiantArmor ? RadiantFlower.ManaMult : 0D);
            healValue = (int)(healValue * healingMult);
        }
        // sniper offlane scope effect
        public override void ModifyZoom(ref float zoom)
        {
            if (Main.mouseRight)
            {
                if (Player.HeldItem.type == ModContent.ItemType<AXMC>())
                {
                    zoom = Player.scope ? 0.8f : 0.6666667f;
                }
            }
        }
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (wearingAllegianceWings && !Player.mount.Active)
                damage *= ((Player.wingTimeMax - Player.wingTime) / (int)(WingsOfAllegiance.WingTime * 1.5)) + 1;
        }
        public void enterDragonInstall(Terraria.Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.dragonInstallActive = true;
        }
        public void exitDragonInstall (Terraria.Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.dragonInstallActive = false;
        }
        public void doButterfly(Terraria.Player player)
        {
            player.moveSpeed *= 1.30f;
            player.maxRunSpeed *= 1.30f;
            player.accRunSpeed *= 1.30f;
            player.wingAccRunSpeed *= 1.30f;
            player.wingRunAccelerationMult *= 1.30f;
        }
        public void doDuelistGloves(Terraria.Player player, Vector2 center)
        {
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC otherNPC = Main.npc[n];
                if (otherNPC.active && otherNPC.friendly == false && otherNPC.whoAmI != otherNPC.whoAmI - 1 && otherNPC.type != NPCID.TargetDummy)
                {
                    if (Microsoft.Xna.Framework.Vector2.Distance(center, otherNPC.Center) < 300f) // 20 = 1 block (i think)
                    {
                        if (duelistStacks <= maxDuelistStacks)
                            duelistStacks++;
                        else
                            duelistStacks = maxDuelistStacks;
                    }
                }
            }

            for (int i = 0; i < duelistStacks; i++)
                player.GetAttackSpeed<MeleeDamageClass>() += .07f;
        }
        public void doParry(Terraria.Player player, Vector2 pos)
        {
            Player.ClearBuff(ModContent.BuffType<Parrying>());
            Player.ClearBuff(ModContent.BuffType<ParrySlow>());
            Player.ClearBuff(ModContent.BuffType<ParryCooldown>());
            Player.AddBuff(ModContent.BuffType<ParryCooldown>(), 60);
            Player.AddBuff(ModContent.BuffType<ParryBuff1>(), 600);
            
            Player.SetImmuneTimeForAllTypes(35);

            for (int i = 0; i < Player.hurtCooldowns.Length; i++)
            {
                Player.hurtCooldowns[i] = 35;
            }

            if (Player.HeldItem.type == ModContent.ItemType<Moonveil>())
                Moonveil.Charges = Moonveil.MaxCharges;

            removeBuff(Player, BuffID.OnFire); //TODO: Eventually make this automatically remove debuffs (with some exceptions)
            removeBuff(Player, BuffID.OnFire3);
            removeBuff(Player, BuffID.Frostburn);
            removeBuff(Player, BuffID.Frostburn2);
            removeBuff(Player, BuffID.Ichor);
            removeBuff(Player, BuffID.BrokenArmor);
            removeBuff(Player, BuffID.Webbed);
            removeBuff(Player, BuffID.Panic);
            removeBuff(Player, BuffID.Poisoned);
            removeBuff(Player, BuffID.CursedInferno);
            removeBuff(Player, BuffID.Confused);
            removeBuff(Player, BuffID.Bleeding);
            removeBuff(Player, BuffID.Oiled);
            removeBuff(Player, BuffID.ShadowFlame);
            removeBuff(Player, BuffID.Venom);
            removeBuff(Player, BuffID.Weak);
            removeBuff(Player, ModContent.BuffType<VonDebuff>());

            doParryFX(pos);
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
            {
                SyncParry(false, pos);
            }

        }
        public void doParryFX(Vector2 pos)
        {
            SoundEngine.PlaySound(ParrySound, pos);

            Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
            dustVelocity.Normalize();
            dustVelocity *= 2;

            int dustPos = 20;
            for (int i = 0; i < 20; i++)
            {
                int P1 = Dust.NewDust(pos, dustPos - 5, dustPos - 5, DustID.YellowStarDust, dustVelocity.X * 2, dustVelocity.Y * 2, 0, default, 2f);
                Main.dust[P1].noGravity = true;
                Main.dust[P1].fadeIn = 2f;
                Main.dust[P1].velocity *= 3f;
            }
        }
        public void removeBuff(Terraria.Player player, int buffID)
        {
            if (player.HasBuff(buffID)) 
            {
                player.ClearBuff(buffID);
            }
        }

        // more regen taking place here
        public override void UpdateLifeRegen()
        {
            if (Player.HeldItem.type == ModContent.ItemType<BerserkersSpear>())
            {
                float percentLifeLeft = (float)Player.statLife / Player.statLifeMax2;
                Player.lifeRegen += Convert.ToInt32(1 / (percentLifeLeft + .065));
            }
            if (headdressAura)
                Player.lifeRegen += Headdress.LifeRegenBoost;
            if (greavesAura)
                Player.lifeRegen += GuardianGreaves.LifeRegenBoost;
            if (wearingSigmaCharm)
                Player.lifeRegen += 6;
            if (wearingTreadsLife)
                Player.lifeRegen += PowerTreads.LifeRegen;
            if (mendingAura)
                Player.lifeRegen += MendingAspect.LifeRegen;
            if (wearingShivasGuard)
                Player.lifeRegen += ShivasGuard.LifeRegenBoost;
            if (wearingOverloading && overloadingRegenCooldown <= 0)
                Player.lifeRegen += OverloadingAspect.LifeRegenBoost;

            double totalLifeMult = 1D +
            (wearingMending ? MendingAspect.LifeMult : 0D);
            Player.lifeRegen = (int)(Player.lifeRegen * totalLifeMult);
        }

        // armlet negative hp regen is here instead of in buff for an unknown reason
        public override void UpdateBadLifeRegen()
        {
            if (healingDisabledDebuff)
            {
                Player.nebulaLevelLife = 0;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;

                if (Player.lifeRegenCount > 0)
                    Player.lifeRegenCount = 0;
            }

            if (armletActive && armletDebuff)
                DamageOverTime(30);
            if (wearingNihilum && nulledDebuff)
                DamageOverTime(50);
            if (wearingSatanic && Main.zenithWorld)
                DamageOverTime(50);

            if (aghHexDebuff)
                DamageOverTime(50);
            if (divineDebuff)
                DamageOverTime(100);
            if (ghostflameDebuff)
                DamageOverTime(15);
            if (infernoDebuff)
                DamageOverTime(40);
            if (wingsOfLightDebuff)
                DamageOverTime(25);
            if (blazingDebuff)
                DamageOverTime(30);
            if (deathDebuff)
                DamageOverTime(30);

            if (Player.lifeRegen < 0)
            {
                if (wearingTreadsLife)
                    Player.lifeRegen += PowerTreads.ReducedDoTAmount;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
            }
        }

        // buff effects
        private void OtherBuffEffects()
        {
            // debuffs
            if (chargeShot)
            {
                ChargeBow();
            }
            if (dpCharge)
            {
                ChargeBow();
            }
            if (skadiDebuff)
            {
                Player.velocity *= 0.3f;
                Player.statDefense -= 25; // -25 flat defense
            }
            if (freezingDebuff)
            {
                Player.moveSpeed *= 0.25f;
            }
            if (jidiDebuff)
            {
                Player.statDefense -= 20; // -20 flat defense
            }
            if (healingDisabledDebuff)
            {
                Player.lifeSteal *= 0f;
                if (Player.potionDelay < 2)
                    Player.potionDelay = 2;
                if (Player.HasBuff(BuffID.PotionSickness))
                {
                    for (var i = 0; i < Player.buffType.Length; i++)
                    {
                        if (Player.buffType[i] == BuffID.PotionSickness)
                        {
                            Player.buffTime[i] = Player.potionDelay;
                        }
                    }
                }
            }

            // buffs
            if (greavesAura)
            {
                Player.statDefense += GuardianGreaves.DefenseBoost;
                Player.statLifeMax2 += GuardianGreaves.LifeBoost;
                Player.statManaMax2 += GuardianGreaves.AuraManaBoost;
                Player.GetDamage<MagicDamageClass>() += GuardianGreaves.MagicDamageBoost;
            }
            if (vladsAura)
            {
                Player.statDefense += VladmirsOffering.DefenseBoost;
                Player.GetDamage<GenericDamageClass>().Flat += VladmirsOffering.FlatDamageBoost;
                Player.manaRegenBonus += VladmirsOffering.ManaRegenBoost;
                Player.lifeSteal *= VladmirsOffering.LifeStealBoost + 1;
            }
            if (wraithAura)
            {
                Player.statDefense += WraithPact.DefenseBoost;
                Player.GetDamage<GenericDamageClass>() += WraithPact.AttackDamageBoost;
                Player.manaRegenBonus += WraithPact.ManaRegenBoost;
                Player.lifeSteal *= WraithPact.LifeStealBoost + 1;
            }
            if (drumsAura)
            {
                Player.moveSpeed += DrumOfEndurance.MovementSpeedBoost;
                Player.GetAttackSpeed<MeleeDamageClass>() += DrumOfEndurance.MeleeSpeedBoost;
                Player.GetAttackSpeed<SummonMeleeSpeedDamageClass>() += DrumOfEndurance.WhipSpeedBoost;
            }
            if (shivasAura)
            {
                Player.statDefense += ShivasGuard.DefenseBoost;
                Player.GetDamage<RangedDamageClass>() += ShivasGuard.RangedDamageBoost;
            }
            if (wraithActive)
            {
                Player.aggro += 1000;
                Player.lifeSteal *= 0f;
            }
            if (satanicBuff)
            {
                Player.lifeSteal *= 4;
                Player.blind = true;
                Player.blackout = true; // greatly reduced light effect
                Player.headcovered = true; // nebula headcrab effect
            }
            if (inShadowRealm)
            {
                Player.yoraiz0rDarkness = true;
                Player.GetDamage<MagicDamageClass>() += (shadowRealmLevel / 30) + 1;
                Player.aggro -= 200;
                Player.blind = true;
                Player.blackout = true; // greatly reduced light effect
                if (Main.zenithWorld)
                    Player.headcovered = true;
                Player.detectCreature = true;
            }
            if (krakenBuff)
            {
                Player.endurance += OversizedAnchor.DefenseReductionBoost;
                Player.moveSpeed *= 0.5f;
            }
            if (wearingSpiritArmor)
                Player.slowFall = true;
            if (armletDebuff)
            {
                Player.GetAttackSpeed<GenericDamageClass>() += 0.15f;
                Player.GetDamage<GenericDamageClass>() += .15f;
            }
            if (nulledDebuff)
                Player.lifeSteal *= 0f;
            if (wearingSigmaCharm)
            {
                Player.wereWolf = sigmaCharmVisual;
                Player.GetDamage<GenericDamageClass>() += 0.08f;
                Player.GetCritChance<GenericDamageClass>() += 4;
                Player.GetAttackSpeed<MeleeDamageClass>() += 0.08f;
                Player.statDefense += 4;
                Player.moveSpeed += 0.1f;
                Player.lifeSteal *= 1.1f;
            }
            if (wearingAghGauntlet)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.15f;
                Player.GetCritChance<MeleeDamageClass>() += 5;
            }
            if (wearingRefresherOrb)
            {
                Player.statManaMax2 += 50;
                Player.GetDamage(DamageClass.Magic) += .10f;
                Player.GetDamage(DamageClass.Summon) += .10f;
                if (Main.zenithWorld)
                {
                    Player.immune = false;
                    Player.immuneTime = 0;
                }
            }

            if (wearingPowerTreads)
            {
                if (Main.zenithWorld)
                {
                    Player.moveSpeed *= 50f;
                    if (Player.velocity.Y == 0f)
                    {
                        Player.velocity.Y -= Player.jumpHeight * (Player.jumpSpeedBoost + 5);
                        SoundEngine.PlaySound(SoundID.Item150, Player.Center);
                    }
                }
                else if (wearingTreadsLife)
                {
                    Player.statLifeMax2 += PowerTreads.LifeBoost;
                }
                else if (wearingTreadsDamage)
                    Player.GetCritChance<GenericDamageClass>() += PowerTreads.CritBoost;
                else if (wearingTreadsBuilding)
                {
                    Player.pickSpeed -= PowerTreads.MiningSpeed;
                    Player.tileSpeed += PowerTreads.PlacementSpeed;
                    Player.wallSpeed += PowerTreads.PlacementSpeed;
                    Player.GetJumpState<TreadsJump>().Enable();
                }
            }

            double totalManaMult = 1D +
            (wearingRadiantArmor ? RadiantFlower.ManaMult : 0D);
            Player.manaRegenBonus = (int)(Player.manaRegenBonus * totalManaMult);

            int percentMaxLifeIncrease = 0;
            //if (wearingLife)
            //    percentMaxLifeIncrease += LifeItem.LifeMult;

            Player.statLifeMax2 += Player.statLifeMax / 5 / 20 * percentMaxLifeIncrease;

            // Gauntlet Melee Speed, prevents glove stacking for melee speed
            if (gloveLevel > 0)
            {
                // Determine the glove the player benefits from in priority of latest in progression
                float gloveAttackSpeed = (gloveLevel == 5 ? 0.15f : gloveLevel == 4 ? 0.14f : gloveLevel >= 2 ? 0.12f : gloveLevel == 1 ? 0.10f : 0);
                Player.GetAttackSpeed<MeleeDamageClass>() += gloveAttackSpeed; // Give the player attack speed based on the glove they have
            }

            // cooldowns
            if (shivCooldown > 0)
                shivCooldown--;
            if (bashCooldown > 0)
                bashCooldown--;
            if (radiantCooldown > 0)
                radiantCooldown--;
            if (jidiPollenCooldown > 0)
                jidiPollenCooldown--;
            if (gunpowderCooldown > 0)
                gunpowderCooldown--;
            if (hellfireCooldown > 0 && wearingHellfireArmor)
                hellfireCooldown--;
            if (satanicAccCooldown > 0)
                satanicAccCooldown--;
            if (seraphicReviveCounter > 0 && wearingSeraphic)
                seraphicReviveCounter--;
            if (VoniumLifeCooldown > 0)
                VoniumLifeCooldown--;
            if (praporCooldown > 0)
                praporCooldown--;
            if (toxicCooldown > 0)
                toxicCooldown--;
            if (overloadingCooldown > 0)
                overloadingCooldown--;
            if (gildedReflectCooldown > 0 && wearingGilded)
                gildedReflectCooldown--;
            if (gildedCoinDropCooldown > 0 && wearingGilded)
                gildedCoinDropCooldown--;
            if (overloadingRegenCooldown > 0 && wearingOverloading)
                overloadingRegenCooldown--;
            if (hellfireOverheat > 0)
                hellfireOverheat--;
        }
        
        // stops player from moving while charging bow
        private void ChargeBow()
        {
            Player.controlUp = false;
            Player.controlDown = false;
            Player.controlLeft = false;
            Player.controlRight = false;
            Player.controlJump = false;
            Player.controlHook = false;
            Player.controlMount = false;
            if (Player.velocity.Y > 15f)
                Player.velocity.Y = 15f;
        }
        public void DamageOverTime(int debuffDamage)
        {
            // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            // Player.lifeRegenTime used to increase the speed at which the player reaches its maximum natural life regeneration
            // So we set it to 0, and while this debuff is active, it never reaches it
            Player.lifeRegenTime = 0;
            // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
            Player.lifeRegen -= debuffDamage;
        }
        #endregion

        #endregion

        #region Reset Effects
        // resets stuff
        public override void ResetEffects()
        {
            mouseWorldListener = false;
            mouseRotationListener = false;

            wearingRigSlot = false;
            isWearingGlimmerCape = false;
            wearingManaBoots = false;
            wearingSatanic = false;
            wearingRefresherOrb = false;

            locketActive = false;
            wandActive = false;
            stickActive = false;
            armletActive = false;

            wearingHelmOfDominator = false;
            wearingHelmOfOverlord = false;
            wearingGigaManaBoots = false;
            wearingMekansm = false;
            wearingForceStaff = false;
            wearingPike = false;
            wearingShivasGuard = false;
            wearingEyeOfSkadi = false;
            wearingFlameOfCorruption = false;
            wearingWingsOfLight = false;
            wingsOfLightVisual = false;
            wearingFishSlop1 = false;
            wearingFishSlop2 = false;
            wearingGiantsMaul = false;
            wearingGunpowderGauntlet = false;
            wearingDuelistGloves = false;
            wearingWhisperDread = false;
            wearingSerratedShiv = false;
            wearingUndyingHelm = false;
            wearingSearingSignet = false;
            wearingVladimirs = false;
            wearingWraithPact = false;
            wearingJidiPollenBag = false;
            wearingShadowAmulet = false;
            shadowAmuletVisual = false;
            exultationEquipped = false;
            plasmaVisual = false;
            polyluteVisual = false;
            wearingRuntyHorseshoe = false;
            wearingAllegianceWings = false;
            wearingSacrosanctAegis = false;
            wearingSigmaCharm = false;
            sigmaCharmVisual = false;
            wearingAghGauntlet = false;
            aghGauntletVisual = false;
            gloveLevel = 0;
            wearingElvenQuiver = false;
            wearingEnchantedQuiver = false;
            wearingFlayersBota = false;
            wearingScavVest = false;
            wearingTriton = false;
            wearingZhuk = false;
            wearingPowerTreads = false;
            wearingTreadsLife = false;
            wearingTreadsDamage = false;
            wearingTreadsBuilding = false;
            wearingOverloading = false;
            wearingBlazing = false;
            wearingGilded = false;
            wearingMending = false;
            wearingToxic = false;
            wearingChaosDice = false;
            //stopFallDamage = false;

            if (!Main.playerInventory || Main.LocalPlayer.chest >= 0 || Main.LocalPlayer.channel)
            {
                tritonActive = false;
                zhukActive = false;
            }

            wearingMendez = false;

            wraithActive = false;

            wearingRadiantArmor = false;
            wearingUndyingArmor = false;
            wearingTankyRizzler = false;
            wearingBladeMail = false;
            wearingFrostArmor = false;
            wearingFrostMagic = false;
            wearingFrostSummon = false;
            wearingDamascus1 = false;
            wearingDamascus2 = false;
            wearingBoneArmor = false;
            wearingWhiteArmor = false;
            wearingFaeArmor = false;
            wearingHellfireArmor = false;
            wearingSpiritArmor = false;
            wearingSeraphic = false;
            canSeraphicRevive = false;
            wearingNihilum = false;
            wearingNihilumRanged = false;

            diademMinion = false;
            dominatorMinion = false;
            overlordMinion = false;

            chargeShot = false;
            dpCharge = false;

            infiniteFlight = false;

            divineDebuff = false;
            skadiDebuff = false;
            freezingDebuff = false;
            aghHexDebuff = false;
            wingsOfLightDebuff = false;
            ghostflameDebuff = false;
            jidiDebuff = false;
            shivaDebuff = false;
            infernoDebuff = false;
            armletDebuff = false;
            nulledDebuff = false;
            blazingDebuff = false;
            toxicDebuff = false;
            deathDebuff = false;
            healingDisabledDebuff = false;

            greavesAura = false;
            wraithAura = false;
            vladsAura = false;
            headdressAura = false;
            drumsAura = false;
            shivasAura = false;
            mendingAura = false;

            satanicBuff = false;

            ahmodPet = false;
            gingyPet = false;

            inShadowRealm = false;
            krakenBuff = false;
            //eSeraphSound = false;

            atgActive = false;
            plasmaActive = false;
            icbmActive = false;
            polyluteActive = false;

            duelistStacks = 0;

            holdingThrowingShade = false;

            ammoCost = 1f;
            //maxShotsMult = 1f;
            //reloadTimeMult = 1f;

            fCrystal = false;
            divinitasMinion = false;

            if (Player.controlDown)
                forceDirection = DashDown;
            else if (Player.controlUp)
                forceDirection = DashUp;
            else if (Player.controlRight)
                forceDirection = DashRight;
            else if (Player.controlLeft)
                forceDirection = DashLeft;
            else
                forceDirection = -1;

            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
                DashDir = DashDown;
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
                DashDir = DashUp;
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 && Player.doubleTapCardinalTimer[DashLeft] == 0)
                DashDir = DashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 && Player.doubleTapCardinalTimer[DashRight] == 0)
                DashDir = DashLeft;
            else
                DashDir = -1;
        }
        #endregion
    }
}