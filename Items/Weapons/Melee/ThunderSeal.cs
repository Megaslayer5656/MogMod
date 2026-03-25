using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using MogMod.Utilities;
using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria.DataStructures;

namespace MogMod.Items.Weapons.Melee
{
    public class ThunderSeal : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        
        // lets you hold right click
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.damage = 60;
            Item.scale = 1.25f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 65, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
                Item.useStyle = ItemUseStyleID.Swing;
                int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<StunEdge>(), 38, knockback);
            } else
            {
                Item.noMelee = false;
                Item.useStyle = ItemUseStyleID.Swing;
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<ShockState>())
            {
                //TODO: Eventually make it a lightning strike instead of a dust particle attack
                var source = target.GetSource_FromAI();
                for (int x = 0; x < 4; x++)
                {
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ThunderSealProj>(), 38, 0f, player.whoAmI, false, 0f);
                }
            }
        }

        public override void AddRecipes()
        {
            //TODO: Make this recipe more interesting but not too grindy
            CreateRecipe().
                AddIngredient(ItemID.SoulofLight, 10).
                AddIngredient(ItemID.SoulofFlight, 5).
                AddRecipeGroup("CobaltBar", 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
