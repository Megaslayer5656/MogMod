using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Runty
{
    [AutoloadEquip(EquipType.Head)]
    public class RuntyHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            // set bonus text
            SetBonusText = this.GetLocalization("SetBonus");
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 18;

            Item.defense = 1; // 5

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<RuntyBreastplate>() && legs.type == ModContent.ItemType<RuntyGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            player.statDefense += 2;
            player.GetDamage<GenericDamageClass>().Flat += 2f;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>().Flat += 1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}