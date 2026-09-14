using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Satanic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float DamageBoost = 0.1f;
        public const int MaxLifeBoost = 50;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
            // makes vampire knives crazy
            player.lifeSteal *= 2f;
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingSatanic = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.SatanicKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<Satanic>("TooltipGFB").Format());
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<Satanic>("TooltipNormal").Format(
                    MaxLifeBoost,
                    DamageBoost.ToPercent(),
                    Hotkey));
                    tooltips.Insert(index, normal);
                }
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>().
                AddIngredient(ItemID.MoonStone).
                AddIngredient(ItemID.BeetleHusk, 7).
                AddIngredient<GriefBar>(5).
                AddIngredient<VitalityBooster>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}