using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Global
{
    public partial class MogGlobalItem : GlobalItem
    {
        /// <summary> How much blood damage this item does. Higher values lead to faster blood procs.
        /// <br/> Automatically adds a tooltip after "Knockback" to indicate its blood damage based off <see cref="bloodDamage"/>.
        /// <br/> Does not use number values like "Damage" to allow for better understanding from the players perspective. 
        /// <br/> If this item does not deal blood damage and is instead used to fire a projectile that does blood damage, use <see cref="visualBloodDamage"/> instead. </summary>
        /// <remarks> Less than 30 <see cref="bloodDamage"/> sets the bleed tooltip to "Low".
        /// <br/> Less than 80 <see cref="bloodDamage"/> sets the bleed tooltip to "Medium".
        /// <br/> Greater than or equal to 80 <see cref="bloodDamage"/> sets the bleed tooltip to "High". </remarks>
        public int bloodDamage;
        /// <summary> Behaves the same as <see cref="bloodDamage"/>, but is only used for tooltips. </summary>
        /// <remarks> Use this value if the item fires a projectile that deals blood damage, but the item itself does no blood damage.
        /// <br/> Modifying this value will have no effect on gameplay. </remarks>
        public int visualBloodDamage;
        public void BloodDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.BloodButcherer:
                    bloodDamage = 16;
                    break;
                case ItemID.PsychoKnife:
                    bloodDamage = 95;
                    break;
            }
        }
        public void ApplyBleedTooltips(Item item, List<TooltipLine> tooltips)
        {
            void AddBloodTooltip(int blood)
            {
                int kbIndex = tooltips.FindIndex(x => x.Name == "Knockback");
                switch (blood)
                {
                    case < 30:
                        tooltips.Insert(kbIndex + 1, new TooltipLine(Mod, "BleedBuildup", "Low Bleed Buildup"));
                        break;
                    case < 80:
                        tooltips.Insert(kbIndex + 1, new TooltipLine(Mod, "BleedBuildup", "Medium Bleed Buildup"));
                        break;
                    case >= 80:
                        tooltips.Insert(kbIndex + 1, new TooltipLine(Mod, "BleedBuildup", "High Bleed Buildup"));
                        break;
                }
            }
            if (bloodDamage > 0 || visualBloodDamage > 0)
            {
                AddBloodTooltip(bloodDamage > visualBloodDamage ? bloodDamage : visualBloodDamage);
            }
        }
    }
}
