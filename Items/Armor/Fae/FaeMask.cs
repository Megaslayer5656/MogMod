using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Fae
{
    [AutoloadEquip(EquipType.Head)]
    public class FaeMask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public static double FlightTimeBoost = 0.5D;
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
            Item.width = 22;
            Item.height = 24;

            Item.defense = 14; // 50

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FaeBreastplate>() && legs.type == ModContent.ItemType<FaeGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFaeArmor = true;
            player.wingTimeMax = (int)(player.wingTimeMax * 1.5f);
        }
        // should atleast be better than crystal assassin
        // also post EOL so it should be really good
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetCritChance<GenericDamageClass>() += 12;
            player.statManaMax2 += 60;
            player.manaCost *= 0.88f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(10).
                AddIngredient(ItemID.CrystalNinjaHelmet, 1). // might replace with something else
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}