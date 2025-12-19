using MogMod.Common.Player;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                AddIngredient<UltimateOrb>(1).
                AddIngredient(ItemID.ChlorophyteBar, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
