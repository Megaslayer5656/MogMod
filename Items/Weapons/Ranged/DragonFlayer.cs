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
    public class DragonFlayer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 118;
            Item.height = 64;

            Item.damage = 65;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;

            Item.useTime = 3;
            Item.reuseDelay = 3;
            Item.useAnimation = 12;
            Item.useLimitPerAnimation = 4;

            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 22f;
            Item.shoot = ModContent.ProjectileType<DragonFlayerProj>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-46, 0);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(7).
                AddIngredient<ScorchedCore>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
