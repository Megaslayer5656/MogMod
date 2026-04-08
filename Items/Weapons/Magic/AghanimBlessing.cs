using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Rarities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace MogMod.Items.Weapons.Magic
{
    public class AghanimBlessing : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.useTime = 3;
            Item.useAnimation = 15;
            Item.reuseDelay = Item.useAnimation + 2;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AghanimBlessingProj>();
            Item.shootSpeed = 26f;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 56;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo projSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float offsetAngle = MathHelper.TwoPi * player.itemAnimation / player.itemAnimationMax;
            offsetAngle += MathHelper.PiOver4 + Main.rand.NextFloat(0f, 1.3f);
            offsetAngle -= AghanimLaser.UniversalAngularSpeed * 0.5f;
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = Item.shootSpeed;
            float xPos = (float)Main.mouseX + Main.screenPosition.X - playerPos.X;
            float yPos = (float)Main.mouseY + Main.screenPosition.Y - playerPos.Y;
            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
            float sourceVariationLow = 20f;
            float sourceVariationHigh = 60f;
            Vector2 source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(sourceVariationLow, sourceVariationHigh, Main.rand.NextFloat());
            for (int i = 0; i < 50; i++)
            {
                source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(sourceVariationLow, sourceVariationHigh, Main.rand.NextFloat());
                if (Collision.CanHit(playerPos, 0, 0, source + (source - playerPos).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            Vector2 velocityReal = Main.MouseWorld - source;
            Vector2 velocityVariation = new Vector2(xPos, yPos).SafeNormalize(Vector2.UnitY) * speed;
            velocityReal = velocityReal.SafeNormalize(velocityVariation) * speed;
            velocityReal = Vector2.Lerp(velocityReal, velocityVariation, 0.25f);
            Projectile.NewProjectile(projSource, source, velocityReal, type, damage, knockback, player.whoAmI, 0f, Main.rand.Next(3));
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            List<Color> colorList = new List<Color>()
            {
                new Color(71, 35, 161),
                new Color(125, 35, 161),
                new Color(161, 35, 113),
            };
            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);

            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip4");
            if (line != null)
                line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AghanimScepter>(1).
                AddIngredient<DagonFive>(1).
                AddIngredient(ItemID.MoonlordTurretStaff, 1).
                AddIngredient<VoniumBar>(5).
                AddIngredient<ManaCore>(3).
                AddIngredient(ItemID.CelestialSigil, 1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}