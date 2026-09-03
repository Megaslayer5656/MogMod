using MogMod.Common.Systems;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Blademail
{

    [AutoloadEquip(EquipType.Body)]
    public class BlademailBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int MeleeCritBoost = 8;
        public const float MeleeDamageBoost = 0.08f;
        public const float ThornBoost = 0.3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeCritBoost);
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
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.08f;
            player.GetCritChance<MeleeDamageClass>() += 8;
            player.thorns += .3f;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            BlademailMask.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.ArmorSetBonusKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChippedVest>().
                AddIngredient(ItemID.Spike, 40).
                AddIngredient(ItemID.Bone, 55).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}