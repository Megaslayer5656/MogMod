using MogMod.Items.Consumables;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class FoundingRainOfStars : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public const int manaCost = 45;
        public const int attackSpeed = 64;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StarsOfRuin>();
        }
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = manaCost;

            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<FoundingRainOfStarsProj>();
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
                AddIngredient<StarShower>(1).
                AddIngredient<FaeBar>(5).
                AddIngredient<ManaCore>(1).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                DisableDecraft().
                Register();
        }
    }
}
