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
    public class CrescentMoon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int maxShots = 6;
        public const int reloadTime = 80;
        ModKeybind keybindActive = null;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots);
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;

            Item.damage = 150;
            Item.knockBack = 8.5f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<CrescentMoonHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override bool CanUseItem(Player player) => false;
        public override bool RangedPrefix() => true;
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.FirstWeaponKeybind);
    }
}