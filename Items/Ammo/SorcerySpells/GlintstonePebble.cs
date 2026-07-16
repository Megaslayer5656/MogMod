using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class GlintstonePebble : SorcerySpell
    {
        public override int ManaCost => 5;
        public override int AttackSpeed => 34;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 26;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.shoot = ModContent.ProjectileType<GlintstonePebbleProj>();
            Item.shootSpeed = 6f;
            SorceryClass = SorceryID.Glintstone;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 40)
                .AddIngredient(ItemID.Sapphire, 4)
                .AddIngredient<Scroll>(1)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}