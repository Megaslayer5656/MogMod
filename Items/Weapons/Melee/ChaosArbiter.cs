using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class ChaosArbiter : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<ChaosArbiterHoldout>();
        public const float UltraCritChance = 0.27f;
        public const float CritMult = 2.7f;
        public const float BoltChance = 0.17f;
        public const int MaxPhantoms = 7;
        public const int PhantomLifetime = 1020;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(UltraCritChance.ToPercent(), CritMult, BoltChance.ToPercent(), MaxPhantoms, PhantomLifetime.FramesToSeconds());
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 66;
            Item.height = 72;

            Item.damage = 97;
            Item.crit = 23;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 40;
            Item.knockBack = 10f;
            Item.autoReuse = true;

            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // clone shooting
            base.Shoot(player, source, position, velocity, type, damage, knockback);
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == ModContent.ProjectileType<ChaosArbiterClone>() && p.owner == player.whoAmI)
                    p.ai[1] = 1f;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChaosBlade>().
                AddIngredient<GriefBar>(3).
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient<LizhardBloodVial>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}