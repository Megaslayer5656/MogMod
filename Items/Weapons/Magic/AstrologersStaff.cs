using MogMod.Items.Ammo;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class AstrologersStaff : ModItem, ILocalizedModType
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

            Item.damage = 15;
            Item.width = Item.height = 58;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            //Item.channel = true;

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
                Item.mana = GlintstonePebble.manaCost;
                Item.useTime = Item.useAnimation = 36;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<RockSling>())
            {
                Item.mana = RockSling.manaCost;
                Item.useTime = Item.useAnimation = 60;
                Item.UseSound = SoundID.Item20;
            }
            else if (ammoItem.type == ModContent.ItemType<ShardSpiral>())
            {
                Item.mana = ShardSpiral.manaCost;
                Item.useTime = Item.useAnimation = 46;
                Item.UseSound = SoundID.Item9;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneStars>())
            {
                Item.mana = GlintstoneStars.manaCost;
                Item.useTime = Item.useAnimation = 40;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarShower>())
            {
                Item.mana = StarShower.manaCost;
                Item.useTime = Item.useAnimation = 44;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<StarsOfRuin>())
            {
                Item.mana = StarsOfRuin.manaCost;
                Item.useTime = Item.useAnimation = 48;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<GlintstoneArc>())
            {
                Item.mana = GlintstoneArc.manaCost;
                Item.useTime = Item.useAnimation = 36;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<CannonOfHaima>())
            {
                Item.mana = CannonOfHaima.manaCost;
                Item.useTime = Item.useAnimation = 60;
                Item.UseSound = SoundID.Item84;
            }
            else if (ammoItem.type == ModContent.ItemType<FoundingRainOfStars>())
            {
                Item.mana = FoundingRainOfStars.manaCost;
                Item.useTime = Item.useAnimation = 64;
                Item.UseSound = SoundID.Item8;
            }
            else if (ammoItem.type == ModContent.ItemType<ExplosiveGhostflame>())
            {
                Item.mana = ExplosiveGhostflame.manaCost;
                Item.useTime = Item.useAnimation = 58;
                Item.UseSound = SoundID.Item73;
            }
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string[] lLine = { "mana", "speed", "knockback" };
            tooltips.RemoveAll(line =>
                lLine.Any(word => line.Text.ToLower().Contains(word.ToLower()))
            );
        }
    }
}
