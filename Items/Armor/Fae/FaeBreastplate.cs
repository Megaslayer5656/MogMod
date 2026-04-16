using MogMod.Items.Global;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Fae
{
    [AutoloadEquip(EquipType.Body)]
    public class FaeBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = false;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;

            Item.defense = 20;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.ammoCost80 = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(15).
                AddIngredient(ItemID.CrystalNinjaChestplate, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}