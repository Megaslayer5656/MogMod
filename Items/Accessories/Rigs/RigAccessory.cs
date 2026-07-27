using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Rigs
{
    public abstract class ChestRig : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Rigs";
        public override void SetDefaults()
        {
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingRigSlot = true;
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            var rigs = MogGlobalItem.ChestRigAccessories;
            if (rigs.Contains(incomingItem.type) && rigs.Contains(equippedItem.type))
                return false;
            return true;
        }
        // add a custom tooltip line
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var player = Main.LocalPlayer;
            if (player != null)
            {
                var rigLine = new TooltipLine(Mod, "RigAccessory", "Chest Rig");
                tooltips.Insert(1, rigLine);
            }
        }
    }
}