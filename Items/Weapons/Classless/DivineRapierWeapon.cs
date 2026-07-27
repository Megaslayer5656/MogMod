using Microsoft.Xna.Framework;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Consumables;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Classless;
using MogMod.Rarities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Classless
{
    public class DivineRapierWeapon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Classless";
        public static bool MaxLife = false;
        public override void SetStaticDefaults() => ItemID.Sets.Spears[Item.type] = true;
        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Generic;
            Item.width = Item.height = 112;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 9.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.expert = true;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
            Item.shoot = ModContent.ProjectileType<DivineRapierProj>();
            Item.shootSpeed = 12f;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 96;
        public override bool MeleePrefix() => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.zenithWorld)
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ProjectileID.ConfettiMelee, 1, 0f, player.whoAmI, 0, 0);
                return false;
            }
            // only fire beams at max health
            if (player.statLife >= (player.statLifeMax2 * 1f))
            {
                MaxLife = true;
                int rapierBeam = ModContent.ProjectileType<DivineRapierBeam>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, rapierBeam, damage * 2, knockback, player.whoAmI, 0, 0);
            }
            else
            {
                MaxLife = false;
            }
            return true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<Color> colorList = new List<Color>()
            {
                new Color(245, 209, 169),
                new Color(198, 245, 169),
                new Color(245, 169, 240),
            };

            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip3");
            if (line != null)
                line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Trident>().
                AddIngredient<DivineRapier>().
                AddIngredient<VoniumBar>(5).
                AddIngredient<SoulOfMogMod>(3).
                AddIngredient<AghanimShard>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}