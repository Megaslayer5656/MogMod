using MogMod.Common.Config;
using MogMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MogMod.Common
{
    internal class GlobalRigItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.waistSlot > 0;
        public override bool CanEquipAccessory(Item item, Terraria.Player player, int slot, bool modded)  
        {
            return modded || ModContent.GetInstance<RigSlotConfig>().AllowEquippingInOtherSlots;
        }
    }
}
