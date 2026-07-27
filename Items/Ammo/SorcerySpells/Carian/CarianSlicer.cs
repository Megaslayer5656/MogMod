using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Carian
{
    public class CarianSlicer : SorcerySpell
    {
        public override int ManaCost => 2;
        public override int AttackSpeed => 12;
        public override SoundStyle UseSound => SoundID.Item9;
        public override bool SwordStyle => true;
        public override bool OnlyOneActive => true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 40;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.shoot = ModContent.ProjectileType<CarianSlicerHoldout>();
            Item.shootSpeed = 8f;
            SorceryClass = SorceryID.Carian;
        }
        /* Sold by Traveling Merchant now.
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrigidShard>(6).
                AddIngredient<SpiritShard>(4).
                AddIngredient<ManaEssence>(3).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        } */
    }
}