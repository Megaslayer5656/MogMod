using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Damascus
{

    [AutoloadEquip(EquipType.Body)]
    public class DamascusMail : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void Load()
        {
            // cape doesnt work for some unknown reason
            if (Main.netMode != NetmodeID.Server)
            {
                // Add equip textures
                EquipLoader.AddEquipTexture(Mod, "MogMod/Items/Armor/Damascus/DamascusMail_Body", EquipType.Back, this);
            }
        }
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
            Item.width = 34;
            Item.height = 24;
            Item.defense = 7;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 6;
            player.GetAttackSpeed<MeleeDamageClass>() += .06f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}