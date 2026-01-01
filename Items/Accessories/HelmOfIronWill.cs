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
    public class HelmOfIronWill : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.statLifeMax2 += 20;
            player.manaRegen -= (int)Math.Round(player.manaRegen * .4f);
            player.manaRegenDelay += 5f;
            player.GetDamage(DamageClass.Magic) += -.10f;
            player.GetDamage(DamageClass.Summon) += -.10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("IronBar", 20).
                AddRecipeGroup("GoldBar", 15).
                AddRecipeGroup("SilverBar", 12).
                AddIngredient(ItemID.LargeDiamond, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
