using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Radiant
{
    [AutoloadEquip(EquipType.Head)]
    public class RadiantFlower : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            // so the players hair can be seen with the armor equipped
            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;

            // set bonus text
            SetBonusText = this.GetLocalization("SetBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.defense = 18;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }

        // what armor is needed for a set bonus
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<RadiantTop>() && legs.type == ModContent.ItemType<RadiantBottom>();
        }

        // visual effects
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        // set bonus
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingRadiantArmor = true;
            player.setBonus = SetBonusText.Value;
            player.manaRegenBonus += 8;
        }

        // armor stat buffs
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 100;
            player.manaCost *= 0.83f;
            player.GetDamage<MagicDamageClass>() += 0.13f;
            player.GetCritChance<MagicDamageClass>() += 13;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreHood, 1).
                AddIngredient(ItemID.ObsidianRose, 1).
                AddIngredient<FaeBar>(9).
                AddIngredient<ManaCore>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SpectreMask, 1).
                AddIngredient(ItemID.ObsidianRose, 1).
                AddIngredient<FaeBar>(9).
                AddIngredient<ManaCore>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}