using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class BloodGrenade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 20;
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 8f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 8f;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<BloodGrenadeProjectile>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, System.Int32 type, System.Int32 damage, System.Single knockback)
        {
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " poured their lifeforce into a blood grenade."), 5, -player.direction, false, false, -1, false, 9999, 0, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient(ItemID.Grenade, 50).
                AddIngredient<CreepBlood>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}