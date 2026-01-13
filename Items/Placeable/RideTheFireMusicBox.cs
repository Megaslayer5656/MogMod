using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class RideTheFireMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.RideTheFireMusicBox>();
        public override string MusicFilePath => "Sounds/Music/RideTheFire";
    }
}