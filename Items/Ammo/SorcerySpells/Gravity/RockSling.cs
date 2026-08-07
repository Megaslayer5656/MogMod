using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Gravity
{
    public class RockSling : SorcerySpell
    {
        public override int ManaCost => 20;
        public override int AttackSpeed => 60;
        public override SoundStyle UseSound => SoundID.Item20;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 54;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<RockSlingProj>();
            Item.shootSpeed = 1f;
            SorceryClass = SorceryID.Gravity;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyEvilBar", 8).
                AddIngredient(ItemID.Amethyst, 4).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}