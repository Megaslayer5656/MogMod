using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class StaffOfTheGuilty : SorceryStaff
    {
        public override int StaffSelfHurtDamage => 4;
        public override float ManaCostMult => 0.8f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 48;
            Item.width = Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 28;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpookyWood, 150).
                AddIngredient(ItemID.Spike, 40).
                AddIngredient<GriefBar>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}