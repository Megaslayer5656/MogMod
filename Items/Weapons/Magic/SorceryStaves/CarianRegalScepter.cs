using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class CarianRegalScepter : SorceryStaff
    {
        public override float ManaCostMult => 0.5f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 55;
            Item.width = Item.height = 76;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 42;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(5).
                AddIngredient<ManaCore>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}