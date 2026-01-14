using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace MogMod.Items.Weapons.Ranged
{
    public class Megaslark : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 36;
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.75f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 31;
        public override Vector2? HoldoutOffset() => new Vector2(-15, -2);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + Main.rand.Next(-5, 6) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-5, 6) * 0.05f;
            if (Main.rand.NextBool(5))
            {
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<MegaslarkProj>(), damage, knockback, player.whoAmI);
            }
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 80)
                return false;
            return true;
        }
        // gets rid of "Expert" tag at the bottom of the item desc
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var changedLine = tooltips.FirstOrDefault(x => x.Name == "Expert" && x.Mod == "Terraria");
            if (changedLine != null)
            {
                changedLine.Text = "";
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SDMG, 1).
                AddIngredient(ItemID.VortexBeater, 1).
                AddIngredient(ItemID.Megashark, 1).
                AddIngredient(ItemID.LunarBar, 20).
                AddIngredient<UltimateOrb>(3).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SDMG, 1).
                AddIngredient(ItemID.VortexBeater, 1).
                AddIngredient<Minislark>(1).
                AddIngredient(ItemID.LunarBar, 20).
                AddIngredient<UltimateOrb>(3).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}