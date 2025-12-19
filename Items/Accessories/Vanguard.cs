using MogMod.Items.Other;
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
    public class Vanguard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
            player.lifeRegen += 4;
            player.statDefense += 12;
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VitalityBooster>(1).
                AddIngredient(ItemID.CobaltShield, 1).
                AddIngredient(ItemID.BandofRegeneration, 1).
                AddIngredient(ItemID.Ruby, 2).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
