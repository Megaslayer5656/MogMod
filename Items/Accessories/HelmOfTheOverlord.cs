using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfTheOverlord : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.HelmOfDominatorKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .15f;
            player.GetDamage(DamageClass.Summon) += .15f;
            player.statManaMax2 += 100;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.overlordMinion = true;
            mogPlayer.dominatorMinion = true;
            mogPlayer.diademMinion = true;
            mogPlayer.wearingHelmOfOverlord = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfTheDominator>(1).
                AddIngredient(ItemID.ChlorophyteBar, 7).
                AddIngredient<GriefBar>(7).
                AddIngredient<FaeBar>(7).
                AddIngredient<ManaCore>(1).
                AddIngredient<FrigidCrystal>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
