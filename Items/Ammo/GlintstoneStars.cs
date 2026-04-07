using MogMod.Items.Global;
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
    public class GlintstoneStars : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public const int manaCost = 12;
        public const int attackSpeed = 40;
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = manaCost;

            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
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
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
