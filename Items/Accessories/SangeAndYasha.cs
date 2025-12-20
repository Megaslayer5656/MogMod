using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SangeAndYasha : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Melee) += .24f;
            player.statLifeMax2 += 50;
            player.lifeRegen += 25;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.accRunSpeed += player.accRunSpeed * .24f;
            player.lifeSteal += .25f;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>(1).
                AddIngredient<Yasha>(1).
                AddIngredient<CraftingRecipe>(1).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddTile(ItemID.MythrilAnvil).
                Register();
        }
    }
}
