using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class RajangMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.RajangMusicBox>();
        public override string MusicFilePath => "Sounds/Music/Rajang";
    }
}