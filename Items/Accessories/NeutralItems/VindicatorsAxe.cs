using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories.NeutralItems
{
    // take and deal 15% more damage when silenced (which means mage (and maybe summoner) cant use it)
    public class VindicatorsAxe : NeutralItem
    {
        public const int ArmorPenetration = 8;
        public const float SilencedDamageBoost = 0.16f;
        public const int SilenceDuration = 15 * 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration, SilencedDamageBoost.ToPercent(), SilenceDuration.FramesToSeconds());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 38;
            Item.height = 32;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingVindicatorsAxe = true;
            player.GetArmorPenetration(DamageClass.Generic) += ArmorPenetration;
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        // TODO: move this to folder in mogmodplayer
        public static void VindicatorsAxeEffect()
        {
            MogPlayer mogPlayer = Player.MogMod();
            //if (KeybindSystem.NeutralItemKeybind.JustPressed && wearingVindicatorsAxe && !Player.HasBuff(BuffID.Silenced))
            //{
            //    Player.AddBuff(BuffID.Silenced, VindicatorsAxe.SilenceDuration);
            //    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, Player.Center);
            //    VindicatorsAxeDustEffect(player);
            //    if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            //    {
            //        VindicatorsAxeSync(Player, false);
            //    }
            //}
        }
        public static void VindicatorsAxeDustEffect(Player player)
        {
            for (int i = 0; i < 80; i++)
            {
                int shiva1 = Dust.NewDust(player.Center, player.width, player.height, DustID.Flare, player.velocity.X * 3, player.velocity.Y * 3, 0, default, 3f);
                //Main.dust[shiva1].noGravity = true;
            }
        }
        public static void VindicatorsAxeSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.VindicatorsAxeSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleVindicatorsAxe(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                VindicatorsAxeSync(client, true);
            }
            VindicatorsAxeDustEffect(client);
        }
    }
}