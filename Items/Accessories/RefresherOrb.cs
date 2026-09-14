using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class RefresherOrb : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float MagicAndSummonDamageBoost = 0.1f;
        public const int ManaBoost = 50;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingRefresherOrb = true;
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Summon) += .10f;
            player.statManaMax2 += 50;
            if (Main.zenithWorld)
            {
                player.immune = false;
                player.immuneTime = 0;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.RefresherOrbKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<RefresherOrb>("TooltipGFB").Format());
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<RefresherOrb>("TooltipNormal").Format(
                    MagicAndSummonDamageBoost.ToPercent(),
                    ManaBoost,
                    Hotkey));
                    tooltips.Insert(index, normal);
                }
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 8).
                AddIngredient(ItemID.ChlorophyteBar, 5).
                AddIngredient<FrigidCrystal>(3).
                AddIngredient<ManaCore>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}