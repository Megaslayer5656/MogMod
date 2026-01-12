using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace MogMod.Tiles
{
    public class ObserverWardTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.DisableSmartInteract[Type] = true;
            DustType = DustID.Torch;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addTile(Type);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            // We can determine the item to show on the cursor by getting the tile style and looking up the corresponding item drop.
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            // If the torch is on
            if (tile.TileFrameX < 66)
            {
                int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
                // Make it emit the following light.
                if (style == 0)
                {
                    r = 15.9f;
                    g = 15.9f;
                    b = 15.9f;
                }
                else if (style == 1)
                {
                    r = 0.5f;
                    g = 1.5f;
                    b = 0.5f;
                }
            }
        }

        public override void EmitParticles(int i, int j, Tile tileCache, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (!visible)
            {
                return;
            }

            if (Main.rand.NextBool(40) && tileFrameX < 66)
            {
                int dustChoice = DustID.Torch;
                Dust dust;
                Vector2 spawnPosition = tileFrameX switch
                {
                    22 => new Vector2(i * 16 + 6, j * 16),
                    44 => new Vector2(i * 16 + 2, j * 16),
                    _ => new Vector2(i * 16 + 4, j * 16)
                };

                dust = Dust.NewDustDirect(new Vector2(i * 16 + 4, j * 16), 4, 4, dustChoice, 0f, 0f, 100);

                if (!Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                }

                dust.velocity *= 0.3f;
                dust.velocity.Y -= 1.5f;
            }
        }
    }
}
