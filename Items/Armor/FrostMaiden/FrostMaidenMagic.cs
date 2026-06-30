using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.FrostMaiden
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostMaidenMagic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public static int ShardMax = 50;
        public static double ShardDamage = 0.25D;
        public static int ShardCap = 30;
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
            Item.width = Item.height = 26;
            Item.defense = 6;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FrostMaidenRobe>() && legs.type == ModContent.ItemType<FrostMaidenPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFrostArmor = true;
            mogPlayer.wearingFrostMagic = true;
            player.setBonus = SetBonusText.Value;
            player.manaRegenBonus += 4;
            player.GetDamage<MagicDamageClass>() += 0.12f;
        }
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 40;
            player.GetCritChance<MagicDamageClass>() += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Bone, 20).
                AddIngredient<FrigidShard>(5).
                AddIngredient(ItemID.FlinxFur, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}