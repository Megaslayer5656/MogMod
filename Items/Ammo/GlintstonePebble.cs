using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class GlintstonePebble : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public const int manaCost = 5;
        public const int attackSpeed = 36;
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = manaCost;

            Item.damage = 22;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.shoot = ModContent.ProjectileType<GlintstonePebbleProj>();
            Item.shootSpeed = 6f;
            Item.ammo = ModContent.ItemType<GlintstonePebble>(); // use this for all sorceries so it can be used by staffs;
        }

        // replaces the "Ammo" description with "Sorcery" since i dont think you can do it in localization;
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
                .AddIngredient<Scroll>(1)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}
