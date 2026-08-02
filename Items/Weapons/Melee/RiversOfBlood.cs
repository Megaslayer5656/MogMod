using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class RiversOfBlood : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle ParryStart = new($"{nameof(MogMod)}/Sounds/SE/ParryStart")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public const int BuffTime = 600;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffTime.FramesToSeconds());
        public const int ItemBloodDamage = 135;
        public const int ProjectileBloodDamage = 300;
        public override int ProjectileType => ModContent.ProjectileType<RiversOfBloodHoldout>();
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 54;
            Item.height = 70;

            Item.damage = 165;
            Item.knockBack = 8f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 22;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = ItemBloodDamage;
        }
        public override bool CanUseItem(Player player)
        {
            var mogPlayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (player.altFunctionUse == 2)
            {
                mogPlayer.swingNum = 0;
                SoundEngine.PlaySound(ParryStart, player.Center);
                return false;
            }
            else if (player.HasBuff(ModContent.BuffType<ParrySlow>()))
                return false;
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<ParryCooldown>())
            {
                player.AddBuff(ModContent.BuffType<Parrying>(), 30);
                player.AddBuff(ModContent.BuffType<ParryCooldown>(), 600);
                player.AddBuff(ModContent.BuffType<ParrySlow>(), 90);
                return true;
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