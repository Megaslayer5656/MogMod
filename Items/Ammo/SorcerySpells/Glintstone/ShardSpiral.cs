using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Glintstone
{
    public class ShardSpiral : SorcerySpell
    {
        public override int ManaCost => 16;
        public override int AttackSpeed => 46;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 17;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.shoot = ModContent.ProjectileType<ShardSpiralProj>();
            Item.shootSpeed = 3f;
            SorceryClass = SorceryID.Glintstone;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PixieDust, 15).
                AddRecipeGroup("AnyMythrilBar", 6).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}