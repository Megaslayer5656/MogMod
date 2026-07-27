using MogMod.Items.Accessories.NeutralItems;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Common.Systems
{
    public class NeutralItemSlot : ModAccessorySlot
    {
        public static LocalizedText NeutralText { get; private set; }
        public static LocalizedText NeutralDyeText { get; private set; }
        public override void SetupContent()
        {
            NeutralText = Mod.GetLocalization($"{nameof(NeutralItemSlot)}.Neutral");
            NeutralDyeText = Mod.GetLocalization($"{nameof(NeutralItemSlot)}.NeutralDye");
        }
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return checkItem.ModItem is NeutralItem; // if is a NeutralItem, then can go in slot
        }
        // Designates our slot to be a priority for putting neutral items in to. NOTE: use ItemLoader.CanEquipAccessory if aiming for restricting other slots from having neutral items!
        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
        {
            return item.ModItem is NeutralItem; // If is neutral item, then we want to prioritize it to go in to our slot.
        }
        public override bool IsEnabled()
        {
            return true;
        }
        // Overrides the default behavior where a disabled accessory slot will allow retrieve items if it contains items
        public override bool IsVisibleWhenNotEnabled()
        {
            return false; // We set to false to just not display if not Enabled. NOTE: this does not affect behavior when mod is unloaded!
        }
        // Icon textures. Nominal image size is 32x32. Will be centered on the slot.
        public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.FeralClaws;

        // Can be used to modify stuff while the Mouse is hovering over the slot.
        public override void OnMouseHover(AccessorySlotType context)
        {
            // We will modify the hover text while an item is not in the slot, so that it says "neutral items".
            switch (context)
            {
                case AccessorySlotType.FunctionalSlot:
                case AccessorySlotType.VanitySlot:
                    Main.hoverItemName = NeutralText.Value;
                    break;
                case AccessorySlotType.DyeSlot:
                    Main.hoverItemName = NeutralDyeText.Value;
                    break;
            }
        }
    }
}