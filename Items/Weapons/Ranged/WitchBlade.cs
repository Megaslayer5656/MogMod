using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class WitchBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 17;
            Item.knockBack = 5;
            Item.shootSpeed = 8;
            Item.useTime = Item.useAnimation = 15;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<WitchBladeProj>();
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 6;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ThornChakram, 1).
                AddIngredient(ItemID.Deathweed, 12).
                AddIngredient(ItemID.BeeWax, 8).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
