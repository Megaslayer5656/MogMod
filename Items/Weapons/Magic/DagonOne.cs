using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonOne : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static Color WeakColor => new(255, 218, 140);
        public static Color StrongColor => new(255, 188, 105);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;

            Item.damage = 32;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 1.5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<DagonOneHoldout>();
            Item.shootSpeed = 2f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WandofSparking).
                AddRecipeGroup("AnyTorch", 20).
                AddIngredient<ManaEssence>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}