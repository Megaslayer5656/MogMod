using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class EchoSabre : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static Color MainColor1 = Color.Silver;
        public static Color MainColor2 = Color.LightGreen;
        public override int ProjectileType => ModContent.ProjectileType<EchoSabreHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 60;

            Item.damage = 71;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 40;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyCobaltBar", 22).
                AddIngredient(ItemID.SoulofNight, 6).
                AddIngredient<FrigidCrystal>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}