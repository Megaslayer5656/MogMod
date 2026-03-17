using MogMod.Items.Accessories;
using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace MogMod.Items.Weapons.Melee
{
    public class Crystalys : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useTurn = false;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            //Item.value = 
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 1.6f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AdamantiteBar", 28).
                AddIngredient<BladesOfAttack>(1).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}