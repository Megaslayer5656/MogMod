using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class RiversOfBlood : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle ParryStart = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ParryStart")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 70;

            Item.scale = 2.25f;
            Item.damage = 165;
            Item.knockBack = 5.5f;
            Item.DamageType = DamageClass.Melee;

            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4.5f;

            Item.autoReuse = true;
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
                player.AddBuff(ModContent.BuffType<ParrySlow>(), 90);
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
            AddIngredient(ModContent.ItemType<Moonveil>()).
            AddIngredient(ModContent.ItemType<Reduvia>()).
            AddIngredient(ModContent.ItemType<LizhardBloodVial>()).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
