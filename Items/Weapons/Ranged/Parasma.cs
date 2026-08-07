using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Parasma : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 52;
            Item.damage = 44;
            Item.knockBack = 7;
            Item.shootSpeed = 12;
            Item.useTime = Item.useAnimation = 18;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<ParasmaProj>();
            Item.ArmorPenetration = 20;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WitchBlade>().
                AddRecipeGroup("AnyEvilWater", 20).
                AddIngredient(ItemID.SpiderFang, 15).
                AddIngredient(ItemID.ChlorophyteBar, 12).
                AddRecipeGroup("AnyEvilMushroom", 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
