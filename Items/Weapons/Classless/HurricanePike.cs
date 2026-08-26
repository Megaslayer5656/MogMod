using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Items.Global;
using MogMod.Items.Tools;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Classless
{
    public class HurricanePike : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Classless";
        public override int ProjectileType => ProjectileID.PurificationPowder; // temp slop
        //public override int ProjectileType => ModContent.ProjectileType<HurricanePikeHoldout>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 60;

            Item.damage = 286;
            Item.crit = 17;
            Item.DamageType = MeleeRangedDamageClass.Instance;
            Item.useAnimation = Item.useTime = 65;
            Item.knockBack = 10f;

            Item.channel = true;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) Item.useTime = Item.useAnimation = 20;
            else Item.useTime = Item.useAnimation = 60;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Pitch = 0.1f }, player.Center);
                Projectile.NewProjectile(source, position, velocity * 3, ModContent.ProjectileType<ElysianSeraphThrownProj>(), damage, knockback, player.whoAmI);
                return false;
            }
            return false; // temp slop
            Item.useStyle = ItemUseStyleID.Shoot;
            base.Shoot(player, source, position, velocity, type, damage, knockback);
            return true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DragonLance>().
                AddIngredient<ForceStaff>().
                AddIngredient(ItemID.ShroomiteBar, 15).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient(ItemID.Silk, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}