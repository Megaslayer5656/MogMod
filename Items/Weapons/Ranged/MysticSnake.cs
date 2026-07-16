using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class MysticSnake : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 64;
            Item.damage = 46;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, 0f); // 0,0 is player center
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int maxArrow = 0;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (!mogPlayer.wearingEyeOfSkadi)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (maxArrow >= 6) break;

                    NPC otherNPC = Main.npc[i];
                    if (otherNPC.active && otherNPC.friendly == false && otherNPC.CountsAsACritter == false && otherNPC.whoAmI != otherNPC.whoAmI - 1 && otherNPC.type != NPCID.TargetDummy)
                    {
                        if (Microsoft.Xna.Framework.Vector2.Distance(player.Center, otherNPC.Center) < 550f)
                        {
                            float SpeedX = velocity.X + (float)Main.rand.Next(-25, 26) * 0.05f;
                            float SpeedY = velocity.Y + (float)Main.rand.Next(-25, 26) * 0.05f;
                            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<MysticSnakeProj>(), damage, knockback, player.whoAmI, 0f, 0f);
                            maxArrow++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-25, 26) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-25, 26) * 0.05f;
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<MysticSnakeProj>(), damage, knockback, player.whoAmI, 0f, 0f);
                }
            }
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<MysticSnakeProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MedusaHead).
                AddIngredient<BrinyRind>(16).
                AddIngredient(ItemID.BeetleHusk, 8).
                AddIngredient<ManaCore>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}