using Terraria.ModLoader;
using Terraria;
using MogMod.Common.MogModPlayer;
using Terraria.ID;
using MogMod.Buffs.PotionBuffs;

namespace MogMod.Items.Weapons.Melee
{
    public class GoldenHydrakanLatch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            //TODO: Nerf and possibly change projectile
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
            Item.shoot = ProjectileID.IchorSplash;
            Item.shootSpeed = 3.5f;
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
                AddRecipeGroup("Ichor", 20).
                AddIngredient<HydrakanLatch>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
