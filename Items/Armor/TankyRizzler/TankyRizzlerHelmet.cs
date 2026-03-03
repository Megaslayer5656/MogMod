using MogMod.Items.Consumables;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.TankyRizzler
{
    [AutoloadEquip(EquipType.Head)]
    public class TankyRizzlerHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.defense = 26;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TankyRizzlerChestplate>() && legs.type == ModContent.ItemType<TankyRizzlerLeggings>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.statLifeMax2 += 70;
            player.endurance *= .10f;
            player.noKnockback = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.08f;
            player.statLifeMax2 += 80;
            player.lifeRegen += 10;
            player.aggro += 700;
            player.moveSpeed -= .1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeetleHelmet, 1).
                AddIngredient(ItemID.MartianConduitPlating, 150).
                AddIngredient<UltimateOrb>(3).
                AddIngredient<BlockOfCheese>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}