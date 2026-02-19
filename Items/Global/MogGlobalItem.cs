using MogMod.Items.Weapons.Melee;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Global
{
    public class MogGlobalItem : GlobalItem
    {
        public int bloodDamage;

        public override void SetDefaults(Item entity)
        {
            if (entity.type == ModContent.ItemType<Reduvia>())
            {
                bloodDamage = 33;

            } else if (entity.type == ModContent.ItemType<Sange>())
            {
                bloodDamage = 110;

            } else
            {
                bloodDamage = 0;

            }
        }

        public override bool InstancePerEntity => true;
    }
}
