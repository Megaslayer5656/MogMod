using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class CarianSlicer : ModItem
    {
        public override void SetDefaults()
        {
            // AS OF NOW IT DOES NOTHING (interesting);
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<CarianSlicerProj>();
            Item.shootSpeed = 8f;
            Item.ammo = ModContent.ItemType<GlintstonePebble>(); // so it can be used by the glintstone staff;
        }

        // replaces the "Ammo" description with "Sorcery";
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var changedLine = tooltips.FirstOrDefault(x => x.Name == "Ammo" && x.Mod == "Terraria");
            if (changedLine != null)
            {
                changedLine.Text = "Sorcery";
            }
        }
        public override void AddRecipes()
        {
            //CreateRecipe().
            //    AddIngredient(ItemID.Katana, 1).
            //    AddRecipeGroup("GoldBar", 18).
            //    AddIngredient(ItemID.FallenStar, 7).
            //    AddIngredient<CraftingRecipe>(1).
            //    AddTile(TileID.Bookcases).
            //    Register();
        }
    }
}
