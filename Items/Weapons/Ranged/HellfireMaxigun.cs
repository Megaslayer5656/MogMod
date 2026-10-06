using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class HellfireMaxigun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int BuiltUpHeat = 0;
        public const int OverheatLevel = 540;
        //public const int OverheatCooldown = 240;
        //public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(OverheatCooldown.FramesToSeconds());
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 30;

            Item.damage = 50;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<HellfireMaxigunHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player) => false;
        public override bool RangedPrefix() => true;
        public override void AddRecipes() // adamantite tier, pre-mech
        {
            CreateRecipe().
               AddIngredient(ItemID.Gatligator).
               AddIngredient<HellfireBar>(12).
               AddIngredient<ScorchedCore>().
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}