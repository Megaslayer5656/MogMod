using MogMod.Items.Other;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Radiant
{
    [AutoloadEquip(EquipType.Body)]
    public class RadiantTop : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.defense = 17;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.09f;
            player.GetCritChance<MagicDamageClass>() += 9;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreRobe, 1).
                AddIngredient<FaeBar>(15).
                AddIngredient<ManaCore>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}