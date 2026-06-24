using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Other
{
    [AutoloadEquip(EquipType.Head)]
    public class PleaseStopMe : ModItem, ILocalizedModType
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
            //SetBonusText = this.GetLocalization("SetBonus");

            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.defense = 6;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        //public override bool IsArmorSet(Item head, Item body, Item legs)
        //{
        //    return body.type == ModContent.ItemType<WhiteCloak>();
        //}
        //public override void UpdateArmorSet(Player player)
        //{
        //    player.setBonus = SetBonusText.Value;
        //    MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
        //    mogPlayer.wearingWhiteArmor = true;
        //}
        public override void UpdateEquip(Player player)
        {
            player.noKnockback = true;
            player.moveSpeed += .16f;
        }
    }
}