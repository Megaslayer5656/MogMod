using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Bone
{
    [AutoloadEquip(EquipType.Head)]
    public class BoneHelm : ModItem, ILocalizedModType
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
            Item.defense = 4;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
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
            player.detectCreature = true;
            player.GetAttackSpeed<GenericDamageClass>() += .08f;
        }
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 10).
                AddIngredient(ItemID.FossilOre, 10).
                AddTile(TileID.Loom).
                Register();
        }
    }
}