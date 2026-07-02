using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Items.Global;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class BlinkDagger : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 28;
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 5;
            Item.useTurn = false;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                DustEffects(player);
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 10;
                Item.useAnimation = 10;
                Item.useAnimation = ItemUseStyleID.HoldUp;
                Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/BlinkSound")
                {
                    Volume = .3f,
                    PitchVariance = .02f,
                };
                DustEffects(player);
                player.Teleport(Main.MouseWorld, TeleportationStyleID.DebugTeleport);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y, TeleportationStyleID.DebugTeleport); //Needed for multiplayer
                player.AddBuff(ModContent.BuffType<BlinkDebuff>(), 600);
            }
            else
            {
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.UseSound = SoundID.Item1;
            }
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<BlinkDebuff>())
            {
                return true;
            }
            return false;
        }
        public void DustEffects(Player player)
        {
            for (int i = 0; i < 35; i++)
            {
                Vector2 dustCorner = player.position - 2f * Vector2.One;
                Vector2 dustVel = player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int dust = Dust.NewDust(dustCorner, player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 67 : 68, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.85f;
                Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.2f;
                }
            }
        }
    }
}