using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class AzursGlintstoneStaff : SorceryStaff
    {
        public override float AttackSpeedMult => 1.4f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 42;
            Item.width = Item.height = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 17;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrinyRind>(15).
                AddRecipeGroup("MythrilBar", 12).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}