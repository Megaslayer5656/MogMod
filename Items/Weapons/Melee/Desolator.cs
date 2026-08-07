using MogMod.Buffs.Debuffs;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Desolator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 105;
            Item.height = 96;
            Item.damage = 115;
            Item.knockBack = 9f;
            Item.useTime = Item.useAnimation = 22;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.UseSound = SoundID.Item71;
            Item.shoot = ModContent.ProjectileType<DesolatorProj>();
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }

        // more damage the lower life you have
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            damage.Base += lifeAmount * 0.2f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.BetsysCurse, 600);
            target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), 300);
        }
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.BetsysCurse, 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
            target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), 300);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DeathSickle).
                AddRecipeGroup("AnyScaleOrTissue", 12).
                AddIngredient(ItemID.FragmentSolar, 8).
                AddIngredient<GriefBar>(5).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
