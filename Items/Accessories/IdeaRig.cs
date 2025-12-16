using System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.UI;
using MogMod.UI;
using Terraria.GameContent.UI.Elements;
using System.Security.Cryptography.X509Certificates;
using MogMod.Items.Other;
using Terraria.GameContent.ItemDropRules;

namespace MogMod.Items.Accessories
{
    public class IdeaRig : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public CustomItemSlot idearigslot;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 25000;
            Item.waistSlot = 1;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ammoCost75 = true;
            player.lifeRegen = 1;
        }
    }
}
