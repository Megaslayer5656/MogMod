using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ArmletOfMordiggian : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
            Item.defense = 7;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .05f;
            player.GetDamage(DamageClass.Generic) += .05f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.armletActive = true;
            if (Main.zenithWorld)
                mogPlayer.armletDebuff = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer != null)
            {
                tooltips.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipDefault"));
                tooltips.IntegrateHotkey(KeybindSystem.ArmletKeybind);
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddIngredient(ItemID.FeralClaws).
                AddIngredient<BladesOfAttack>().
                AddRecipeGroup("AnyEmblem").
                AddRecipeGroup("AnyAdamantiteBar", 10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}