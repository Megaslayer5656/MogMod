using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class GlintstoneStars : ModItem
    {
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = 12;

            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(0, 12, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<GlintstoneStarsProj>();
            Item.shootSpeed = 6f;
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
            CreateRecipe().
                AddIngredient(ItemID.MeteoriteBar, 16).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Flesh"}", 9).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
