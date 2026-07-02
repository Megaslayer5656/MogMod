using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class ElysianSeraph : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 40;

            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<ElysianSeraphProj>();
            Item.shootSpeed = 3f;
            Item.knockBack = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 31;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.useTime = Item.useAnimation = 60;
            else
                Item.useTime = Item.useAnimation = 20;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.4f }, player.Center);
                Projectile.NewProjectile(source, position, velocity * 3, ModContent.ProjectileType<ElysianSeraphThrownProj>(), damage, knockback, player.whoAmI);
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.4f }, player.Center);
                Projectile.NewProjectile(source, position, velocity * 0.75f, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override bool MeleePrefix() => true;
        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ElysianSeraphThrownProj>()] == 0;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Gungnir).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}