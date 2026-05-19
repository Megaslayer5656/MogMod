using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.MusicBoxes
{
    public class VonEvilIncarnateMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.MusicBox.VonEvilIncarnateMusicBox>();
        public override string MusicFilePath => "Sounds/Music/VonTheme2";
    }
}