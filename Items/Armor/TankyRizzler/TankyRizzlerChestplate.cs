using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.TankyRizzler
{
    [AutoloadEquip(EquipType.Body)]
    public class TankyRizzlerChestplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.defense = 38;
            Item.rare = ItemRarityID.Cyan;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.12f;
            player.GetCritChance<MeleeDamageClass>() += 8;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeetleShell, 1).
                AddIngredient(ItemID.MartianConduitPlating, 125).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}