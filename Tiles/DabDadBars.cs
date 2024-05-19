using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.DataStructures;

namespace MogMod.Tiles
{
        public class DabDadBars : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type]= true;
            Main.tileSolidTop[Type]= true;
            Main.tileShine[Type]= 1100;
            Main.tileFrameImportant[Type]= true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(34, 101, 33), Language.GetText("MapObject.MetalBar"));
        }
    }
}
