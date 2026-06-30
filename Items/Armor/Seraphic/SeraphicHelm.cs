using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Seraphic
{
    [AutoloadEquip(EquipType.Head)]
    public class SeraphicHelm : ModItem, ILocalizedModType
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
            Item.width = Item.height = 26;

            Item.defense = 44; // idk yet, gonna make it post ML though

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateVanitySet(Player player)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 156 : DustID.GoldCoin, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.65f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.3f;
                }
            }
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SeraphicBreastplate>() && legs.type == ModContent.ItemType<SeraphicGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSeraphic = true;
            player.aggro += 1700;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.3f;
            player.lifeRegen += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedMask).
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.AncientHallowedMask).
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}