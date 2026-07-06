using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class SplinterAmmo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
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
