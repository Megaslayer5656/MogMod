using Microsoft.Xna.Framework;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Crystalys : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults() //TODO: make this weapon do something cool
        {
            Item.width = Item.height = 60;
            Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => modifiers.CritDamage *= 1.6f;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladesOfAttack>(1).
                AddRecipeGroup("AdamantiteBar", 10).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}