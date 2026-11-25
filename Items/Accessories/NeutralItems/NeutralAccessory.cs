using MogMod.Common.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    /// <summary>
    /// Abstract class for accessories that can only be equipped in the "Neutral Item Slot" <br/>
    /// Automatically sets <see cref="Item.accessory"/> to <see langword="true"/>, then adds a custom tooltip aswell.
    /// </summary>
    public abstract class NeutralItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.NeutralItem";
        public override void SetDefaults()
        {
            Item.accessory = true;
        }
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            //var neutralSlot = ModContent.GetInstance<NeutralItemSlot>();
            if (!modded) return false;
            return true; // May cause issues with other modded accessory slots
            //if (slot == neutralSlot.Type || slot == (neutralSlot.Type + 3)) return true;
            return false;
        }
        // add a custom tooltip line
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var player = Main.LocalPlayer;
            if (player != null)
            {
                var neutralLine = new TooltipLine(Mod, "NeutralItem", "Neutral Item");
                int index = tooltips.FindIndex(x => x.Name == "Equipable" && x.Mod == "Terraria");
                if (index != -1)
                {
                    tooltips.Insert(index++, neutralLine);
                }
            }
        }
    }
}