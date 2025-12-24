using Microsoft.Xna.Framework;
using MogMod.Buffs;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Consumables;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace MogMod.Common.Player
{
    public class MogPlayer : ModPlayer
    {
        public bool mewing = false;
        public float mewingguide = 0;
        // buffs for the accessories
        public bool isWearingGlimmerCape = false;
        public bool armletActive = false;
        public bool wearingManaBoots = false;
        public bool wearingSatanic = false;
        public bool wearingRefresherOrb = false;
        public bool wearingGigaManaBoots = false;
        public bool wearingMekansm = false;
        public bool wearingEyeOfSkadi = false;

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

        public bool wearingHelmOfDominator = false;
        public bool wearingHelmOfOverlord = false;
        public bool wearingForceStaff = false;
        public bool wearingPike = false;
        public bool wearingBladeMail = false;

        public bool diademMinion = false;
        public bool dominatorMinion = false;
        public bool overlordMinion = false;

        public bool wearingShivasGuard = false;
        public int shivasSlowTimer = 0;
        public int shivasSlowTimerMax = 36000;

        public int forceDirection = -1;
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;
        public const float ForceVelocity = 12f;
        public const float PikeVelocity = 25f;

        public int essenceShiftLevel = 0;
        public static int essenceShiftLevelMax = 60;

        public int fierySoulLevel = 0;
        public static int fierySoulLevelMax = 30;

        public enum MewingType
        {
            mewingguide = 0
        }
        public MewingType mewingType = MewingType.mewingguide;
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
        public override void PostUpdateMiscEffects()
        {
            MiscEffects();
        }
        public void MiscEffects()
        {
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

            // essence shift stacking buff
            if (Player.HasBuff<EssenceShift>() && (Player.HeldItem.Name == "Hydrakan Latch" || Player.HeldItem.Name == "Golden Hydrakan Latch"))
            {
                if (essenceShiftLevel > essenceShiftLevelMax)
                {
                    essenceShiftLevel = essenceShiftLevelMax;
                }
                Player.GetAttackSpeed(DamageClass.Melee) += .1f * essenceShiftLevel;
                Player.moveSpeed += .025f * essenceShiftLevel;
                Player.accRunSpeed += Player.accRunSpeed * .025f * essenceShiftLevel;
            } else
            {
                essenceShiftLevel = 0;
                Player.ClearBuff(ModContent.BuffType<EssenceShift>());
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
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC otherNPC = Main.npc[i];
                        if (wearingEyeOfSkadi)
                    {
                        target.AddBuff(ModContent.BuffType<EyeOfSkadiDebuff>(), 600);
                        otherNPC.defense -= Convert.ToInt32(otherNPC.defense * 0.1);
                    }
                }
        }
        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
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
        }
        public override void OnHitByProjectile(Projectile proj, Terraria.Player.HurtInfo hurtInfo)
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
        }
        public override void PreUpdateMovement()
        {
            int forceStaffCooldown = ModContent.BuffType<Buffs.ForceStaffDebuff>();
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
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            int refresherCooldown = ModContent.BuffType<Buffs.RefresherOrbDebuff>();
            int glimmerBuff = ModContent.BuffType<Buffs.GlimmerCapeBuff>();
            int glimmerCooldown = ModContent.BuffType<Buffs.GlimmerCapeDebuff>();
            int satanicBuff = ModContent.BuffType<Buffs.SatanicBuff>();
            int satanicCooldown = ModContent.BuffType<Buffs.SatanicDebuff>();
            int manabootsCooldown = ModContent.BuffType<Buffs.ArcaneBootsDebuff>();
            int guardianCooldown = ModContent.BuffType<Buffs.GuardianGreavesDebuff>();
            int mekansmCooldown = ModContent.BuffType<Buffs.MekansmDebuff>();
            int helmOfDominator = ModContent.BuffType<Buffs.HelmOfDominatorDebuff>();
            int forceStaffCooldown = ModContent.BuffType<Buffs.ForceStaffDebuff>();
            int blademailBuff = ModContent.BuffType<Buffs.BladeMailBuff>();
            int blademailCooldown = ModContent.BuffType<Buffs.BladeMailDebuff>();
            int ShivasCooldown = ModContent.BuffType<ShivasDebuff>();
            int locketHeal = ModContent.BuffType<HolyLocketBuff>();
            int wandHeal = ModContent.BuffType<WandBuff>();
            int stickHeal = ModContent.BuffType<MagicStickBuff>();
            int armletToggled = ModContent.BuffType<Buffs.ArmletOfMordiggianBuff>();

            // refresher orb
            if (KeybindSystem.RefresherOrbKeybind.JustPressed && wearingRefresherOrb && !Player.HasBuff(refresherCooldown))
            {
                // make it play a sound when activating (add any additional debuffs here)
                Player.ClearBuff(glimmerCooldown);
                Player.ClearBuff(satanicCooldown);
                Player.ClearBuff(manabootsCooldown);

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
                // make it play a sound when activating
                Player.statMana += 200;
                Player.AddBuff(manabootsCooldown, 1800);
            }

            // guardian greaves
            if (KeybindSystem.GuardianGreavesKeybind.JustPressed && wearingGigaManaBoots && !Player.HasBuff(guardianCooldown))
            {
                // make it play a sound when activating
                Player.statLife += 100;
                Player.statMana += 300;
                Player.AddBuff(guardianCooldown, 3600);
            }

            // mekansm
            if (KeybindSystem.MekansmKeybind.JustPressed && wearingMekansm && !Player.HasBuff(mekansmCooldown))
            {
                // make it play a sound when activating
                Player.statLife += 50;
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
                for (int i = 0; i < Main.maxNPCs; i++) //Every npc is in an index, this goes through all of them
                {
                    NPC otherNPC = Main.npc[i]; //This sets the var otherNPC to the current npc we are targeting in the index
                    if (otherNPC.active && otherNPC.townNPC == false && otherNPC.whoAmI != otherNPC.whoAmI - 1) //Makes shivas not hit inactive npcs, townNpcs, and not cast on the same npc twice.
                    {
                        if (Microsoft.Xna.Framework.Vector2.Distance(Player.Center, otherNPC.Center) < 1200f)
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
                            otherNPC.defense -= Convert.ToInt32(otherNPC.defense * .15); //Removes 15% of enemy's defense (rounded)
                            //TODO: Put the stat decreases on a timer, slow enemies.
                        }
                    }
                    Player.AddBuff(ShivasCooldown, 3600);
                    SoundEngine.PlaySound(ShivasActivateSound, Player.Center);
                }

                for (int i = 0; i < 80; i++)
                {
                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 6;

                    int dustPos = 20;

                    int shiva1 = Dust.NewDust(Entity.Center, dustPos, dustPos, DustID.SnowSpray, dustVelocity.X * 3, dustVelocity.Y * 3, 0, default, 3f);
                    Main.dust[shiva1].noGravity = true;
                    Main.dust[shiva1].fadeIn = 5f;
                    Main.dust[shiva1].velocity *= 3f;
                    int shiva2 = Dust.NewDust(Entity.Center, dustPos-5, dustPos-5, DustID.Snow, dustVelocity.X * 2, dustVelocity.Y * 2, 0, Color.White, 9f);
                    Main.dust[shiva2].noGravity = true;
                    Main.dust[shiva2].fadeIn = 5f;
                    Main.dust[shiva2].velocity *= 3f;
                }
            }

            // armlet timer
            if (KeybindSystem.ArmletKeybind.JustPressed && armletActive) //&& !Player.HasBuff(buff2))
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
        }
        public override void UpdateBadLifeRegen()
        {
            if (armletOn && Player.HasBuff<ArmletOfMordiggianBuff>())
            {
                Player.lifeRegen += -30;
            }
        }

        public override void UpdateLifeRegen()
        {
           if (Player.HeldItem.Name == "Berserker's Spear")
            {
                float percentLifeLeft = (float)Player.statLife / Player.statLifeMax2;
                Player.lifeRegen += Convert.ToInt32((1 / (percentLifeLeft + .065)));
            }
        }
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
            wearingBladeMail = false;
            wearingShivasGuard = false;
            wearingEyeOfSkadi = false;

                diademMinion = false;
            dominatorMinion = false;
            overlordMinion = false;


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
    }
}