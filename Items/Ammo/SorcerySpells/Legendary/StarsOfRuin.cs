using MogMod.Items.Ammo.SorcerySpells.Glintstone;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Legendary
{
    public class StarsOfRuin : SorcerySpell
    {
        public override int ManaCost => 28;
        public override int AttackSpeed => 50;
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<FoundingRainOfStars>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 20;
            Item.knockBack = 6f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<StarsOfRuinProj>();
            Item.shootSpeed = 6f;
            SorceryClass = SorceryID.Legendary;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StarShower>().
                AddIngredient<SpookyEssence>(8).
                AddIngredient<ManaCore>().
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                DisableDecraft().
                Register();
        }
    }
}