using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class HellfireCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 30;

            Item.damage = 104;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;

            Item.useTime = Item.useAnimation = 32;

            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<HellfireCannonProj>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item61;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HellfireBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}