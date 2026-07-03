using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles;
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
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StarsOfRuin>();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<FoundingRainOfStarsProj>();
            Item.shootSpeed = 6f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StarShower>(1).
                AddIngredient<FaeBar>(5).
                AddIngredient<ManaCore>(1).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                DisableDecraft().
                Register();
        }
    }
}