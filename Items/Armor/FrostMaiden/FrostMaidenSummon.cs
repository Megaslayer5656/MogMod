using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.FrostMaiden
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostMaidenSummon : ModItem, ILocalizedModType
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
            Item.width = 24;
            Item.height = 22;
            Item.defense = 1;
            Item.rare = ItemRarityID.Green;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FrostMaidenRobe>() && legs.type == ModContent.ItemType<FrostMaidenPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFrostArmor = true;
            player.setBonus = SetBonusText.Value;
            player.maxMinions += 1;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.07f;
            player.maxMinions += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrigidShard>(4).
                AddIngredient<ManaEssence>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}