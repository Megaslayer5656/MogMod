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
    public class StarShower : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public const int manaCost = 20;
        public const int attackSpeed = 46;
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = manaCost;

            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
            Item.shoot = ModContent.ProjectileType<StarShowerProj>();
            Item.shootSpeed = 6f;
            Item.ammo = ModContent.ItemType<GlintstonePebble>(); // so it can be used by the glintstone staff;
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
            CreateRecipe().
                AddIngredient<GlintstoneStars>(1).
                AddRecipeGroup("CobaltBar", 14).
                AddIngredient(ItemID.SoulofSight, 7).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
