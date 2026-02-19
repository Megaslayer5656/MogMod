using System.Linq;
using MogMod.Items.Ammo;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic //So yeah I'm gonna make a glintstone sorcery inspired magic weapon line, with shard spiral, glintstone stars, and star shower from elden ring. Will have cool vfx. Placeholder file for now until I get home and work on it.
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
            Item.mana = 0;

            Item.damage = 10;
            Item.width = Item.height = 32;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // fires glintstone ammo types;
            Item.noMelee = true;
        }

        // change stats depending on what spell was casted;
        public override bool CanUseItem(Player player)
        {
            Item ammoItem = player.ChooseAmmo(Item);

            if (ammoItem.type == ModContent.ItemType<GlintstonePebble>())
            {
                Item.mana = 5;
                Item.useTime = Item.useAnimation = 36;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<RockSling>())
            {
                Item.mana = 20;
                Item.useTime = Item.useAnimation = 60;
                Item.UseSound = SoundID.Item20;
            }
            else if (ammoItem.type == ModContent.ItemType<ShardSpiral>())
            {
                Item.mana = 16;
                Item.useTime = Item.useAnimation = 46;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneStars>())
            {
                Item.mana = 12;
                Item.useTime = Item.useAnimation = 40;
                Item.UseSound = SoundID.Item8;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DiamondStaff, 1)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(ItemID.ManaCrystal, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
