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
    public class SuperLongBarrelledShotgun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int maxShots = 2;
        public const int reloadTime = 100;
        ModKeybind keybindActive = null;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots);
        public override void SetDefaults()
        {
            Item.width = 164;
            Item.height = 18;

            Item.damage = 50;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<SuperLongBarrelledShotgunHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override bool CanUseItem(Player player) => false;
        public override bool RangedPrefix() => true;
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.FirstWeaponKeybind);
    }
}