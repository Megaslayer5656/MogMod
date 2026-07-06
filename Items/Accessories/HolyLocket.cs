using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
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
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .07f;
            player.GetDamage(DamageClass.Summon) += .07f;
            player.statManaMax2 += 70;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.locketActive = true;
            float dim = .02f;
            Lighting.AddLight(player.Center, 75 * dim, 73 * dim, 61 * dim);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagicWand>(1).
                AddIngredient(ItemID.PygmyNecklace, 1).
                AddIngredient<Diadem>(1).
                AddIngredient<SolRing>(1).
                AddRecipeGroup("AdamantiteBar", 8).
                AddIngredient(ItemID.SoulofSight, 7).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
