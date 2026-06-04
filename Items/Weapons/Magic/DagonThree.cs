using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonThree : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 88;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 32;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DagonThreeProj>();
            Item.shootSpeed = 20f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DagonTwo>(1).
                AddIngredient<PointBooster>(1).
                AddIngredient(ItemID.MagmaStone, 1).
                AddIngredient(ItemID.LivingFireBlock, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}