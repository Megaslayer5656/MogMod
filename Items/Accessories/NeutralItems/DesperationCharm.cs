using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class DesperationCharm : NeutralItem
    {
        public int AdditiveDamageBonus = 100;
        public int AttackSpeedBonus = 25;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 30;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < player.statLifeMax2 * 0.25f)
            {
                player.GetDamage(DamageClass.Generic) += AdditiveDamageBonus / 100f;
                player.GetAttackSpeed(DamageClass.Generic) += AttackSpeedBonus / 100f;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CharmofMyths, 1).
                AddIngredient<DabDadBar>(10).
                AddIngredient<SoulOfMogMod>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}