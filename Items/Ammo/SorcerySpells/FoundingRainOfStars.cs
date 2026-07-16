using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class FoundingRainOfStars : SorcerySpell
    {
        public override int ManaCost => 45;
        public override int AttackSpeed => 64;
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StarsOfRuin>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 24;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<FoundingRainOfStarsProj>();
            Item.shootSpeed = 6f;
            SorceryClass = SorceryID.Legendary;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StarShower>().
                AddIngredient<FrostEssence>(8).
                AddIngredient<ManaCore>().
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                DisableDecraft().
                Register();
        }
    }
}