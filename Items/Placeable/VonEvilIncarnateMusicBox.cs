using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class VonEvilIncarnateMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.VonEvilIncarnateMusicBox>();
        public override string MusicFilePath => "Sounds/Music/VonTheme2";
    }
}