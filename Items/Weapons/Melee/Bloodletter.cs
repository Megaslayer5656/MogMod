using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace MogMod.Items.Weapons.Melee
{
    //This sprite is genuinely so awful that I'm gonna fix it tomorrow. This is my worst one yet somehow.
    public class Bloodletter : ModItem, ILocalizedModType
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
            Item.width = 74;
            Item.height = 78;
            Item.damage = 28;
            Item.scale = 1.25f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 12).
                AddRecipeGroup(RecipeGroupID.IronBar, 12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
