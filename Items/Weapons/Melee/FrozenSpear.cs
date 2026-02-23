using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Ranged;

namespace MogMod.Items.Weapons.Melee
{
    public class FrozenSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 32;
            Item.useTime = 35;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.damage = 22;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<FrozenSpearProjectile>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TheRottedFork).
                AddIngredient(ItemID.SnowBlock, 20).
                AddIngredient(ItemID.IceBlock, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
