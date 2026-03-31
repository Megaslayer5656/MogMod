using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.ClasslessProjectiles;
using MogMod.Projectiles.MeleeProjectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace MogMod.Common.MogModPlayer
{
    // this files a mess to look at
    public partial class MogPlayer : ModPlayer
    {
        #region Setup
        public bool mewing = false;
        public float mewingguide = 0;

        Random rand = new Random();

        // buffs for the accessories
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
        public bool armletOn = false;

        public bool wearingHelmOfDominator;
        public bool wearingHelmOfOverlord;
        public bool wearingForceStaff;
        public bool wearingPike;
        public bool wearingBladeMail;
        public bool wearingFrostArmor;
        public bool wearingDamascus;

        public bool diademMinion = false;
        public bool dominatorMinion = false;
        public bool overlordMinion = false;

        public bool wearingShivasGuard = false;
        public int shivasSlowTimer = 0;
        public int shivasSlowTimerMax = 36000;
        public bool shivasAttack = false;

        public int wingsOfLightDust = 0;
        public int forceDirection = -1;
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;
        public const float ForceVelocity = 12f;
        public const float PikeVelocity = 25f;

        public int duelistStacks = 0;
        public static int maxDuelistStacks = 3;

        // weapon buffs
        public int essenceShiftLevel = 0;
        public static int essenceShiftLevelMax = 60;

        public int fierySoulLevel = 0;
        public static int fierySoulLevelMax = 30;

        public bool holdingThrowingShade;
        public int shadowRealmLevel = 0;
        public static int shadowRealmLevelMax = 150;

        public Vector2 mouseWorld;
        public bool wearingFlameOfCorruption = false;
        public bool dragonInstallActive;

        public int cooldownReference;
        public enum MewingType
        {
            mewingguide = 0
        }
        public MewingType mewingType = MewingType.mewingguide;

        public bool chargeShot = false;
        public bool dpCharge = false;

        // armor effects
        public bool wearingRadiantArmor;
        public bool wearingUndyingArmor;
        public bool wearingTankyRizzler;
        public int tankyRizzlerHits = 0;
        public static int counterHelixDmg = 500;

        // debuffs
        public bool divineDebuff;
        public bool skadiDebuff;
        public bool freezingDebuff;
        public bool aghHexDebuff;
        public bool wingsOfLightDebuff;
        public bool ghostflameDebuff;
        public bool jidiDebuff;
        public bool shivaDebuff;

        // auras
        public bool greavesAura = false;
        public bool wraithAura = false;
        public bool vladsAura = false;
        public bool headdressAura = false;
        public bool drumsAura = false;
        public bool shivasAura = false;

        public float auraRange = 5000f;

        public bool inShadowRealm;

        public bool riversOfBloodProj = false;
        public bool exultationEquipped = false;

        public bool markerProjOut = false;

        public bool atgActive = false;
        public bool plasmaActive = false;
        public bool icbmActive = false;
        public bool polyluteActive = false;

        public int shivCooldown = 0;
        public int bashCooldown = 0;
        public int gunpowderCooldown = 0;
        public int radiantCooldown = 0;
        public int jidiPollenCooldown = 0;

        public bool moonveilProj = false;

        // sound effects
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
            Volume = .25f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public static readonly SoundStyle ParrySound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ParrySfx")
        {
            Volume = .25f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        #endregion

        #region Mod Buff ID/s
        int glimmerBuff = ModContent.BuffType<Buffs.PotionBuffs.GlimmerCapeBuff>();
        int satanicBuff = ModContent.BuffType<Buffs.PotionBuffs.SatanicBuff>();
        int blademailBuff = ModContent.BuffType<Buffs.PotionBuffs.BladeMailBuff>();

        // cooldowns
        int refresherCooldown = ModContent.BuffType<Buffs.Cooldowns.RefresherOrbDebuff>();
        int glimmerCooldown = ModContent.BuffType<Buffs.Cooldowns.GlimmerCapeDebuff>();
        int satanicCooldown = ModContent.BuffType<Buffs.Cooldowns.SatanicDebuff>();
        int manabootsCooldown = ModContent.BuffType<Buffs.Cooldowns.ArcaneBootsDebuff>();
        int guardianCooldown = ModContent.BuffType<Buffs.Cooldowns.GuardianGreavesDebuff>();
        int mekansmCooldown = ModContent.BuffType<Buffs.Cooldowns.MekansmDebuff>();
        int helmOfDominator = ModContent.BuffType<Buffs.Cooldowns.HelmOfDominatorDebuff>();
        int forceStaffCooldown = ModContent.BuffType<Buffs.Cooldowns.ForceStaffDebuff>();
        int blademailCooldown = ModContent.BuffType<Buffs.Cooldowns.BladeMailDebuff>();
        int ShivasCooldown = ModContent.BuffType<ShivasDebuff>();

        // one time buffs (and armlet)
        int locketHeal = ModContent.BuffType<HolyLocketBuff>();
        int wandHeal = ModContent.BuffType<WandBuff>();
        int stickHeal = ModContent.BuffType<MagicStickBuff>();

        int greavesHeal = ModContent.BuffType<GuardianGreavesBuff>();
        int mekansmHeal = ModContent.BuffType<MekansmBuff>();

        int armletToggled = ModContent.BuffType<Buffs.PotionBuffs.ArmletOfMordiggianBuff>();

        // dragon install
        int dragonInstall = ModContent.BuffType<Buffs.PotionBuffs.DragonInstallBuff>();
        int dragonInstallCooldown = ModContent.BuffType<Buffs.Cooldowns.DragonInstallCooldown>();
        #endregion

        #region In Game Checks

        #region On Hit Effects
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (wearingEyeOfSkadi)
                target.AddBuff(ModContent.BuffType<EyeOfSkadiDebuff>(), 360);
            if (wearingSearingSignet)
                target.AddBuff(BuffID.ShadowFlame, 300);
            if (Player.HasBuff<DragonInstallBuff>())
                target.AddBuff(BuffID.Daybreak, 600);
            if (wearingFrostArmor)
                target.AddBuff(ModContent.BuffType<FreezingDebuff>(), 300);
        }

        public void doATG(int damageDone)
        {
            Vector2 kirk = new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(15));
            Vector2 einstein = Main.MouseWorld - Player.Center;
            einstein.Normalize();

            Vector2 epstein = einstein * 20;

            int procChance = rand.Next(1, 11);
            if (atgActive && procChance == 5)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk, ModContent.ProjectileType<ATGProjectile>(), damageDone + 1, 3, Player.whoAmI);
                if (icbmActive)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        kirk = new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(15));
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, kirk, ModContent.ProjectileType<ATGProjectile>(), Convert.ToInt32(damageDone * .5f) + 1, 3, Player.whoAmI);
                    }
                }
            }
            if (plasmaActive)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, epstein, ModContent.ProjectileType<PlasmaShrimpProj>(), Convert.ToInt32(damageDone * .1f) + 1, 1, Player.whoAmI);
        }
        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
            Player.ClearBuff(ModContent.BuffType<ClarityBuff>());
            Player.ClearBuff(ModContent.BuffType<HealingSalveBuff>());
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
        public override void OnHitByProjectile(Projectile proj, Terraria.Player.HurtInfo hurtInfo)
        {
            Player.ClearBuff(ModContent.BuffType<ClarityBuff>());
            Player.ClearBuff(ModContent.BuffType<HealingSalveBuff>());
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
        #endregion

        // the big one
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            #region Accessory Checks
            // refresher orb
            if (KeybindSystem.RefresherOrbKeybind.JustPressed && wearingRefresherOrb && !Player.HasBuff(refresherCooldown))
            {
                // make it play a sound when activating (add any additional debuffs here)
                Player.ClearBuff(glimmerCooldown);
                Player.ClearBuff(satanicCooldown);
                Player.ClearBuff(manabootsCooldown);
                Player.ClearBuff(guardianCooldown);
                Player.ClearBuff(mekansmCooldown);
                Player.ClearBuff(helmOfDominator);
                Player.ClearBuff(forceStaffCooldown);
                Player.ClearBuff(blademailCooldown);
                Player.ClearBuff(ShivasCooldown);
                //Don't add dragon install to this. It shouldn't be able to be refreshed by refresher as it is more of a different mechanic than a buff. Will if you see this stop playing Chen <-- Chen (pronounced "shen") has crazy micro and once i get good at him hes gonna be crazy. that one game was a loss no matter who i played. also it was mendez fault for picking IO

                Player.AddBuff(refresherCooldown, 9000);
            }

            // glimmer cape
            if (KeybindSystem.GlimmerCapeKeybind.JustPressed && isWearingGlimmerCape && !Player.HasBuff(glimmerCooldown))
            {
                // give buff, 600 = 10 seconds
                Player.AddBuff(glimmerBuff, 1800);
                // give debuff cd
                Player.AddBuff(glimmerCooldown, 3600);
                // Main.NewText("applied glimmer cape"); //RandomBuffText.Format(Lang.GetBuffName(buff)));
            }

            // satanic
            if (KeybindSystem.SatanicKeybind.JustPressed && wearingSatanic && !Player.HasBuff(satanicCooldown))
            {
                Player.AddBuff(satanicBuff, 1800);
                Player.AddBuff(satanicCooldown, 3600);
            }

            // blademail
            if (KeybindSystem.BladeMailKeybind.JustPressed && wearingBladeMail && !Player.HasBuff(blademailCooldown))
            {
                Player.AddBuff(blademailBuff, 600);
                Player.AddBuff(blademailCooldown, 3600);
            }

            // arcane boots
            if (KeybindSystem.ArcaneBootsKeybind.JustPressed && wearingManaBoots && !Player.HasBuff(manabootsCooldown))
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Terraria.Player targetPlayer = Main.player[i];
                    if (targetPlayer.active && targetPlayer.team == targetPlayer.team && targetPlayer.team != 0)
                    {
                        targetPlayer.AddBuff(greavesHeal, 600);
                        //if (Main.netMode == NetmodeID.Server) // Check if the game is in multiplayer server mode
                        //{
                        //    NetMessage.SendData(MessageID.PlayerBuffs, -1, -1, null, i, mekansmHeal, 600f, 0f, 0, 0, 0);
                        //}
                        for (int k = 0; k < 16; k++)
                        {
                            Dust dust2 = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.ManaRegeneration);
                            dust2.scale = Main.rand.NextFloat(0.6f, 0.8f);
                        }
                    }
                }
                // make it play a sound when activating
                Player.statMana += 200;
                Player.AddBuff(manabootsCooldown, 1800);
            }

            // guardian greaves
            if (KeybindSystem.GuardianGreavesKeybind.JustPressed && wearingGigaManaBoots && !Player.HasBuff(guardianCooldown))
            {
                // make it play a sound when activating
                Player.statLife += 50;
                Player.statMana += 250;
                Player.AddBuff(guardianCooldown, 3600);
            }

            // mekansm
            if (KeybindSystem.MekansmKeybind.JustPressed && wearingMekansm && !Player.HasBuff(mekansmCooldown))
            {
                // make it play a sound when activating
                Player.statLife += 20;
                Player.AddBuff(mekansmCooldown, 3600);
            }

            // helm of dominator
            if (KeybindSystem.HelmOfDominatorKeybind.JustPressed && wearingHelmOfDominator && !Player.HasBuff(helmOfDominator))
            {
                // for now it summons a mount (change to make it summon a friendly npc to damage enemies)
                Player.AddBuff(BuffID.BasiliskMount, 1);
                Player.AddBuff(helmOfDominator, 1800);
            }

            // helm of overlord
            if (KeybindSystem.HelmOfDominatorKeybind.JustPressed && wearingHelmOfOverlord && !Player.HasBuff(helmOfDominator))
            {
                // for now it summons a mount (change to make it summon a friendly npc to damage enemies)
                Player.AddBuff(BuffID.CuteFishronMount, 1);
                Player.AddBuff(helmOfDominator, 600);
            }

            // holy locket
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (locketActive)
                {
                    Player.AddBuff(locketHeal, 6);
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            //Wand
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (wandActive)
                {
                    Player.AddBuff(wandHeal, 6);
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            //Magic Stick
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (stickActive)
                {
                    Player.AddBuff(stickHeal, 6);
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
                    int DI2 = Dust.NewDust(Player.Center, dustPos - 5, dustPos - 5, DustID.Blood, dustVelocity.X * 2, dustVelocity.Y * 2, 0, Color.Red, 2f);
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
                    armletOn = true;
                    SoundEngine.PlaySound(ArmletOnSound, Player.Center);
                } else if (armletTimer >= armletTimerMax)
                {
                    Player.ClearBuff(armletToggled);
                    armletOn = false;
                    armletTimer = 0;
                    SoundEngine.PlaySound(ArmletOffSound, Player.Center);
                }
            }

            if (!armletActive)
            {
                Player.ClearBuff(armletToggled);
            }

            while (armletTimer >= 1 && armletTimer <= armletTimerMax + 1)
            {
                armletTimer += 1;
            }
            #endregion
        }

        #region Miscelanious Effects (spelt right)

        // force staff movement
        public override void PreUpdateMovement()
        {
            int forceStaffCooldown = ModContent.BuffType<Buffs.Cooldowns.ForceStaffDebuff>();
            // if force staff isn't on cooldown and was equipped and player just pressed keybind
            if (wearingForceStaff && !Player.mount.Active &&  KeybindSystem.ForceStaffKeybind.JustPressed && !Player.HasBuff(forceStaffCooldown))
            {
                // change to force staff sound
                SoundEngine.PlaySound(WandUse, Player.Center);
                Vector2 newVelocity = Player.velocity;

                switch (forceDirection)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case DashUp when Player.velocity.Y > -ForceVelocity:
                    case DashDown when Player.velocity.Y < ForceVelocity:
                            {
                            // Y-velocity is set here
                            // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                            // This adjustment is roughly 1.3x the intended dash velocity
                            float dashDirection = forceDirection == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * ForceVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -ForceVelocity:
                    case DashRight when Player.velocity.X < ForceVelocity:
                        {
                            // X-velocity is set here
                            float dashDirection = forceDirection == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * ForceVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                }

                // start our dash
                //DashDelay = DashCooldown;
                //DashTimer = DashDuration;
                Player.velocity = newVelocity;
                Player.AddBuff(forceStaffCooldown, 600);
            }

            if (wearingPike && !Player.mount.Active && KeybindSystem.ForceStaffKeybind.JustPressed && !Player.HasBuff(forceStaffCooldown))
            {
                // change to force staff sound
                SoundEngine.PlaySound(ArmletOnSound, Player.Center);
                Vector2 newVelocity = Player.velocity;

                switch (forceDirection)
                {
                    case DashUp when Player.velocity.Y > -PikeVelocity:
                    case DashDown when Player.velocity.Y < PikeVelocity:
                        {
                            float dashDirection = forceDirection == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * PikeVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -PikeVelocity:
                    case DashRight when Player.velocity.X < PikeVelocity:
                        {
                            float dashDirection = forceDirection == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * PikeVelocity;
                            break;
                        }
                    default:
                        return;
                }
                Player.velocity = newVelocity;
                Player.AddBuff(forceStaffCooldown, 300);
            }
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
                {
                    if (diademMinion)
                    {
                        Player.maxMinions++;
                    }
                }
            }
            if (locketActive)
            {
                Player.maxMinions += 2;
            }
            if (wearingWraithPact)
            {
                Player.maxTurrets += 5;
            }
            else
                if (wearingVladimirs)
                {
                    Player.maxTurrets += 2;
                }
            #endregion

            #region Weapon Buffs
            // essence shift stacking buff
            if (Player.HasBuff<EssenceShift>() && (Player.HeldItem.Name == "Hydrakan Latch" || Player.HeldItem.Name == "Golden Hydrakan Latch" || Player.HeldItem.Name == "Megaslark" || Player.HeldItem.Name == "Minislark"))
            {
                if (Player.HeldItem.Name == "Hydrakan Latch" || Player.HeldItem.Name == "Golden Hydrakan Latch")
                {
                    if (essenceShiftLevel > essenceShiftLevelMax)
                    {
                        essenceShiftLevel = essenceShiftLevelMax;
                    }
                    Player.GetAttackSpeed(DamageClass.Melee) += .1f * essenceShiftLevel;
                    Player.moveSpeed += .025f * essenceShiftLevel;
                    Player.accRunSpeed += Player.accRunSpeed * .025f * essenceShiftLevel;
                }
                if (Player.HeldItem.Name == "Megaslark")
                {
                    if (essenceShiftLevel > essenceShiftLevelMax)
                    {
                        essenceShiftLevel = essenceShiftLevelMax;
                    }
                    Player.GetAttackSpeed(DamageClass.Ranged) += .1f * essenceShiftLevel;
                    Player.GetArmorPenetration(DamageClass.Ranged) += essenceShiftLevel;
                    Player.moveSpeed += .025f * essenceShiftLevel;
                    Player.accRunSpeed += Player.accRunSpeed * .025f * essenceShiftLevel;
                }
                if (Player.HeldItem.Name == "Minislark")
                {
                    if (essenceShiftLevel > essenceShiftLevelMax)
                    {
                        essenceShiftLevel = essenceShiftLevelMax;
                    }
                    Player.GetAttackSpeed(DamageClass.Ranged) += .05f * essenceShiftLevel;
                    Player.GetArmorPenetration(DamageClass.Ranged) += (float)essenceShiftLevel / 3;
                    Player.moveSpeed += .015f * essenceShiftLevel;
                    Player.accRunSpeed += Player.accRunSpeed * .015f * essenceShiftLevel;
                }
            }
            else
            {
                essenceShiftLevel = 0;
                Player.ClearBuff(ModContent.BuffType<EssenceShift>());
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncEssenceShift(false);
                }
            }
            if (Player.HeldItem.type == ModContent.ItemType<ThrowingShade>() || Player.HeldItem.type == ModContent.ItemType<ShadowRealm>())
            {
                if (Player.HeldItem.type == ModContent.ItemType<ThrowingShade>())
                    holdingThrowingShade = true;
                if ((shadowRealmLevel < shadowRealmLevelMax) && Player.HasBuff<ShadowRealmBuff>())
                    shadowRealmLevel++;
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
                Player.GetAttackSpeed(DamageClass.Magic) += .015f * fierySoulLevel;
                Player.manaCost -= .015f * fierySoulLevel;
                Player.moveSpeed += .0225f * fierySoulLevel;
                Player.accRunSpeed += Player.accRunSpeed * .0225f * fierySoulLevel;
            }
            else
            {
                fierySoulLevel = 0;
            }

            if (Player.HeldItem.Name == "Butterfly")
            {
                doButterfly(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncButterfly(false); //TODO: Add a timer so this doesn't sync every tick
                }
            }

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
        }
        public override void PostUpdateMiscEffects()
        {
            MiscEffects();
            OtherBuffEffects();
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            static Item createItem(int type)
            {
                Item i = new Item();
                i.SetDefaults(type);
                return i;
            }

            if (!mediumCoreDeath)
                yield return createItem(ModContent.ItemType<VonWarning>());
        }
        public override void PostUpdate()
        {
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
            {
                shadowTimer = 0;
            }
        }

        // shivas effect and dust;
        public void doShivas(Terraria.Player player, Vector2 center) //This needs to be its own method for netcode to work. See how I did it in MogModNetcode.cs and MogPlayerNetcode.cs
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
        public void doWingsOfLight(Terraria.Player player, Vector2 center) // refer to shivas for how this works
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

        // helm of undying
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (wearingUndyingHelm)
            {
                doUndying();
            }
        }
        public void doUndying()
        {
            Player.respawnTimer = Convert.ToInt32(Player.respawnTimer * .6f);
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<UndyingPortalProj>(), 100, 1, Player.whoAmI); //Might need to rebalance this damage
        }
        // both undying portals damage prob have to be nerfed
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (wearingUndyingArmor && !Player.HasBuff(ModContent.BuffType<WraithBuff>()))
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52, Player.Center);
                Player.AddBuff(ModContent.BuffType<WraithBuff>(), 300);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<PlayerUndyingPortalProj>(), 100, 1, Player.whoAmI); //Might need to rebalance this damage
            }
            if (Player.HasBuff(ModContent.BuffType<WraithBuff>()))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Player Buffs / Debuffs
        public override void ModifyHurt(ref Terraria.Player.HurtModifiers modifiers)
        {
            double damageMult = 1D;
            if (wearingWhisperDread) // increases damage taken
                damageMult += 0.15;

            modifiers.SourceDamage *= (float)damageMult;
        }

        // sniper offlane scope effect
        public override void ModifyZoom(ref float zoom)
        {
            if (Player.HeldItem.Name == "AXMC")
            {
                if (Main.mouseRight == true)
                {
                    zoom = Player.scope ? 0.8f : 0.6666667f;
                }
            }
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
                player.GetAttackSpeed(DamageClass.Melee) += .07f;
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

            if (Player.HeldItem.Name == "Rivers Of Blood")
            {
                riversOfBloodProj = true;
            }

            if (Player.HeldItem.Name == "Moonveil")
            {
                moonveilProj = true;
            }

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

        // armlet negative hp regen is here instead of in buff for an unknown reason
        public override void UpdateBadLifeRegen()
        {
            if (armletOn && Player.HasBuff<ArmletOfMordiggianBuff>())
            {
                DamageOverTime(30);
            }
        }

        // more regen taking place here
        public override void UpdateLifeRegen()
        {
            if (Player.HeldItem.Name == "Berserker's Spear")
            {
                float percentLifeLeft = (float)Player.statLife / Player.statLifeMax2;
                Player.lifeRegen += Convert.ToInt32((1 / (percentLifeLeft + .065)));
            }
            if (headdressAura)
            {
                Player.lifeRegen += 4;
            }
            if (greavesAura)
            {
                Player.lifeRegen += 8;
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
                Player.velocity *= 0.988f;
                Player.statDefense -= 25; // -25 flat defense
            }
            if (freezingDebuff)
            {
                Player.velocity *= 0.985f;
            }
            if (aghHexDebuff)
            {
                Convert.ToInt32(Player.GetDamage(DamageClass.Generic) * .8f); // 20% damage reduction
            }
            if (wingsOfLightDebuff)
            {
                Convert.ToInt32(Player.GetDamage(DamageClass.Generic) * .9f); // 10% damage reduction
            }
            if (jidiDebuff)
            {
                Player.statDefense -= 10; // -10 flat defense
            }

            // buffs
            if (greavesAura)
            {
                Player.statDefense += 4;
                Player.statLifeMax2 += 20;
                Player.statManaMax2 += 50;
                Player.GetDamage(DamageClass.Magic) += .075f;
            }
            if (vladsAura)
            {
                Player.statDefense += 3;
                Player.GetDamage(DamageClass.Generic).Flat += 5f;
                Player.lifeSteal *= 1.2f;
                Player.manaRegenBonus += 4;
            }
            if (wraithAura)
            {
                Player.statDefense += 7;
                Player.GetDamage(DamageClass.Generic) += .1f;
                Player.lifeSteal *= 1.8f;
                Player.manaRegenBonus += 6;
            }
            if (drumsAura)
            {
                Player.GetAttackSpeed(DamageClass.Melee) += .1f;
                Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += .1f;
                Player.moveSpeed += 0.30f;
            }
            if (shivasAura)
            {
                Player.statDefense += 10;
                Player.GetDamage(DamageClass.Ranged) += .10f;
            }

            if (wraithActive)
            {
                Player.aggro += 1000;
                Player.lifeSteal *= 0f;
            }
            if (inShadowRealm)
            {
                Player.GetDamage(DamageClass.Magic) += (shadowRealmLevel / 30) + 1;
            }

            // cooldowns
            if (shivCooldown > 0)
                shivCooldown--;
            if (bashCooldown > 0)
                shivCooldown--;
            if (radiantCooldown > 0)
                shivCooldown--;
            if (jidiPollenCooldown > 0)
                shivCooldown--;
            if (gunpowderCooldown > 0)
                shivCooldown--;
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

        #region Reset Effects
        // resets stuff
        public override void ResetEffects()
        {
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

            wearingMendez = false;

            wraithActive = false;

            wearingRadiantArmor = false;
            wearingUndyingArmor = false;
            wearingTankyRizzler = false;
            wearingBladeMail = false;
            wearingFrostArmor = false;
            wearingDamascus = false;

            diademMinion = false;
            dominatorMinion = false;
            overlordMinion = false;

            chargeShot = false;
            dpCharge = false;

            divineDebuff = false;
            skadiDebuff = false;
            freezingDebuff = false;
            aghHexDebuff = false;
            wingsOfLightDebuff = false;
            ghostflameDebuff = false;
            jidiDebuff = false;
            shivaDebuff = false;

            greavesAura = false;
            wraithAura = false;
            vladsAura = false;
            headdressAura = false;
            drumsAura = false;
            shivasAura = false;

            inShadowRealm = false;

            atgActive = false;
            plasmaActive = false;
            icbmActive = false;
            polyluteActive = false;

            duelistStacks = 0;

            holdingThrowingShade = false;

            if (Player.controlDown)
            {
                forceDirection = DashDown;
            }
            else if (Player.controlUp)
            {
                forceDirection = DashUp;
            }
            else if (Player.controlRight)
            {
                forceDirection = DashRight;
            }
            else if (Player.controlLeft)
            {
                forceDirection = DashLeft;
            }
            else
            {
                forceDirection = -1;
            }
        }
        #endregion
        #endregion
    }
}
