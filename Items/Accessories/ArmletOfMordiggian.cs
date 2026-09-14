using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories.Boots;
using MogMod.Items.Accessories.NeutralItems.Aspects;
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
        public const float DamageMult = 0.05f;
        public const float ArmletDamageMult = 0.15f;
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
            player.GetAttackSpeed(DamageClass.Generic) += DamageMult;
            player.GetDamage(DamageClass.Generic) += DamageMult;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.armletActive = true;
            if (Main.zenithWorld) mogPlayer.armletDebuff = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.ArmletKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<ArmletOfMordiggian>("TooltipGFB").Format());
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<ArmletOfMordiggian>("TooltipNormal").Format(
                    DamageMult.ToPercent(), 
                    Hotkey,
                    ArmletDamageMult.ToPercent()));
                    tooltips.Insert(index, normal);
                }
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