using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Carian
{
    public class AdulasMoonblade : SorcerySpell
    {
        public override int ManaCost => 12;
        public override int AttackSpeed => 30;
        public override SoundStyle UseSound => SoundID.Item9;
        public override bool SwordStyle => true;
        public override bool OnlyOneActive => true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 100;
            Item.knockBack = 10f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.shoot = ModContent.ProjectileType<CarianSlicerHoldout>();
            Item.shootSpeed = 8f;
            SorceryClass = SorceryID.Carian;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CarianGreatsword>().
                AddIngredient<FrostEssence>(12).
                AddIngredient(ItemID.FragmentStardust, 8).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}