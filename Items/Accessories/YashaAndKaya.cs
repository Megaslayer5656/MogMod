using MogMod.Items.Other;
using MogMod.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class YashaAndKaya : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .24f;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.GetDamage(DamageClass.Magic) += .12f;
            player.moveSpeed += .24f;
            player.accRunSpeed += player.accRunSpeed * .24f;
            player.manaRegen += (int)Math.Round(player.manaRegen * .5f);
            player.statManaMax2 += 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kaya>(1).
                AddIngredient<Yasha>(1).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(ItemID.MythrilAnvil).
                Register();
        }
    }
}