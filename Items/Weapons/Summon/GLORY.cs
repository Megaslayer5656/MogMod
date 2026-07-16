using Microsoft.Xna.Framework;
using MogMod.Buffs.Summons;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Summon;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Summon
{
    public class GLORY : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;

            Item.mana = 10;
            Item.damage = 24;
            Item.DamageType = DamageClass.Summon;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 6f;

            Item.UseSound = SoundID.Item44;
            //Item.buffType = ModContent.BuffType<DivinitasSummonBuff>();
            //Item.shoot = ModContent.ProjectileType<DivinitasSummon>();
            Item.buffType = BuffID.PirateMinion;
            Item.shoot = Main.rand.Next(ProjectileID.OneEyedPirate, ProjectileID.PirateCaptain + 1);

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Vector2 mouse = player.ClampedMouseWorld();
            Point mouseTileCoords = mouse.ToTileCoordinates();
            if (!MiscUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).IsTileSolidGround())
            {
                var minion = Projectile.NewProjectileDirect(source, mouse, Vector2.UnitY * 4f, type, damage, knockback, player.whoAmI);
                minion.originalDamage = Item.damage;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(12).
                AddIngredient<VitalityBooster>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}