using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class GlintstoneStaff : SorceryStaff
    {
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 7;
            Item.width = Item.height = 32;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.autoReuse = true;
        }
        public override bool MagicPrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(RecipeGroupID.Wood, 12).
                AddIngredient<ManaEssence>().
                AddIngredient<FrigidShard>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}