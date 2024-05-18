using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Other
{
    public class LedX : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 5;
            Item.height = 5;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Red;
            Item.value = 10000000;
        }
    }
}