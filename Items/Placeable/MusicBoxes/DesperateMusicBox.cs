using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.MusicBoxes
{
    public class DesperateMusicBox : MusicBox
    {
        public override int MusicBoxTile => ModContent.TileType<Tiles.MusicBox.DesperateMusicBox>();
        public override string MusicFilePath => "Sounds/Music/Desperate";
    }
}