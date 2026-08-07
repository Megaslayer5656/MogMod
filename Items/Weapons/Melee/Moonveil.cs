using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Items.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Moonveil : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle ParryStart = new($"{nameof(MogMod)}/Sounds/SE/ParryStart")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public override int ProjectileType => ModContent.ProjectileType<MoonveilHoldout>();
        public static int MaxCharges = 3;
        public static int Charges;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxCharges);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 64;

            Item.damage = 85;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
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
            AddRecipeGroup("AnyCobaltBar", 12).
            AddIngredient(ItemID.SoulofNight, 7).
            AddIngredient(ItemID.SoulofLight, 7).
            AddIngredient(ItemID.SoulofSight, 7).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}