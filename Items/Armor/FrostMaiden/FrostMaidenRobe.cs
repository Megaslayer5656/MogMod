using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.FrostMaiden
{

    [AutoloadEquip(EquipType.Body)]
    public class FrostMaidenRobe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            //ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            //ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 20;
            Item.defense = 4;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 80;
            player.manaCost *= 0.84f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe, 1).
                AddIngredient<FrigidShard>(5).
                AddIngredient<ManaEssence>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}