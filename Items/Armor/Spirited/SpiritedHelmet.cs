using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Spirited
{
    [AutoloadEquip(EquipType.Head)]
    public class SpiritedHelmet : ModItem, ILocalizedModType
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
            Item.height = 26;
            Item.defense = 6;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SpiritedBreastplate>() && legs.type == ModContent.ItemType<SpiritedLeggings>();
        }
        public override void UpdateArmorSet(Player player)
        {
            // set bonus will be a custom double jump + slowfall
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSpiritArmor = true;
            player.GetJumpState<SpiritJump>().Enable();
            player.setBonus = SetBonusText.Value;
            player.statManaMax2 += 80;
            player.GetDamage<MeleeDamageClass>() += 0.08f;
            player.GetDamage<MagicDamageClass>() += 0.08f;
        }
        public override void UpdateVanitySet(Player player)
        {
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? DustID.PlatinumCoin : DustID.SilverCoin, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
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
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<MeleeDamageClass>() += 6;
            player.GetCritChance<MagicDamageClass>() += 6;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritShard>(6).
                AddIngredient<ManaEssence>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}