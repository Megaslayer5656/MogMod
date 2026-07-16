using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class AlbinauricStaff : SorceryStaff
    {
        public override float VelocityMult => 1.2f;
        public override float ManaCostMult => 0.9f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 19;
            Item.width = Item.height = 54;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritShard>(7).
                AddIngredient(ItemID.Lens, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}