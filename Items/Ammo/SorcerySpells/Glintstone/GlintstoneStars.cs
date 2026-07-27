using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Glintstone
{
    public class GlintstoneStars : SorcerySpell
    {
        public override int ManaCost => 12;
        public override int AttackSpeed => 40;
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 20;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.shoot = ModContent.ProjectileType<GlintstoneStarsProj>();
            Item.shootSpeed = 6f;
            SorceryClass = SorceryID.Glintstone;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Flesh"}", 10).
                AddIngredient(ItemID.MeteoriteBar, 8).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}