using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Utilities;
using Mono.Cecil;

namespace MogMod.Items.Weapons.Melee
{
    public class OversizedAnchor : ModItem
    {
        Random random = new Random();
        //TODO: Somehow make the offset look right for holding weapon
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 45;
            Item.useTurn = false;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            //Item.value = 
            Item.rare = ItemRarityID.Blue;
            Item.scale = 2f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            bool randomBool = random.Next(2) == 0;
            for (int i = 0; i < 3; i++)
            {
                // proj barrage does (source, Vector2 originVec, Vector2 targetPos, T/F fromRight, xOffsetMin, xOffsetMax, yOffsetMin, yOffsetMax, projSpeed, projType, damage, knockback, owner, T/F clamped, innacuracy)
                MogModUtils.ProjectileBarrage(source, target.Center, target.Center, randomBool, 200f, 200f, -200f, 200f, 4f, ModContent.ProjectileType<AnchorProj>(), 65, 3f, player.whoAmI, false, 0f);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("IronBar", 20).
                AddIngredient(ItemID.SharkFin, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
