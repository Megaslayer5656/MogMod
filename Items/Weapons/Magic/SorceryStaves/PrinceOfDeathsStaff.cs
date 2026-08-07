using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class PrinceOfDeathsStaff : SorceryStaff
    {
        public override float AttackSpeedMult => 1.2f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 30;
            Item.width = Item.height = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient(ItemID.Ectoplasm, 8).
                AddRecipeGroup("AnyTombstone", 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}