using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class WyvernJawblade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const float MaxCharge = 120f;
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 88;

            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 100;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1 with { Pitch = -0.1f };
            Item.shoot = ModContent.ProjectileType<WyvernJawbladeHoldout>();
            Item.shootSpeed = 10f;

            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, 0);
            return false;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FossilOre, 20).
                AddIngredient(ItemID.Leather, 5).
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}