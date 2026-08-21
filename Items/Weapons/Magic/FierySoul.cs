using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class FierySoul : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int FierySoulMax = 30;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FierySoulMax);
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 44;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 24;
            Item.mana = 8;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<FierySoulProjectile>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item20;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Book, 1).
                AddIngredient(ItemID.FlowerofFire, 1).
                AddIngredient(ItemID.Fireblossom, 3).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}