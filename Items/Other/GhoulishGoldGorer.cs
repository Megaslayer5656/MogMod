using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Other
{
    public class GhoulishGoldGorer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Yellow;
            // (platcoin, goldcoin, silvercoin, coppercoin)
            Item.value = Item.buyPrice(0, 5, 96, 0);
        }
    }
}