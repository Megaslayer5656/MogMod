using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class BootsOfTravel : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += .35f;
            player.accRunSpeed = 10f;
            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects(); // This ensures that boot visual effects don't overlap if multiple are equipped
            }
        }
    }
}