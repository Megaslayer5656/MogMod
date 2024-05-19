using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
        public class SigmaCharm : ModItem
    {
        public override void SetDefaults()
        {
           Item.accessory = true;
           Item.rare = 8;
           Item.value = 15000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wereWolf = true;
            player.team = 0;
        }
    }
}