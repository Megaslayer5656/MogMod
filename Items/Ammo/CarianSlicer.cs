using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class CarianSlicer : ModItem
    {
        public const int SwingTime = 24;
        public const float TrailOffsetCompletionRatio = 0.2f;
        public const float ExplosionExpandFactor = 1.013f;
        public static readonly Color SwordColor1 = new(50, 100, 255); // light blue
        public static readonly Color SwordColor2 = new(50, 50, 210); // dark blue
        public static readonly SoundStyle SwingSound = SoundID.Item15;
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<CarianSlicerProj>();
            Item.shootSpeed = 60f;
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
                AddIngredient(ItemID.Katana, 1).
                AddRecipeGroup("GoldBar", 18).
                AddIngredient(ItemID.FallenStar, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
