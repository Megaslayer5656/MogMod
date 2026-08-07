using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class GoldenHydrakanLatch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const int EssenceMax = 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(EssenceMax);
        public override void SetDefaults() //TODO: Test and balance, make better sprite
        {
            Item.width = 40;
            Item.height = 48;
            Item.scale = 1.25f;
            Item.damage = 37;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useTurn = false;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 3.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<EssenceShift>(), 600);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.essenceShiftLevel += 1;
                if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    mogPlayer.SyncEssenceShift(false);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 20).
                AddRecipeGroup("AnyScaleOrTissue", 12).
                AddIngredient<HydrakanLatch>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
