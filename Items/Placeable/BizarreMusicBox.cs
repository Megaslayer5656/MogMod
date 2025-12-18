using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class BizarreMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.BizarreMusicBox>();
        public override string MusicFilePath => "Sounds/Music/Bizarre";
    }
}