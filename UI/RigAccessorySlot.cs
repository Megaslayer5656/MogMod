using MogMod.Common.Config;
using MogMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;
using MogMod.Common.Player;
using MogMod.UI;
using Terraria.UI;
using MogMod.Items.Accessories;
using MogMod.Common;

namespace MogMod.UI
{
    [Autoload(true)]
    internal class RigAccessorySlot : DraggableAccessorySlots
    {
        public override AccessorySlotsUI UI => RigSlotSystem.UI;
        public override string FunctionalTexture => "Terraria/Images/Item_" + (Mod, "IdeaRig");
        public override string VanityTexture => "Terraria/Images/Item_" + (Mod, "IdeaRig");
        public override bool UseCustomLocation => ModContent.GetInstance<RigSlotConfig>().SlotLocation == RigSlotConfig.Location.Custom;
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return checkItem.waistSlot > 0;
        }
        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
        {
            return item.waistSlot > 0;
        }
        public override void OnMouseHover(AccessorySlotType context)
        {
        }
    }
}
