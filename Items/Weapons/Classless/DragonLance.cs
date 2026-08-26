using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.Melee;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Classless
{
    public class DragonLance : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Classless";
        public override int ProjectileType => ProjectileID.PurificationPowder; // temp slop
        //public override int ProjectileType => ModContent.ProjectileType<DragonLanceHoldout>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 60;

            Item.damage = 114;
            Item.crit = 7;
            Item.DamageType = MeleeRangedDamageClass.Instance;
            Item.useAnimation = Item.useTime = 60;
            Item.knockBack = 10f;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
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
                SoundEngine.PlaySound(SoundID.DD2_GoblinBomberThrow with { Pitch = -0.1f }, player.Center);
                Projectile.NewProjectile(source, position, velocity * 3, ModContent.ProjectileType<YashaProj>(), damage, knockback, player.whoAmI);
                return false;
            }
            base.Shoot(player, source, position, velocity, type, damage, knockback);
            Item.useStyle = ItemUseStyleID.Shoot;
            return true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BeltOfStrength>(1).
                AddIngredient<FuciumBar>(7).
                AddIngredient(ItemID.Ruby, 5).
                AddIngredient(ItemID.AntlionMandible, 3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}