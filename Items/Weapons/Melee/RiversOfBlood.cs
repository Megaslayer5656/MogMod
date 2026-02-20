using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class RiversOfBlood : ModItem
    {
        public static readonly SoundStyle ParryStart = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ParryStart")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 101;
            Item.damage = 130;
            Item.scale = 1f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4.5f;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(ParryStart, player.Center);
                return false;
            }
            else if (player.HasBuff(ModContent.BuffType<ParrySlow>()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<ParryCooldown>())
            {
                player.AddBuff(ModContent.BuffType<Parrying>(), 30); //Actually accurate to Sekiro parry timing
                player.AddBuff(ModContent.BuffType<ParryCooldown>(), 600);
                player.AddBuff(ModContent.BuffType<ParrySlow>(), 60);
                return true;
            }
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (mogPlayer.riversOfBloodProj)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RiversOfBloodProj>(), Convert.ToInt32(Item.damage * 5f), knockback, player.whoAmI, 0f, 0f);

                mogPlayer.riversOfBloodProj = false;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient(ModContent.ItemType<RedKatana>()).
            AddIngredient(ModContent.ItemType<Reduvia>()).
            AddIngredient(ModContent.ItemType<LizhardBloodVial>()).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
