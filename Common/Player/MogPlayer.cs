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
        public bool isWearingGlimmerCape = false;
        public enum MewingType
        {
            mewingguide = 0
        }
        public MewingType mewingType = MewingType.mewingguide;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
                int debuff1 = ModContent.BuffType<Buffs.GlimmerCapeDebuff>();
            // give debuff cd
            if (KeybindSystem.GlimmerCapeKeybind.JustPressed && isWearingGlimmerCape && !Player.HasBuff(debuff1))
                {
                int buff1 = ModContent.BuffType<Buffs.GlimmerCapeBuff>();
                Player.AddBuff(buff1, 600);
                Player.AddBuff(debuff1, 600);
                // Main.NewText("applied glimmer cape"); //RandomBuffText.Format(Lang.GetBuffName(buff)));
            }
        }

        public override void ResetEffects()
        {
            isWearingGlimmerCape = false;
        }
    }

}