using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class RykardsRancor : SorcerySpell
    {
        public override int ManaCost => 32;
        public override int AttackSpeed => 52;
        public override SoundStyle UseSound => SoundID.Item73;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 60;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
            Item.shoot = ModContent.ProjectileType<RykardsRancorProj>();
            Item.shootSpeed = 8f;
            SorceryClass = SorceryID.Magma;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient<HellfireEssence>(5)
                .AddIngredient<Scroll>()
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}