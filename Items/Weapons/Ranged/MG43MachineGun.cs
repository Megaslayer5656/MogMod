using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class MG43MachineGun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static int rpm = 3;
        public const int maxShots = 175;
        public const int reloadTime = 180;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots, reloadTime.FramesToSeconds());
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 22;

            Item.damage = 40;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<MG43Holdout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                // determine rpm
                SoundEngine.PlaySound(SoundID.Item149, player.Center);
                if (rpm > 1) rpm--;
                else rpm = 3;
                return false;
            }
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
        public override bool RangedPrefix() => true;
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            holdout.velocity = (player.MogMod().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // display the rpm in the tooltip
            var effectDescTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[RPM]") && x.Mod == "Terraria");
            int fireRate = (rpm * 2) + 10;

            if (effectDescTooltip != null)
                effectDescTooltip.Text = Main.zenithWorld ? effectDescTooltip.Text = effectDescTooltip.Text.Replace("[RPM]", this.GetLocalizedValue("GFBRPM")) : effectDescTooltip.Text = effectDescTooltip.Text.Replace("[RPM]", $"{60 / fireRate * 175}");
        }
        public override void AddRecipes() // adamantite tier, pre-mech
        {
            CreateRecipe().
               AddIngredient<R8Revolver>().
               AddRecipeGroup("AnyAdamantiteBar", 16).
               AddIngredient<FuciumBar>(10).
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}