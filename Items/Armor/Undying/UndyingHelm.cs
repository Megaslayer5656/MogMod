using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Undying
{
    [AutoloadEquip(EquipType.Head)]
    public class UndyingHelm : ModItem, ILocalizedModType
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
            Item.width = Item.height = 24;
            Item.defense = 15;
            Item.rare = ItemRarityID.Lime;
        }

        //public override bool IsArmorSet(Item head, Item body, Item legs)
        //{
        //    return body.type == ModContent.ItemType<TiglaVest>() && legs.type == ModContent.ItemType<TiglaPants>();
        //}
        public override void UpdateArmorSet(Player player)
        {
            // set bonus description
            player.setBonus = SetBonusText.Value;

            // hunter and ammo potion effects

        }
        public override void UpdateEquip(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingUndyingArmor = true;

            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.GetCritChance<GenericDamageClass>() += 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteMask, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<UltimateOrb>(3).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}