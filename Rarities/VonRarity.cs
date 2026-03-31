﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Rarities
{
    public class VonRarity : ModRarity
    {
        public override Color RarityColor => new Color(Main.DiscoR / 5, (byte)(Main.DiscoG / 0f), (byte)(Main.DiscoB / 5f));
        public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
        {
            -2 => ItemRarityID.Red,
            -1 => ItemRarityID.Purple,
            1 => ModContent.RarityType<VonRarity>(),
            2 => ModContent.RarityType<VonRarity>(),
            _ => Type,
        };
    }
}