using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class MantaStyle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle mantaActivate = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/MantaStyle")
        {
            Volume = 0.2f,
            PitchVariance = .2f
        };
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 52;
            Item.damage = 80;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<MantaProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Yellow;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                SoundEngine.PlaySound(SoundID.Item71, player.Center);
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0, 0);

                if (player.ownedProjectileCounts[ModContent.ProjectileType<MantaSummon>()] > 0)
                {
                    SoundEngine.PlaySound(SoundID.Item71, player.Center);
                }

                // clone shooting
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == ModContent.ProjectileType<MantaSummon>() && p.owner == player.whoAmI)
                        p.ai[1] = 1f;
                }
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MantaSummon>()] <= 0)
                {
                    // summon the clones. position is determined by ai[0]
                    for (int i = 0; i < 2; i++)
                    {
                        // Stats are set to dynamically update, so damage, kb, crit have to be feeded
                        Projectile clone = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<MantaSummon>(), Item.damage, Item.knockBack, player.whoAmI, i);
                        clone.OriginalCritChance = Item.crit;
                    }
                }
            }
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            // only allow summoning clicking if the player doesn't have manta out
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MantaSummon>()] <= 0)
                {
                    Item.UseSound = mantaActivate;
                    Item.useTurn = false;
                    Item.noMelee = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Item.UseSound = SoundID.Item1;
                Item.useTurn = true;
                Item.noMelee = false;
            }
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player) => true;
        public static void KillShootProjectiles(bool shouldBreak, int projType, Player player)
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj.active && proj.owner == player.whoAmI && proj.type == projType)
                {
                    proj.Kill();
                    if (shouldBreak)
                        break;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.InfluxWaver, 1).
                AddIngredient(ItemID.SpectreBar, 15).
                AddIngredient<AghanimShard>(1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
