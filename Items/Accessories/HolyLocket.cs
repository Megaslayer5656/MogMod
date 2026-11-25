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
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories
{
    public class HolyLocket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxMinionBoost = 2;
        public const float MagicAndSummonDamageBoost = 0.07f;
        public const int ManaBoost = 70;
        public const int LifeHeal = 10; // shared with mana
        public const int ManaHeal = 10; // shared with life
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxMinionBoost, MagicAndSummonDamageBoost.ToPercent(), ManaBoost, LifeHeal);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.WandKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingHolyLocket = true;
            player.GetDamage(DamageClass.Magic) += MagicAndSummonDamageBoost;
            player.GetDamage(DamageClass.Summon) += MagicAndSummonDamageBoost;
            player.statManaMax2 += ManaBoost;
            player.maxMinions += MaxMinionBoost;
            float dim = .02f;
            Lighting.AddLight(player.Center, 75 * dim, 73 * dim, 61 * dim);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagicWand>().
                AddIngredient(ItemID.PygmyNecklace).
                AddIngredient<Diadem>().
                AddIngredient<SolRing>().
                AddRecipeGroup("AnyAdamantiteBar", 8).
                AddIngredient(ItemID.SoulofSight, 7).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void HolyLocketEffect()
        {
            if (KeybindSystem.WandKeybind.JustPressed && wearingHolyLocket && locketCharges > 0)
            {
                HolyLocketBuff(false);
                HolyLocketDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    HolyLocketSync(Player, false);
                }
            }
        }
        public void HolyLocketBuff(bool hit)
        {
            if (hit)
            {
                if (wearingHolyLocket)
                {
                    locketCharges++;
                    if (locketCharges > maxLocketCharges)
                    {
                        locketCharges = maxLocketCharges;
                    }
                }
            }
            else
            {
                int heal = HolyLocket.LifeHeal * locketCharges;
                int mana = HolyLocket.ManaHeal * locketCharges;

                Player.HealLifeMult(heal);
                Player.HealManaMult(mana);

                locketCharges = 0;
                SoundEngine.PlaySound(WandUse, Player.Center);
            }
        }
        public static void HolyLocketDustEffect(Player player)
        {
            SoundEngine.PlaySound(WandUse, player.Center);
            for (int i = 0; i < 150; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2CircularEdge(player.width * 1.5f, player.height * 1.25f);
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-0.25f, 1.26f);
                Dust locketOval = Dust.NewDustPerfect(player.Center + shinePos, DustID.RainbowTorch, shineVel, 100, Color.Goldenrod);
                locketOval.fadeIn = 1.45f;
                locketOval.velocity *= 0.98f;
                locketOval.scale *= 0.375f;
                locketOval.noGravity = true;

                if (i % 2 == 0)
                {
                    float velocity = 3.01f;
                    Vector2 xPos = Main.rand.NextVector2Circular(player.width * 0.25f, player.height * 0.125f);
                    Vector2 xVel = new Vector2(Main.rand.NextBool() == true ? -1 : 1, Main.rand.NextBool() == true ? -1 : 1) * Main.rand.NextFloat(-velocity, velocity + 0.01f);
                    Dust locketX = Dust.NewDustPerfect(player.Center + xPos, DustID.RainbowTorch, xVel, 100, Color.Goldenrod);
                    locketX.fadeIn = 1.5f;
                    locketX.velocity *= 0.98f;
                    locketOval.scale *= 0.5f;
                    locketX.noGravity = true;
                }
            }
        }
        public void HolyLocketSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.HolyLocketSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleHolyLocket(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                HolyLocketSync(client, true);
            }
            HolyLocketDustEffect(client);
        }
    }
}