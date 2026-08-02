using log4net;
using MogMod.Common.Systems;
using MogMod.Items.Weapons.Melee;
using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates; // what could this possibly even do
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