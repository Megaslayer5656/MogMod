using log4net;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Systems;
using MogMod.Items.Weapons.Melee;
using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates; // what could this possibly even do
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod
{
    public class MogMod : Mod
    {
        internal static MogMod Instance => _Instance ??= ModContent.GetInstance<MogMod>();
        private static MogMod _Instance;
        internal static ILog Log => Instance.Logger;
        public override void Load()
        {
            GameShaders.Misc["MogMod:FlameLashRGB"] = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);
            GameShaders.Misc["MogMod:FlameLashRGB"].UseImage0(ModContent.Request<Texture2D>("MogMod/Assets/Trails/FlameLashRGB", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            GameShaders.Misc["MogMod:FlameLashRGB"].UseImage1("Images/Extra_189");
            GameShaders.Misc["MogMod:FlameLashRGB"].UseImage2("Images/Extra_190");

            GameShaders.Misc["MogMod:MagicMissileRGB"] = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);
            GameShaders.Misc["MogMod:MagicMissileRGB"].UseImage0(ModContent.Request<Texture2D>("MogMod/Assets/Trails/MagicMissileRGB", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            GameShaders.Misc["MogMod:MagicMissileRGB"].UseImage1("Images/Extra_194");
            GameShaders.Misc["MogMod:MagicMissileRGB"].UseImage2("Images/Extra_193");
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MogModNetcode.HandlePacket(this, reader, whoAmI);
        }

        public override void PostSetupContent() //For some reason this has to be here to make shimmer work for this item specifically.
        {
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Flamebrand>()] = ItemID.Frostbrand;
        }
    }
}