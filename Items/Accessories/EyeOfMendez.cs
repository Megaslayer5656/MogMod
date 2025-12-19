using MogMod.Common.Player;
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
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 500;
            player.statManaMax2 += 500;
            player.GetDamage(DamageClass.Generic) += 25f;
            player.GetAttackSpeed(DamageClass.Generic) += 50f;
            player.maxMinions += 500;
            player.maxTurrets += 500;
        }
    }
}
