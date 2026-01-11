using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
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
    public class EyeOfMendez : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.buyPrice(1000, 0, 0, 67);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 1500;
            player.statManaMax2 += 2000;
            player.GetDamage(DamageClass.Generic) += 25f;
            player.GetAttackSpeed(DamageClass.Generic) += 50f;
            player.maxMinions += 500;
            player.maxTurrets += 500;
            player.statDefense += 500;
            player.aggro += -2500;
            player.endurance *= 5f;
        }
    }
}
