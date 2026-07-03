using Microsoft.Xna.Framework;
using MogMod.Items.Ammo.SorcerySpells;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class LusatsGlintstoneStaff : SorceryStaff
    {
        public override float ManaCostMult => 1.5f;
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 66;
            Item.width = Item.height = 74;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 31;
        public override bool MagicPrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentNebula, 8).
                AddIngredient<ManaCore>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}