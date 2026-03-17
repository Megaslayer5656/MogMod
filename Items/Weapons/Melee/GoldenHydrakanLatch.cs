using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Items.Weapons.Melee
{
    public class GoldenHydrakanLatch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults() //TODO: Test and balance, make better sprite
        {
            Item.width = 68;
            Item.height = 90;
            Item.scale = .65f;
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useTurn = false;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Yellow;
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
                AddIngredient(ItemID.GoldBar, 20).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Hardmode Evil Material"}", 12).
                AddIngredient<HydrakanLatch>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
