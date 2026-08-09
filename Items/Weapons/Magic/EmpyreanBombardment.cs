using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using System.Linq;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class EmpyreanBombardment : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static int MaxStars = 10;
        //public override void SetStaticDefaults() => Item.staff[Item.type] = true; // not required since its a holdout
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 50;

            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 3;
            Item.useTime = Item.useAnimation = 3;
            Item.knockBack = 4f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<EmpyreanHoldout>();
            Item.shootSpeed = 2f;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.ArmorPenetration = 40;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            holdout.velocity = (player.MogMod().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            List<Color> colorList = new List<Color>()
            {
                new(255, 249, 59),
                new(247, 119, 224),
                new(40, 105, 240),
            };
            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);

            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
            if (line != null) line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient(ItemID.FallenStar, 12).
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient(ItemID.FragmentStardust, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}