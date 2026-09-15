using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class GreatswordOfSouls : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static Color MainColor1 = Color.MediumPurple;
        public static Color MainColor2 = Color.Orchid;
        public override int ProjectileType => ModContent.ProjectileType<GreatswordOfSoulsHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 86;

            Item.damage = 165;
            Item.knockBack = 13f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 40;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<SoulFragment>(5).
                AddIngredient<GriefBar>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}