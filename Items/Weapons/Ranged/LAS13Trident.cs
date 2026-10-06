using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class LAS13Trident : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int NumBeams = 6;
        public const int NumAmmoUsed = 15;
        public const int ArmorPenetration = 15;
        public const int OverheatLevel = 220;
        public const int reloadTime = 180;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(NumBeams, NumAmmoUsed, ArmorPenetration);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 24;

            Item.damage = 27;
            Item.knockBack = 2.5f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Gel;
            Item.shoot = ModContent.ProjectileType<LAS13Holdout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override bool CanUseItem(Player player) => false;
        public override bool RangedPrefix() => true;
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.FirstWeaponKeybind);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyAdamantiteBar", 13).
                AddIngredient(ItemID.SoulofMight, 7).
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient(ItemID.GolfCupFlagBlue).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}