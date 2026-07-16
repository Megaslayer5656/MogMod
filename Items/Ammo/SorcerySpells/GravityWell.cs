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
    public class GravityWell : SorcerySpell
    {
        public override int ManaCost => 22;
        public override int AttackSpeed => 32;
        public override SoundStyle UseSound => SoundID.Item12;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 44;
            Item.knockBack = 10f;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.shoot = ModContent.ProjectileType<GravityWellProj>();
            Item.shootSpeed = 1f;
            SorceryClass = SorceryID.Gravity;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GlintstonePebble>().
                AddIngredient<FuciumBar>(4).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}