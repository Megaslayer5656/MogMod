using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace MogMod.Items.Other
{
    public class RajangHeart : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            // (platcoin, goldcoin, silvercoin, coppercoin)
            Item.value = Item.buyPrice(6, 50, 0, 0);
        }

        // gets rid of "Master" text in tooltip
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var changedLine = tooltips.FirstOrDefault(x => x.Name == "Master" && x.Mod == "Terraria");
            if (changedLine != null)
            {
                changedLine.Text = "";
            }
        }
    }
}