using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories
{
    public class ArmletOfMordiggian : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float DamageMult = 0.05f;
        public const float ArmletDamageMult = 0.15f;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
            Item.defense = 7;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += DamageMult;
            player.GetDamage(DamageClass.Generic) += DamageMult;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingArmlet = true;
            if (Main.zenithWorld) mogPlayer.armletActive = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.ArmletKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<ArmletOfMordiggian>("TooltipGFB").Format());
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<ArmletOfMordiggian>("TooltipNormal").Format(
                    DamageMult.ToPercent(), 
                    Hotkey,
                    ArmletDamageMult.ToPercent()));
                    tooltips.Insert(index, normal);
                }
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddIngredient(ItemID.FeralClaws).
                AddIngredient<BladesOfAttack>().
                AddRecipeGroup("AnyEmblem").
                AddRecipeGroup("AnyAdamantiteBar", 10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void ArmletEffect()
        {
            if (KeybindSystem.ArmletKeybind.JustPressed && wearingArmlet)
            {
                if (armletTimer <= armletTimerMax)
                {
                    Player.AddBuff(ArmletToggled, 9999999);
                    armletTimer++;
                    ArmletDustEffect(Player, true);
                    if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ArmletSync(Player, true, false);
                    }
                }
                else if (armletTimer >= armletTimerMax)
                {
                    Player.ClearBuff(ArmletToggled);
                    armletTimer = 0;
                    if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ArmletSync(Player, false, false);
                    }
                }
            }
            if (!wearingArmlet)
            {
                Player.ClearBuff(ArmletToggled);
                armletTimer = 0;
            }
            while (armletTimer >= 1 && armletTimer <= armletTimerMax + 1) armletTimer++;
        }
        public void ArmletBuff(bool GFB)
        {
            if (GFB)
            {
                if (wearingArmlet && armletActive) DamageOverTime(30);
            }
            if (armletActive)
            {
                Player.GetAttackSpeed<GenericDamageClass>() += ArmletOfMordiggian.ArmletDamageMult;
                Player.GetDamage<GenericDamageClass>() += ArmletOfMordiggian.ArmletDamageMult;
            }
        }
        public static void ArmletDustEffect(Player player, bool activated)
        {
            if (activated)
            {
                SoundEngine.PlaySound(ArmletOnSound, player.Center);
                for (int i = 0; i < 150; i++)
                {
                    Vector2 shinePos = Main.rand.NextVector2Circular(player.width * 0.5f, player.height * 0.25f);
                    Vector2 shineVel = new Vector2((Main.rand.NextBool() == true ? -1 : 1), (Main.rand.NextBool() == true ? -1 : 1)) * Main.rand.NextFloat(-3.5f, 3.51f);
                    Dust armletX = Dust.NewDustPerfect(player.Center + shinePos, DustID.CrimsonTorch, shineVel, 100);
                    armletX.fadeIn = 1.3f;
                    armletX.velocity *= 0.98f;
                    armletX.noGravity = true;
                }
            }
            else SoundEngine.PlaySound(ArmletOffSound, player.Center);
        }
        public void ArmletSync(Player player, bool activated, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.ArmletSync);
            packet.Write(player.whoAmI);
            packet.Write(activated);

            player.SendPacket(packet, server);
        }
        internal void HandleArmlet(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            bool activated = reader.ReadBoolean();
            if (Main.netMode == NetmodeID.Server)
            {
                ArmletSync(client, activated, true);
            }
            ArmletDustEffect(client, activated);
        }
    }
}