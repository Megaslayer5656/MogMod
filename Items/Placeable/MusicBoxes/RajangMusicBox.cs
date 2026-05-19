using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.MusicBoxes
{
    public class RajangMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.MusicBox.RajangMusicBox>();
        public override string MusicFilePath => "Sounds/Music/Rajang";
    }
}