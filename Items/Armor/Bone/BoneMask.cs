using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Bone
{
    [AutoloadEquip(EquipType.Head)]
    public class BoneMask : ModItem, ILocalizedModType
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
            Item.width = 22;
            Item.height = 30;
            Item.defense = 2;
            Item.rare = ItemRarityID.Green;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BoneMail>() && legs.type == ModContent.ItemType<BoneGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            player.setBonus = SetBonusText.Value;
            mogPlayer.wearingBoneArmor = true;
            player.findTreasure = true;
            player.blockRange += 3;
        }
        public override void UpdateEquip(Player player)
        {
            player.pickSpeed -= .15f;
            player.tileSpeed += 0.4f;
            player.wallSpeed += 0.4f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 10).
                AddIngredient(ItemID.FossilOre, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}