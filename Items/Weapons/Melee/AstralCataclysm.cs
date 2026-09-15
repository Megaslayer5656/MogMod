using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // 92x110
    // charged greatsword that launches out 3-5 stars that come to a stop
    // sword can hit stars again to launch them again and upgrade them into stronger stars
    // think holy collider calamity
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