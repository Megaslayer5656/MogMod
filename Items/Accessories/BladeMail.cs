using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class BladeMail : ModItem, ILocalizedModType
    {
        // TODO: turn this into an armor set
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.BladeMailKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 7;
            player.GetDamage(DamageClass.Generic) += .05f;
            player.thorns += 1f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingBladeMail = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FalconBlade, 1).
                AddRecipeGroup(RecipeGroupID.IronBar, 25).
                AddIngredient(ItemID.Spike, 15).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
