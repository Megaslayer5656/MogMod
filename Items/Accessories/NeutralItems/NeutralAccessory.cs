using MogMod.Common.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public abstract class NeutralItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.NeutralItem";
        public override void SetDefaults()
        {
            Item.accessory = true;
        }
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (!modded)
                return false;
            var neutralSlot = ModContent.GetInstance<NeutralItemSlot>();
            if (slot == neutralSlot.Type) // TODO: allow this to be equipped into vanity slot without having to right click switch from the functional slot
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