using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.Other
{
    public class SplinterAmmo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public const int BloodDamage = 10;
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 6;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 0, 0, 7);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<SplinterProjectile>();
            Item.shootSpeed = 1f;
            Item.ammo = ItemID.Nail;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = BloodDamage;
        }
        public override void AddRecipes()
        {
            CreateRecipe(100).
            AddRecipeGroup(RecipeGroupID.Wood, 10).
            AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Mushroom"}", 1).
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}
