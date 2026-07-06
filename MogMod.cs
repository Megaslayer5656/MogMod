using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates; // what could this possibly even do
using Terraria.ModLoader;
using MogMod.Common.Systems;
using Terraria.ID;
using MogMod.Items.Weapons.Melee;

namespace MogMod
{
    public class MogMod : Mod
    {
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