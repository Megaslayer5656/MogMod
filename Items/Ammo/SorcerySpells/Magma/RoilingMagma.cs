using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Magma
{
    public class RoilingMagma : SorcerySpell
    {
        public override int ManaCost => 16;
        public override int AttackSpeed => 38;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 42;
            Item.knockBack = 8f;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.shoot = ModContent.ProjectileType<RoilingMagmaProj>();
            Item.shootSpeed = 12f;
            SorceryClass = SorceryID.Magma;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 8).
                AddIngredient(ItemID.Dynamite, 5).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}