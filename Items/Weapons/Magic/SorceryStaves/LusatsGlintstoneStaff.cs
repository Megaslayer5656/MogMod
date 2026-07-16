using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class LusatsGlintstoneStaff : SorceryStaff
    {
        public override float KnockbackMult => 1.5f;
        public override float ManaCostMult => 1.5f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 66;
            Item.width = Item.height = 74;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 21;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentNebula, 8).
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}