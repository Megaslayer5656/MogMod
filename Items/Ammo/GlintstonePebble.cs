using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace MogMod.Items.Ammo
{
    public class GlintstonePebble : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.height = 32;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<GlintstonePebbleProj>();
            Item.shootSpeed = 2f;
            Item.ammo = Item.type;
        }

        // replaces the "Ammo" description with "Sorcery" since i dont you can do it in localization;
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
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 40)
                .AddIngredient(ItemID.LargeSapphire, 1)
                .AddIngredient<CraftingRecipe>(1)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}
