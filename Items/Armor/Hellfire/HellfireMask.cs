using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Hellfire
{
    [AutoloadEquip(EquipType.Head)]
    public class HellfireMask : ModItem, ILocalizedModType
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
            Item.height = 24;

            Item.defense = 13; // 45

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlinesForbidden = true;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HellfireBreastplate>() && legs.type == ModContent.ItemType<HellfireGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingHellfireArmor = true;
            player.lavaImmune = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.08f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenHelmet).
                AddIngredient<GriefBar>(10).
                AddIngredient<ScorchedCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}