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
    public class EyeOfSkadi : ModItem, ILocalizedModType
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
            player.statLifeMax2 += 50;
            player.statManaMax2 += 100;
            player.GetDamage(DamageClass.Generic) += .15f;
            // change to .35
            player.GetAttackSpeed(DamageClass.Generic) += 35f;
            // remove
            player.maxMinions += 500;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingEyeOfSkadi = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UltimateOrb>(1).
                AddIngredient<PointBooster>(1).
                AddIngredient(ItemID.Ectoplasm, 10).
                AddIngredient(ItemID.TitaniumBar, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
            CreateRecipe().
                AddIngredient<UltimateOrb>(1).
                AddIngredient<PointBooster>(1).
                AddIngredient(ItemID.Ectoplasm, 10).
                AddIngredient(ItemID.AdamantiteBar, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
