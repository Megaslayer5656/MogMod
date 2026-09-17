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
    public class BladeOfSelves : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static Color MainColor1 = Color.Pink;
        public static Color MainColor2 = Color.Goldenrod;
        public override int ProjectileType => ModContent.ProjectileType<BladeOfSelvesHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 46;

            Item.damage = 94;
            Item.knockBack = 12f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EchoSabre>().
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}