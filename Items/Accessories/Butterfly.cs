using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Butterfly : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .20f;
            player.GetAttackSpeed(DamageClass.Generic) += .35f;
            player.GetDamage(DamageClass.Generic) += .25f;
            Random random = new Random();
            int rand100 = random.Next(100);
            if (rand100 <= 35)
            {
                player.shadowDodge = true;
                player.shadowDodgeTimer = 3;
            }
            else
            {
                player.shadowDodge = false;
            }
        }
    }
}
