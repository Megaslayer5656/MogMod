using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class WarFury : ModItem, ILocalizedModType // ILocalizedModType is needed so that LocalizationCategory actually works
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.damage = 58;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.shootSpeed = 16f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 38, 0, 0);
            Item.axe = 150 / 5;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<WarFuryProjectile>();
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float breakBlocks = 1;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f, breakBlocks);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BattleFury>(1).
                AddIngredient(ItemID.LivingFireBlock, 12).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}