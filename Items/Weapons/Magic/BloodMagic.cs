using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic //Very important note: All of the blood stuff is set up in MogGlobalItem.cs, MogModGlobalProjectileBleed.cs, and MogModGlobalNPC.cs.
{
    public class BloodMagic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodMagicProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
              AddIngredient(ItemID.Book, 1).
              AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 15).
              AddIngredient(ItemID.Deathweed, 5).
              AddTile(TileID.Bookcases).
              Register();
        }
    }
}
