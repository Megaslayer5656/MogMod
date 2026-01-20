using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class OversizedAnchor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        //TODO: Somehow make the offset look right for holding weapon <-- an issue with how the player holds melee weapons, calamity's "Earth" weapon has the same problem
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 78;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<AnchorProj>(); // melee weapons have to shoot something you want to aim it, look at blade of grass vs volcano
            Item.shootSpeed = 8f;
            Item.useTime = 45;
            Item.useTurn = false;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            //Item.value = 
            Item.rare = ItemRarityID.Cyan;
            Item.scale = 2f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AnchorProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // fire dolphins
            var source = player.GetSource_OnHit(target);
            bool randomBool = random.Next(2) == 0;
            for (int i = 0; i < 3; i++)
            {
                // proj barrage does (source, Vector2 originVec, Vector2 targetPos, T/F fromRight, xOffsetMin, xOffsetMax, yOffsetMin, yOffsetMax, projSpeed, projType, damage, knockback, owner, T/F clamped, innacuracy)
                MogModUtils.ProjectileBarrage(source, target.Center, target.Center, randomBool, 200f, 200f, -200f, 200f, 4f, ModContent.ProjectileType<AnchorProj>(), Convert.ToInt32(Item.damage * .65), 3f, player.whoAmI, false, 0f);
            }
        }

        // added an anchor to the recipe but made anchors craftable
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Anchor, 1).
                AddIngredient(ItemID.SharkFin, 5).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
