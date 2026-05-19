using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.MusicBoxes
{
    public class RideTheFireMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.MusicBox.RideTheFireMusicBox>();
        public override string MusicFilePath => "Sounds/Music/RideTheFire";
    }
}