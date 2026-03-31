using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Utilities;

namespace MogMod.Items.Armor.Blademail
{

    [AutoloadEquip(EquipType.Body)]
    public class BlademailBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.BladeMailKeybind);
        ModKeybind keybindActive = null;
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 28;
            Item.defense = 10;
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.08f;
            player.GetCritChance<MeleeDamageClass>() += 8;
            player.thorns += .3f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChippedVest>(1).
                AddIngredient(ItemID.Spike, 40).
                AddIngredient(ItemID.Bone, 55).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}