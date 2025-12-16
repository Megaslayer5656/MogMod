using MogMod.Common.Player;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
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
    public class ShivasGuard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 10;
            player.GetDamage(DamageClass.Ranged) -= .30f;
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Generic) += .10f;
            player.GetAttackSpeed(DamageClass.Generic) += .10f;
            player.lifeRegen += 4;
            player.statManaMax2 += 50;
            player.statLifeMax2 += 50;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingShivasGuard = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VeilOfDiscord>(1).
                AddIngredient(ItemID.TitaniumBreastplate, 1).
                AddIngredient(ItemID.FrostCore, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient<VeilOfDiscord>(1).
                AddIngredient(ItemID.AdamantiteBreastplate, 1).
                AddIngredient(ItemID.FrostCore, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
