using System;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class TheMarker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 97;
            Item.height = 96;
            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 13f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
            Item.scale = 1f;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void AddRecipes()
        {
            //TODO: Add recipe
        }
    }
}