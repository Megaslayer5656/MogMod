using Microsoft.Xna.Framework;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Common.Player
{
    public class MogPlayer : ModPlayer
    {
        public bool mewing = false;
        public float mewingguide = 0;
        // add this for every buff you apply
        public bool isWearingGlimmerCape = false;
        public bool armletActive = false;
        public int armletTimer = 0;
        public int armletTimerMax = 120;
        public enum MewingType
        {
            mewingguide = 0
        }
        public MewingType mewingType = MewingType.mewingguide;

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

            int buff2 = ModContent.BuffType<Buffs.ArmletOfMordiggianBuff>();

            if (KeybindSystem.ArmletKeybind.JustPressed && armletActive) //&& !Player.HasBuff(buff2))
            {
                if (armletTimer <= armletTimerMax)
                {
                    Player.AddBuff(buff2, 9999999);
                    armletTimer += 1;
                } else if (armletTimer >= armletTimerMax)
                {
                    Player.ClearBuff(buff2);
                    armletTimer = 0;
                }
            }

            while (armletTimer >= 1 && armletTimer <= armletTimerMax + 1)
            {
                armletTimer += 1;
            }
        }

        public override void ResetEffects()
        {
            isWearingGlimmerCape = false;
            armletActive = false;

        }
    }

}