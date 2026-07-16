using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.FrostMaiden
{

    [AutoloadEquip(EquipType.Body)]
    public class FrostMaidenRobe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int ManaBoost = 80;
        public const float ManaReduction = 0.8f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, ManaReduction.ToReversedPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            //ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            //ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 20;
            Item.defense = 7;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += ManaBoost;
            player.manaCost *= ManaReduction;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<FrostMaidenMagic>())
                FrostMaidenMagic.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<FrostMaidenSummon>())
                FrostMaidenSummon.ModifySetTooltips(this, tooltips);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe, 1).
                AddIngredient(ItemID.Bone, 35).
                AddIngredient<FrigidShard>(7).
                AddIngredient(ItemID.FlinxFur, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}