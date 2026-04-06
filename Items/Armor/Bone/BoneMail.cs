using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Bone
{
    [AutoloadEquip(EquipType.Body)]
    public class BoneMail : ModItem, ILocalizedModType
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
            Item.width = 30;
            Item.height = 22;
            Item.defense = 6;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 4;
            player.GetDamage<GenericDamageClass>() += .05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 20).
                AddIngredient(ItemID.FossilOre, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}