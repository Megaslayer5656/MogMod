using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.MusicBoxes
{
    public class KingVonMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.MusicBox.KingVonMusicBox>();
        public override string MusicFilePath => "Sounds/Music/VonTheme1";
    }
}