using Terraria.ModLoader;

namespace MogMod.Items.Placeable.MusicBoxes
{
    public class BizarreMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.MusicBox.BizarreMusicBox>();
        public override string MusicFilePath => "Sounds/Music/Bizarre";
    }
}