using Microsoft.Xna.Framework;
using MogMod.Buffs;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
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

        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
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
        }

        public override void OnHitByProjectile(Projectile proj, Terraria.Player.HurtInfo hurtInfo)
        {
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
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // give debuff cd , 600 = 10 seconds
            int debuff1 = ModContent.BuffType<Buffs.GlimmerCapeDebuff>();

            if (KeybindSystem.GlimmerCapeKeybind.JustPressed && isWearingGlimmerCape && !Player.HasBuff(debuff1))
            {
                int buff1 = ModContent.BuffType<Buffs.GlimmerCapeBuff>();
                Player.AddBuff(buff1, 1800);
                Player.AddBuff(debuff1, 3600);
                // Main.NewText("applied glimmer cape"); //RandomBuffText.Format(Lang.GetBuffName(buff)));
            }

            // best code to ctrlv ctrlc
            int debuff3 = ModContent.BuffType<Buffs.SatanicDebuff>();
            if (KeybindSystem.SatanicKeybind.JustPressed && wearingSatanic && !Player.HasBuff(debuff1))
            {
                int buff3 = ModContent.BuffType<Buffs.SatanicBuff>();
                Player.AddBuff(buff3, 1800);
                Player.AddBuff(debuff3, 3600);
            }

            // arcane boots
            int debuff4 = ModContent.BuffType<Buffs.ArcaneBootsDebuff>();
            if (KeybindSystem.ArcaneBootsKeybind.JustPressed && wearingManaBoots && !Player.HasBuff(debuff4))
            {
                // make it play a sound when activating
                Player.statMana += 200;
                Player.AddBuff(debuff4, 1800);
            }

            // refresher orb
            int debuff5 = ModContent.BuffType<Buffs.RefresherOrbDebuff>();
            if (KeybindSystem.RefresherOrbKeybind.JustPressed && wearingRefresherOrb && !Player.HasBuff(debuff5))
            {
                // make it play a sound when activating (add any additional debuffs here unless it shouldn't be cleared by refresher)
                Player.ClearBuff(debuff1);
                Player.ClearBuff(debuff3);
                Player.ClearBuff(debuff4);
                Player.AddBuff(debuff5, 9000);
            }

            int helmOfDominator = ModContent.BuffType<Buffs.HelmOfDominatorDebuff>();
            if (KeybindSystem.HelmOfDominatorKeybind.JustPressed && wearingHelmOfDominator && !Player.HasBuff(helmOfDominator))
            {
                // for now it summons a mount (change to make it summon a friendly npc to damage enemies)
                Player.AddBuff(BuffID.BasiliskMount, 1);
                Player.AddBuff(helmOfDominator, 1800);
            }

            //Wand
            int buff4 = ModContent.BuffType<WandBuff>();
            if (KeybindSystem.WandKeybind.JustPressed)
            {
                if (wandActive)
                {
                    Player.AddBuff(buff4, 60);
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            //Magic Stick
            int buff5 = ModContent.BuffType<MagicStickBuff>();
            if (KeybindSystem.MagicStickKeybind.JustPressed)
            {
                if (stickActive)
                {
                    Player.AddBuff(buff5, 60);
                    SoundEngine.PlaySound(WandUse, Player.Center);
                }
            }

            // armlet timer
            int buff2 = ModContent.BuffType<Buffs.ArmletOfMordiggianBuff>();
            if (KeybindSystem.ArmletKeybind.JustPressed && armletActive) //&& !Player.HasBuff(buff2))
            {
                if (armletTimer <= armletTimerMax)
                {
                    Player.AddBuff(buff2, 9999999);
                    armletTimer += 1;
                    armletOn = true;
                    SoundEngine.PlaySound(ArmletOnSound, Player.Center);
                } else if (armletTimer >= armletTimerMax)
                {
                    Player.ClearBuff(buff2);
                    armletOn = false;
                    armletTimer = 0;
                    SoundEngine.PlaySound(ArmletOffSound, Player.Center);
                }
            }

            while (armletTimer >= 1 && armletTimer <= armletTimerMax + 1)
            {
                armletTimer += 1;
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (armletOn)
            {
                Player.lifeRegen += -30;
            }
        }
        public override void ResetEffects()
        {
            isWearingGlimmerCape = false;
            armletActive = false;
            wearingManaBoots = false;
            wearingSatanic = false;
            wearingRefresherOrb = false;
            wandActive = false;
            stickActive = false;
            wearingHelmOfDominator = false;
        }
    }
}