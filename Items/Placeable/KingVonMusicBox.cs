using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class KingVonMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.KingVonMusicBox>();
        public override string MusicFilePath => "Sounds/Music/VonTheme1";
    }
}