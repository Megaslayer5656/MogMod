using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class AzursGlintstoneStaff : SorceryStaff
    {
        public override float AttackSpeedMult => 1.2f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 42;
            Item.width = Item.height = 80;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 21;
        public override bool MagicPrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrinyRind>(15).
                AddRecipeGroup("MythrilBar", 12).
                AddIngredient<ManaCore>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}