using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class VeilOfDiscord : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .05f;
            player.GetDamage(DamageClass.Generic) += .05f;
            player.GetAttackSpeed(DamageClass.Generic) += .05f;
            player.lifeRegen += 2;
            player.statManaMax2 += 20;
            player.statLifeMax2 += 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>(1).
                AddIngredient<Crown>(1).
                AddIngredient(ItemID.Bone, 40).
                AddIngredient<FuciumBar>(8).
                AddIngredient(ItemID.LargeAmethyst, 1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
