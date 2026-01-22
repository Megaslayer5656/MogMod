using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class RedKatana : ModItem
    {
        public static readonly SoundStyle ParryStart = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ParryStart")
        {
            Volume = .4f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };
        public override void SetDefaults()
        {
            Item.width = 9;
            Item.height = 83;
            Item.damage = 13;
            Item.scale = .75f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
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
            AddRecipeGroup("Wood", 25).
            AddRecipeGroup("IronBar", 15).
            AddTile(TileID.WorkBenches).
            Register(); //TODO: Add something else cool to this recipe (pre boss still)
        }
    }
}
