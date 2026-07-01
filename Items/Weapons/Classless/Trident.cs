using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using MogMod.Items.Weapons.Magic;
using MogMod.Common.Classes;
using MogMod.Projectiles.Classless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Classless
{
    public class Trident : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Classless";
        public override void SetStaticDefaults() => ItemID.Sets.Spears[Item.type] = true;
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 66;

            Item.damage = 50;
            Item.DamageType = MeleeRangedMagicDamageClass.Instance;
            Item.useAnimation = Item.useTime = 30;
            Item.knockBack = 7f;
            
            Item.noMelee = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<TridentSpear>();
            Item.shootSpeed = 8f;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 11;
        public override bool MeleePrefix() => true;
        //public override bool RangedPrefix() => true;
        //public override bool MagicPrefix() => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>().
                AddIngredient<Yasha>().
                AddIngredient<Kaya>().
                AddIngredient<SoulOfMogMod>().
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}