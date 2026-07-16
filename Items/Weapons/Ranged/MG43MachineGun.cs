using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    // LDR MACHNINE GUN TYPE WEAPON WITH 3 DIFFERENT TYPES OF RPM
    // HIGHER RPM == HIGHER SPREAD
    // TODO:
    // 175 ROUNDS UNTIL 3 SECOND RELOAD
    // CUSTOM UI USE EXAMPLE MOD CUSTOM RESOURCE WEAPON
    public class MG43MachineGun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static int rpm = 3;
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 22;

            Item.damage = 40;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item40;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;

            Item.noMelee = true;
            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                // determine rpm
                SoundEngine.PlaySound(SoundID.Item149, player.Center);
                if (rpm > 1)
                    rpm--;
                else
                    rpm = 3;
                return false;
            }
            else
                return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // different spread for each rpm
            int fireRate = rpm;
            if (rpm == 3)
                fireRate = Main.zenithWorld ? 0: 0;
            else if (rpm == 2)
                fireRate = Main.zenithWorld ? 0 : 25;
            else
                fireRate = Main.zenithWorld ? 500 : 50;

            // spread
            float SpeedX = velocity.X + Main.rand.Next(-fireRate, fireRate + 1) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-fireRate, fireRate + 1) * 0.05f;
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
            return false;
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
               AddRecipeGroup("AdamantiteBar", 16).
               AddIngredient<FuciumBar>(10).
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}