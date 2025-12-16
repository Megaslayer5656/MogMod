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
    public class MewingGuide : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateEquip(Player player)
        {
        }
    }
}