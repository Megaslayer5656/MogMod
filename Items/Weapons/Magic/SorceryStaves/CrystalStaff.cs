using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class CrystalStaff : SorceryStaff
    {
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 32;
            Item.width = Item.height = 66;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 36;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyCobaltBar", 12).
                AddIngredient<ManaEssence>(5).
                AddIngredient<FrigidCrystal>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}