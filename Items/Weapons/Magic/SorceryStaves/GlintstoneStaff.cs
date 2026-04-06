using Microsoft.Xna.Framework;
using MogMod.Items.Ammo;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves //So yeah I'm gonna make a glintstone sorcery inspired magic weapon line, with shard spiral, glintstone stars, and star shower from elden ring. Will have cool vfx. Placeholder file for now until I get home and work on it.
    // making glintstone pebble -> glintstone staff that fires ammo types and gonna make glintstone pebble a non-consumable ammo type that fires glintstone pebble proj;

{
    public class GlintstoneStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            // TODO: make it so that certain sorceries play different sounds, change item usetime and mana cost for each sorcery;
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 1; // set to 1 so it can recieve all magic reforges (doesnt work)

            Item.damage = 7;
            Item.width = Item.height = 32;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f; // set to 1 so it can recieve all magic reforges
            Item.value = Item.buyPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            //Item.channel = true;

            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // fires glintstone ammo types;
            Item.noMelee = true;
        }

        // change stats depending on what spell was casted;
        // TODO: change stats automatically from the ammo so you can easily add any new sorceries without doing this slop
        public override bool CanUseItem(Player player)
        {
            Item ammoItem = player.ChooseAmmo(Item);

            if (ammoItem.type == ModContent.ItemType<GlintstonePebble>())
            {
                Item.mana = GlintstonePebble.manaCost;
                Item.useTime = Item.useAnimation = GlintstonePebble.attackSpeed;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<RockSling>())
            {
                Item.mana = RockSling.manaCost;
                Item.useTime = Item.useAnimation = RockSling.attackSpeed;
                Item.UseSound = SoundID.Item20;
            }
            else if (ammoItem.type == ModContent.ItemType<ShardSpiral>())
            {
                Item.mana = ShardSpiral.manaCost;
                Item.useTime = Item.useAnimation = ShardSpiral.attackSpeed;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneStars>())
            {
                Item.mana = GlintstoneStars.manaCost;
                Item.useTime = Item.useAnimation = GlintstoneStars.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarShower>())
            {
                Item.mana = StarShower.manaCost;
                Item.useTime = Item.useAnimation = StarShower.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarsOfRuin>())
            {
                Item.mana = StarsOfRuin.manaCost;
                Item.useTime = Item.useAnimation = StarsOfRuin.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneArc>())
            {
                Item.mana = GlintstoneArc.manaCost;
                Item.useTime = Item.useAnimation = GlintstoneArc.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<CannonOfHaima>())
            {
                Item.mana = CannonOfHaima.manaCost;
                Item.useTime = Item.useAnimation = CannonOfHaima.attackSpeed;
                Item.UseSound = SoundID.Item84;
            }
            else if (ammoItem.type == ModContent.ItemType<FoundingRainOfStars>())
            {
                Item.mana = FoundingRainOfStars.manaCost;
                Item.useTime = Item.useAnimation = FoundingRainOfStars.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<ExplosiveGhostflame>())
            {
                Item.mana = ExplosiveGhostflame.manaCost;
                Item.useTime = Item.useAnimation = ExplosiveGhostflame.attackSpeed;
                Item.UseSound = SoundID.Item73;
            }
            else if (ammoItem.type == ModContent.ItemType<CarianSlicer>())
            {
                Item.mana = CarianSlicer.manaCost;
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
            string[] lLine = { "mana", "speed", "knockback" };
            tooltips.RemoveAll(line =>
                lLine.Any(word => line.Text.ToLower().Contains(word.ToLower()))
            );
        }
        public override bool MagicPrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DiamondStaff, 1).
                AddIngredient<ManaEssence>(1).
                AddIngredient<FrigidShard>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
