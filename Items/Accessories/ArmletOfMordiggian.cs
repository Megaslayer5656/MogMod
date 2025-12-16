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
    public class ArmletOfMordiggian : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ArmletKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 7;
            player.GetAttackSpeed(DamageClass.Generic) += .10f;
            player.GetDamage(DamageClass.Generic) += .10f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.armletActive = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddIngredient(ItemID.FeralClaws, 1).
                AddIngredient<BladesOfAttack>().
                AddIngredient(ItemID.TitaniumBar, 10).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddIngredient(ItemID.FeralClaws, 1).
                AddIngredient<BladesOfAttack>().
                AddIngredient(ItemID.AdamantiteBar, 10).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
