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
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 6f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<StarsOfRuinProj>();
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