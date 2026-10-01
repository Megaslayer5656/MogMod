using Microsoft.Xna.Framework;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
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
        public const int maxBarrageShots = 72;
        public const int reloadTime = 80;
        public Color LeftColor = Color.SeaGreen;
        public Color RightColor = Color.Turquoise;
        ModKeybind keybindActive = null;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots, maxBarrageShots);
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;

            Item.damage = 180;
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
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var ReloadHotkey = KeybindSystem.FirstWeaponKeybind.TooltipHotkeyString();
            var UltHotkey = KeybindSystem.SecondWeaponKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<CrescentMoon>("TooltipGFB").Format());
                    gfb.OverrideColor = Color.Lerp(LeftColor, RightColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
                    tooltips.Insert(index++, gfb);
                }
                else
                {
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<CrescentMoon>("TooltipNormal").Format(
                        maxShots,
                        maxBarrageShots,
                        ReloadHotkey,
                        UltHotkey));
                    tooltips.Insert(index++, normal);
                }
                TooltipLine quote = Main.zenithWorld ? new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<CrescentMoon>("TooltipQuoteGFB").Format()) : new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<CrescentMoon>("TooltipQuote").Format());
                quote.OverrideColor = Color.Lerp(RightColor, LeftColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
                tooltips.Insert(index++, quote);
            }
        }
    }
}