using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Blademail
{
    [AutoloadEquip(EquipType.Head)]
    public class BlademailMask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.BladeMailKeybind);
        ModKeybind keybindActive = null;
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
            Item.width = 28;
            Item.height = 24;
            Item.defense = 9;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BlademailBreastplate>() && legs.type == ModContent.ItemType<BlademailLeggings>();
        }
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingBladeMail = true;
            player.setBonus = SetBonusText.Value;
            player.thorns += 1f;
            player.aggro += 550;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.07f;
            player.GetCritChance<MeleeDamageClass>() += 7;
            player.thorns += .4f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FalconBlade, 1).
                AddIngredient(ItemID.Spike, 25).
                AddIngredient(ItemID.Bone, 30).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}