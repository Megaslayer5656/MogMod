using MogMod.Common.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    // TODO: fix CanEquipAccessory to only allow neutral items in the neutral item slot-
    // -and not in any modded slot
    public abstract class NeutralItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.NeutralItem";
        public override void SetDefaults()
        {
            Item.accessory = true;
        }
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (modded && slot == ModContent.GetInstance<NeutralItemSlot>().Type)
                return true;
            return false;
        }
        // add a custom tooltip line
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var player = Main.LocalPlayer;
            if (player != null)
            {
                var neutralLine = new TooltipLine(Mod, "NeutralItem", "Neutral Item");
                tooltips.Insert(1, neutralLine);
            }
        }
    }
}