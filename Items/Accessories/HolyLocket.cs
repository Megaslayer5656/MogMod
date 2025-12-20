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
    public class HolyLocket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.WandKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Summon) += .10f;
            player.statManaMax2 += 70;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.locketActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagicWand>(1).
                AddIngredient<Diadem>(1).
                AddIngredient(ItemID.Glass, 20).
                AddRecipeGroup("AdamantiteBar", 15).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
