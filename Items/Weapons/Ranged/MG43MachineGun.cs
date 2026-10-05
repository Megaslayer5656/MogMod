using Microsoft.Xna.Framework;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class MG43MachineGun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int maxShots = 175;
        public const int reloadTime = 180;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots, reloadTime.FramesToSeconds());
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 22;

            Item.damage = 40;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<MG43MachineGunHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override bool CanUseItem(Player player) => false;
        public override bool RangedPrefix() => true;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // display the rpm in the tooltip
            var effectDescTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[RPM]") && x.Mod == "Terraria");
            int fireRate = (Main.LocalPlayer.MogMod().mg43RPM * 2) + 10;

            tooltips.IntegrateHotkey(KeybindSystem.FirstWeaponKeybind);

            if (effectDescTooltip != null)
                effectDescTooltip.Text = Main.zenithWorld ? effectDescTooltip.Text = effectDescTooltip.Text.Replace("[RPM]", this.GetLocalizedValue("GFBRPM")) : effectDescTooltip.Text = effectDescTooltip.Text.Replace("[RPM]", $"{60 / fireRate * 175}");
        }
        public override void AddRecipes() // adamantite tier, pre-mech
        {
            CreateRecipe().
               AddIngredient<R8Revolver>().
               AddRecipeGroup("AnyAdamantiteBar", 16).
               AddIngredient<FuciumBar>(10).
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}