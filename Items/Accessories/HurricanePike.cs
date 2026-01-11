using MogMod.Common.MogModPlayer;
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
    public class HurricanePike : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ForceStaffKeybind);
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
            player.GetKnockback(DamageClass.Generic) += .30f;
            player.GetAttackSpeed(DamageClass.Generic) += .15f;
            player.GetDamage(DamageClass.Ranged) += .15f;
            player.GetDamage(DamageClass.Magic) += .15f;
            player.GetDamage(DamageClass.Summon) += .15f;
            Player.tileRangeX = Player.tileRangeY += 3;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingPike = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DragonLance>(1).
                AddIngredient<ForceStaff>(1).
                AddIngredient(ItemID.AvengerEmblem, 1).
                AddIngredient(ItemID.ShroomiteBar, 15).
                AddIngredient(ItemID.SoulofMight, 7).
                AddIngredient(ItemID.Silk, 3).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
