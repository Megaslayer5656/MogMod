using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles;
using Terraria.ID;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class Sange : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 59;
            Item.damage = 150;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 7, 30, 50);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 76;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteSword, 1).
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(ItemID.MythrilAnvil).
                Register();
        }
    }
}
