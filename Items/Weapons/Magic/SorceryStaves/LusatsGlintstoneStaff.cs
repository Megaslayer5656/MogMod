using Microsoft.Xna.Framework;
using MogMod.Items.Ammo;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class LusatsGlintstoneStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Staves";
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 1;

            Item.damage = 66;
            Item.width = Item.height = 74;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // fires glintstone ammo types;
            Item.noMelee = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 31;

        // change stats depending on what spell was casted;
        // TODO: change stats automatically from the ammo so you can easily add any new sorceries without doing this slop
        public override bool CanUseItem(Player player)
        {
            Item ammoItem = player.ChooseAmmo(Item);

            if (ammoItem.type == ModContent.ItemType<GlintstonePebble>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(GlintstonePebble.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = GlintstonePebble.attackSpeed;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<RockSling>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(RockSling.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = RockSling.attackSpeed;
                Item.UseSound = SoundID.Item20;
            }
            else if (ammoItem.type == ModContent.ItemType<ShardSpiral>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(ShardSpiral.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = ShardSpiral.attackSpeed;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneStars>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(GlintstoneStars.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = GlintstoneStars.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarShower>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(StarShower.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = StarShower.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarsOfRuin>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(StarsOfRuin.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = StarsOfRuin.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneArc>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(GlintstoneArc.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = GlintstoneArc.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<CannonOfHaima>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(CannonOfHaima.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = CannonOfHaima.attackSpeed;
                Item.UseSound = SoundID.Item84;
            }
            else if (ammoItem.type == ModContent.ItemType<FoundingRainOfStars>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(FoundingRainOfStars.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = FoundingRainOfStars.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<ExplosiveGhostflame>())
            {
                Item.noUseGraphic = false;
                Item.mana = Convert.ToInt32(ExplosiveGhostflame.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = ExplosiveGhostflame.attackSpeed;
                Item.UseSound = SoundID.Item73;
            }
            else if (ammoItem.type == ModContent.ItemType<CarianSlicer>())
            {
                Item.noUseGraphic = true;
                Item.mana = Convert.ToInt32(CarianSlicer.manaCost * 1.5f);
                Item.useTime = Item.useAnimation = CarianSlicer.attackSpeed;
                Item.UseSound = null;
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item ammoItem = player.ChooseAmmo(Item);
            if (ammoItem.type == ModContent.ItemType<CarianSlicer>())
            {
                // do this so different rockets don't mess with the projectile spawned
                int slicer = ModContent.ProjectileType<CarianSlicerProj>();
                // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
                Projectile.NewProjectile(source, position, velocity, slicer, damage, knockback, Main.myPlayer);
                return false; // return false to prevent original projectile from being shot
            }
            else
                return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string[] lLine = { "uses", "speed", "knockback" };
            tooltips.RemoveAll(line =>
                lLine.Any(word => line.Text.ToLower().Contains(word.ToLower()))
            );
        }
        public override bool MagicPrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentNebula, 8).
                AddIngredient<ManaCore>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}