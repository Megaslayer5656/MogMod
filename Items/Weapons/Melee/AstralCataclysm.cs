using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class AstralCataclysm : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static Color MainColor1 = new(255, 249, 59);
        public static Color MainColor2 = new(247, 119, 224);
        public static Color MainColor3 = new(40, 105, 240);
        public override int ProjectileType => ModContent.ProjectileType<AstralCataclysmHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 92;
            Item.height = 110;

            Item.damage = 560;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 60;
            Item.knockBack = 16f;
            Item.channel = true;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage *= Main.zenithWorld ? 5f : 1f;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StarWrath).
                AddIngredient<BlackBlade>().
                AddIngredient(ItemID.FallenStar, 12).
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient(ItemID.FragmentStardust, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}