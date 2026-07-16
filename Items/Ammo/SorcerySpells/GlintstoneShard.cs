using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class GlintstoneShard : SorcerySpell
    {
        public override int ManaCost => 2;
        public override int AttackSpeed => 20;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 9;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.shoot = ModContent.ProjectileType<GlintstoneShardProj>();
            Item.shootSpeed = 8f;
            SorceryClass = SorceryID.Glintstone;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 30)
                .AddIngredient(ItemID.Emerald, 2)
                .AddIngredient<Scroll>(1)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}