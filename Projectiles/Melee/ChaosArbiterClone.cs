using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class ChaosArbiterClone : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public Player clone;
        // How far the clone should move from the player
        private Vector2 moveTo;
        public bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = ChaosArbiter.PhantomLifetime;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
            //writer.WriteVector2(Projectile.position);
            writer.WriteVector2(Projectile.Center);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
            //Projectile.position = reader.ReadVector2();
            Projectile.Center = reader.ReadVector2();
        }
        public override void AI()
        {
            if (!initialized)
            {
                moveTo = Main.rand.NextVector2CircularEdge(175f, 175f);
                initialized = true;
                Projectile.netUpdate = true;
            }
            // kill minion if the player isn't holding chaos arbiter
            if (Owner.HeldItem.type != ModContent.ItemType<ChaosArbiter>() || !Owner.active || Owner.CCed || Owner == null)
            {
                Projectile.Kill();
                return;
            }
            // if the velocity is not zero, the visuals get offset weirdly
            Projectile.velocity = Vector2.Zero;
            // move the clone to the desired position
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center + moveTo, 0.4f);
            // produce smoke during initial move
            if (Projectile.Distance(Owner.Center + moveTo) < 16)
            {
                Projectile.ai[2]++;
                //if (Projectile.ai[2] % 2 == 0) Projectile.netUpdate = true;
            }
            if (Projectile.ai[2] == 0)
            {
                int dustsplash = 0;
                while (dustsplash < 4)
                {
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                    Main.dust[d].position = Projectile.Center;
                    dustsplash += 1;
                }
            }
            int holdoutProj = ModContent.ProjectileType<ChaosArbiterHoldout>();
            // shoot bolts while the player is attacking
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1] = 0;
                Vector2 direction = Projectile.Center.DirectionTo(Main.MouseWorld);
                Projectile.direction = Math.Sign(direction.X);
                Projectile.netUpdate = true;
                // summon the sword holdout
                if (Projectile.owner == Main.myPlayer && !clone.active)
                {
                    Projectile holdout = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, holdoutProj, (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                    holdout.active = true;
                    // set the identity to the clones identity
                    holdout.identity = Projectile.identity;
                    // set the sword holdouts center to be the clones center
                    BaseSwordHoldoutProjectile mogProj = holdout.ModProjectile<BaseSwordHoldoutProjectile>();
                    mogProj.ProjectilePosition = Projectile.Center;
                }
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                // if the projectile in activeprojectiles is a sword holdout, is owned by the player, and its identity = the clones identity, set its center to the clones center
                if (p.type == holdoutProj && p.owner == Projectile.owner && p.identity == Projectile.identity)
                {
                    BaseSwordHoldoutProjectile mogProj = p.ModProjectile<BaseSwordHoldoutProjectile>();
                    mogProj.ProjectilePosition = Projectile.Center;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // code taken fron "The dark master" weapon from calamity
            // make a player visual clone. it inherits the player's hair type and clothes style and is otherwise all blue with red pupils
            // Main.playerVisualClone[Projectile.owner] will throw stack trace errors on reloads
            clone ??= new Player();
            clone.CopyVisuals(Owner);
            clone.skinColor = Color.DarkRed;
            clone.shirtColor = Color.DarkRed;
            clone.underShirtColor = Color.DarkRed;
            clone.pantsColor = Color.DarkRed;
            clone.shoeColor = Color.DarkRed;
            clone.hairColor = Color.DarkRed;
            clone.eyeColor = Color.White;
            // red manta effect
            for (int i = 0; i < clone.dye.Length; i++)
                if (clone.dye[i].type != ItemID.FlameDye)
                    clone.dye[i].SetDefaults(ItemID.FlameDye);
            // updates 
            clone.ResetEffects();
            clone.ResetVisibleAccessories();
            clone.DisplayDollUpdate();
            clone.UpdateSocialShadow();
            clone.UpdateDyes();
            clone.PlayerFrame();
            // copy the player's arm movements while swinging, otherwise idle
            if (Owner.ItemAnimationActive && Owner.altFunctionUse != 2) clone.bodyFrame = Owner.bodyFrame;
            else clone.bodyFrame.Y = 0;
            // legs never jump or walk
            clone.legFrame.Y = 0;
            // face towards the player's cursor
            clone.direction = Math.Sign(Projectile.DirectionTo(Main.MouseWorld).X);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, clone, Projectile.position, 0f, clone.fullRotationOrigin, 0f, 1f);
            /*
            // draw the sword
            if (Owner.ItemAnimationActive && Owner.altFunctionUse != 2)
            {
                Texture2D Sword = ModContent.Request<Texture2D>("MogMod/Projectiles/Melee/ChaosArbiterCloneSword").Value;
                Vector2 distToPlayer = Projectile.position - Owner.position;
                Main.EntitySpriteDraw(Sword, (Vector2)Owner.HandPosition + distToPlayer - Main.screenPosition, null, lightColor, Owner.direction == clone.direction ? Owner.itemRotation : -Owner.itemRotation, new Vector2(clone.direction == 1 ? 0 : Sword.Width, Sword.Height), 1.5f, clone.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            */
            return false;
        }
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
        public override bool? CanCutTiles() => true;
        public override bool? CanDamage() => false;
        public override void OnKill(int timeLeft)
        {
            int dustsplash = 0;
            while (dustsplash < 4)
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }
    }
}