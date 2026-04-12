using Microsoft.Xna.Framework;
using MogMod.Items.Ammo;
using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class AstrologersStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Staves";
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 1;

            Item.damage = 15;
            Item.width = Item.height = 58;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // fires glintstone ammo types;
            Item.noMelee = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 2;

        // change stats depending on what spell was casted;
        // TODO: change stats automatically from the ammo so you can easily add any new sorceries without doing this slop
        public override bool CanUseItem(Player player)
        {
            Item ammoItem = player.ChooseAmmo(Item);

            if (ammoItem.type == ModContent.ItemType<GlintstonePebble>())
            {
                Item.noUseGraphic = false;
                Item.mana = GlintstonePebble.manaCost;
                Item.useTime = Item.useAnimation = GlintstonePebble.attackSpeed;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<RockSling>())
            {
                Item.noUseGraphic = false;
                Item.mana = RockSling.manaCost;
                Item.useTime = Item.useAnimation = RockSling.attackSpeed;
                Item.UseSound = SoundID.Item20;
            }
            else if (ammoItem.type == ModContent.ItemType<ShardSpiral>())
            {
                Item.noUseGraphic = false;
                Item.mana = ShardSpiral.manaCost;
                Item.useTime = Item.useAnimation = ShardSpiral.attackSpeed;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneStars>())
            {
                Item.noUseGraphic = false;
                Item.mana = GlintstoneStars.manaCost;
                Item.useTime = Item.useAnimation = GlintstoneStars.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarShower>())
            {
                Item.noUseGraphic = false;
                Item.mana = StarShower.manaCost;
                Item.useTime = Item.useAnimation = StarShower.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarsOfRuin>())
            {
                Item.noUseGraphic = false;
                Item.mana = StarsOfRuin.manaCost;
                Item.useTime = Item.useAnimation = StarsOfRuin.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneArc>())
            {
                Item.noUseGraphic = false;
                Item.mana = GlintstoneArc.manaCost;
                Item.useTime = Item.useAnimation = GlintstoneArc.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<CannonOfHaima>())
            {
                Item.noUseGraphic = false;
                Item.mana = CannonOfHaima.manaCost;
                Item.useTime = Item.useAnimation = CannonOfHaima.attackSpeed;
                Item.UseSound = SoundID.Item84;
            }
            else if (ammoItem.type == ModContent.ItemType<FoundingRainOfStars>())
            {
                Item.noUseGraphic = false;
                Item.mana = FoundingRainOfStars.manaCost;
                Item.useTime = Item.useAnimation = FoundingRainOfStars.attackSpeed;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<ExplosiveGhostflame>())
            {
                Item.noUseGraphic = false;
                Item.mana = ExplosiveGhostflame.manaCost;
                Item.useTime = Item.useAnimation = ExplosiveGhostflame.attackSpeed;
                Item.UseSound = SoundID.Item73;
            }
            else if (ammoItem.type == ModContent.ItemType<CarianSlicer>())
            {
                Item.noUseGraphic = true;
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
        public override bool MagicPrefix() => true;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string[] lLine = { "mana", "speed", "knockback" };
            tooltips.RemoveAll(line =>
                lLine.Any(word => line.Text.ToLower().Contains(word.ToLower()))
            );
        }
    }
}
