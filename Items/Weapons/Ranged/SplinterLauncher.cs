using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class SplinterLauncher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 26;

            Item.damage = 14;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item108; //Nail gun sound

            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.NailFriendly;

            Item.noMelee = true;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projNum = Main.zenithWorld ? 50 : 2;
            for (int i = 0; i < projNum; i++)
            {
                Vector2 splinterVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3);
                Projectile.NewProjectile(source, position, splinterVelocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));
        }
        public override float UseSpeedMultiplier(Player player) => Main.zenithWorld ? 0.2f : 1f;
        public override Vector2? HoldoutOffset() => new Vector2(-4.8f, 1.75f);
        // change the tooltip when in get fixed boi worlds
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = tooltips.FirstOrDefault(x => x.Text.Contains("[GFB]") && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = Lang.SupportGlyphs(this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
                if (Main.zenithWorld)
                    line.OverrideColor = new Color(Main.DiscoR, Main.DiscoR * 2, (int)(Main.DiscoR * 0.5f));
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient(ItemID.FlintlockPistol).
            AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Material"}", 8).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}