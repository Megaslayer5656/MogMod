using MogMod.Items.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.WhiteMask
{
    [AutoloadEquip(EquipType.Body)]
    public class WhiteCloak : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            EquipLoader.AddEquipTexture(Mod, "MogMod/Items/Armor/WhiteMask/WhiteCloak_Legs", EquipType.Legs, this);
        }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = false;
        }
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.defense = 14;
            Item.rare = ItemRarityID.Lime;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .08f; // i think it would be cooler if it gave attack speed instead of damage because of bleed buildup
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 14).
                AddIngredient(ItemID.SoulofFright, 8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
