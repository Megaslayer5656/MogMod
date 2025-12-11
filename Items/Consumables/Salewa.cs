using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class Salewa : ModItem
    {
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.value = 30000;
            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.useStyle = ItemUseStyleID.EatFood;
        }
    }
}
