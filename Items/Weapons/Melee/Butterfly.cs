using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.PotionBuffs;
using MogMod.Items.Global;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Butterfly : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random rand = new Random();
        public override void SetDefaults()
        {
            Item.width = Item.height = 96;
            Item.damage = 95;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useTurn = false;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
            Item.shoot = ProjectileID.PurificationPowder; //This and the shoot method are to allow the weapon to swing in the direction of your cursor
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<ButterflyCooldown>())
            {
                player.AddBuff(ModContent.BuffType<ButterflyBuff>(), 60);
                player.AddBuff(ModContent.BuffType<ButterflyCooldown>(), 900);
                return true;
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = target.GetSource_FromAI();
            if (Main.rand.NextBool(4))
            {
                for (int i = 0; i <= 3; i++)
                {
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, Main.rand.NextBool(2), -400f, 400f, -300f, 300f, 5.75f, ModContent.ProjectileType<ButterflyProjectile>(), Convert.ToInt32(Item.damage / 4f), 0f, player.whoAmI, false, 0f);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophyteBar, 15).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Butterfly"}", 5).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}