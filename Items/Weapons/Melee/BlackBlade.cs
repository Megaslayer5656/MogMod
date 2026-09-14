using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class BlackBlade : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<BlackBladeHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 114;

            Item.damage = 248;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 60;
            Item.knockBack = 14f;
            Item.channel = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1 with { Pitch = -0.1f };

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage *= Main.zenithWorld ? 5f : 1f;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BreakerBlade, 1).
                AddIngredient<WyvernJawblade>().
                AddIngredient<SpookyEssence>(20).
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}