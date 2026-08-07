using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class RedKatana : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle ParryStart = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ParryStart")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 54;
            Item.damage = 18;
            Item.scale = 1f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(ParryStart, player.Center);
                return false;
            } else if (player.HasBuff(ModContent.BuffType<ParrySlow>())){
                return false;
            } else
            {
                return true;
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<ParryCooldown>())
            {
                // we should turn this into a function in modplayer so we modify it easier
                player.AddBuff(ModContent.BuffType<Parrying>(), 30); //Actually accurate to Sekiro parry timing
                player.AddBuff(ModContent.BuffType<ParryCooldown>(), 600);
                player.AddBuff(ModContent.BuffType<ParrySlow>(), 90);
                return true;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnySilverBar", 10).
                AddRecipeGroup("AnyEvilMaterial", 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
