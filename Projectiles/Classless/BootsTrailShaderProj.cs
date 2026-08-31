using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    // TODO: find a way to draw this using the player's old position && rotation
    // TODO: also move every texture instance of MogMod/Projectiles/BaseProjectiles to MogMod/Assets/Textures
    /// <summary>
    /// This projectiles entire existence is to draw a shader behind the player when they have certain accessories equipped. <br/>
    /// As far as I know, this effect cannot be done in a modplayer hook as that lacks the players old position. <br/>
    /// Player.oldPosition doesn't work as that is a Vector2, not a Vector2[] <br/>
    /// A Vector2[] value is needed to draw trails correctly. <br/> <br/>
    /// </summary>
    public class BootsTrailShaderProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public bool travelBoots = false;
        public bool ultraTravelBoots = false;
        public bool lunarBoots = false;
        public bool wingsOfLight = false;
        public bool allegianceWings = false;
        public override void SetStaticDefaults()
        {
            // Required for texture drawing.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Default;
        }
        public override void AI()
        {
            MogPlayer mogPlayer = Owner.MogMod();

            travelBoots = mogPlayer.wearingTravelBoots && mogPlayer.travelBootsVisual;
            ultraTravelBoots = mogPlayer.wearingUltraTravelBoots && mogPlayer.ultraTravelBootsVisual;
            lunarBoots = mogPlayer.wearingLunarBoots && mogPlayer.lunarBootsVisual;
            wingsOfLight = mogPlayer.wearingWingsOfLight && mogPlayer.wingsOfLightVisual;
            allegianceWings = mogPlayer.wearingAllegianceWings && mogPlayer.allegianceWingsVisual;

            int accCheck = (travelBoots ? 1 : 0) +
                (ultraTravelBoots ? 1 : 0) +
                (lunarBoots ? 1 : 0) +
                (wingsOfLight ? 1 : 0) +
                (allegianceWings ? 1 : 0);

            if (accCheck != 0 && Projectile.active)
            {
                // Rotation is required to draw the trail in all directions correctly.
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 playerPosition = Owner.Center + Vector2.UnitY * Owner.gfxOffY;
                    Vector2 orbAttemptedVelocity = Vector2.Zero.MoveTowards(playerPosition - Projectile.Center, 9999f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, orbAttemptedVelocity, 1f);
                    Projectile.netUpdate = true;
                }

                Projectile.timeLeft = 2;
            }

            // If the player's dead, delete the projectile.
            if (Owner.dead || accCheck == 0) Projectile.active = false;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.active)
                return false;
            TrailDrawer trailDrawer = default;
            string type = "MagicMissile";
            Color innerColor = Color.White;
            Color outerColor = Color.White;
            float width = 1f;
            float length = 16f;

            if (travelBoots)
            {
                outerColor = Color.White;
                innerColor = Color.OrangeRed;
                type = "MogMod:FlameLashRGB";
                width = 0.9f;
                trailDrawer.Draw(Projectile, type, outerColor, innerColor, width, length, length + 8f);
            }
            if (ultraTravelBoots)
            {
                outerColor = Color.LightGoldenrodYellow;
                innerColor = Color.Goldenrod;
                type = "MogMod:FlameLashRGB";
                width = 0.75f;
                length = 20f;
                trailDrawer.Draw(Projectile, type, outerColor, innerColor, width, length, length + 8f);
            }
            if (lunarBoots)
            {
                outerColor = Color.LightSkyBlue;
                innerColor = Color.SkyBlue;
                type = "MagicMissile";
                width = 0.6f;
                length = 24f;
                trailDrawer.Draw(Projectile, type, outerColor, innerColor, width, length, length + 8f);
            }
            if (wingsOfLight)
            {
                outerColor = Color.Goldenrod;
                innerColor = Color.DarkGoldenrod;
                type = "MogMod:FlameLashRGB";
                width = 0.7f;
                length = 18f;
                trailDrawer.Draw(Projectile, type, outerColor, innerColor, width, length, length + 8f);
            }
            if (allegianceWings)
            {
                outerColor = new(201, 238, 255);
                innerColor = new(255, 232, 201);
                //outerColor = Color.WhiteSmoke;
                //innerColor = Color.BlanchedAlmond;
                type = "MogMod:FlameLashRGB";
                string type2 = "MogMod:MagicMissileRGB";
                width = 0.6f;
                length = 30f;
                trailDrawer.Draw(Projectile, type, outerColor, innerColor, width + 0.25f, length - 4f, length + 4f);
                trailDrawer.Draw(Projectile, type2, outerColor, innerColor, width, length, length + 8f);
            }

            return false;
        }
    }
}
