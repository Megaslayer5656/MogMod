using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class RockSling : SorcerySpell
    {
        public override int ManaCost => 20;
        public override int AttackSpeed => 60;
        public override SoundStyle UseSound => SoundID.Item20;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 54;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<RockSlingProj>();
            Item.shootSpeed = 1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 18).
                AddIngredient(ItemID.Amethyst, 4).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}