using MogMod.Items.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HeartOfTarrasque : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 100;
            player.statManaMax2 -= 100;
            player.lifeRegen += 8;
            player.statDefense += 20;
            player.shinyStone = true;
            player.GetDamage(DamageClass.Melee) += .10f;
            player.GetDamage(DamageClass.Magic) -= .20f;
            player.GetDamage(DamageClass.Summon) -= .20f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShinyStone, 1).
                AddIngredient(ItemID.CharmofMyths, 1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Hardmode Evil Material"}", 17).
                AddIngredient<VitalityBooster>(1).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
