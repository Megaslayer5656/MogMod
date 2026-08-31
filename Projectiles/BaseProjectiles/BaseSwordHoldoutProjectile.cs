using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    // taken and slightly modified from calamity mod
    // swinging for multiplayer clients is slightly off (works normally if swinging to the left)
    public class BaseSwordHoldoutPlayer : ModPlayer
    {
        /// <summary>
        /// Used to detect what swing a weapon is doing.
        /// </summary>
        public int swingNum = 0;

        public override void CopyClientState(ModPlayer targetCopy)
        {
            BaseSwordHoldoutPlayer clone = (BaseSwordHoldoutPlayer)targetCopy;
            clone.swingNum = swingNum;
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            BaseSwordHoldoutPlayer clone = (BaseSwordHoldoutPlayer)clientPlayer;
            if (swingNum != clone.swingNum)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
    }
    /// <summary>
    /// Manages all the required settings for custom sword holdouts done using BaseSwordHoldoutProjectile
    /// </summary>
    public abstract class BaseSwordHoldoutItem : ModItem
    {
        public virtual int ProjectileType { get; set; }
        public virtual bool SizeModifiers { get; set; } = true;

        public virtual bool RClickAutoswing { get; set; } = false;

        public override void SetStaticDefaults()
        {
            if (RClickAutoswing) ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override bool MeleePrefix()
        {
            return SizeModifiers;
        }
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.useStyle = ItemUseStyleID.Shoot;
            if (SizeModifiers) PrefixLegacy.ItemSets.SwordsHammersAxesPicks[Item.type] = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.itemTime > 0)
            {
                return false;
            }
            for (int i = 0; i < 1000; i++)
            {
                var proj = Main.projectile[i];
                if (proj.type == ProjectileType && proj.owner == player.whoAmI && proj.active)
                {
                    return false;
                }

            }
            return base.CanUseItem(player);
        }
    }
    public abstract class BaseSwordHoldoutProjectile : ModProjectile
    {
        #region Overrideable Fields
        /// <summary>
        /// The position of the projectile.
        /// Change to make the projectile center around a set position.
        /// <br/> Defaults to Vector2.Zero, where the players center is used.
        /// </summary>
        public virtual Vector2 ProjectilePosition { get; set; } = Vector2.Zero;
        /// <summary>
        /// How many hits it takes to reach <see cref="DamageMin"/>.
        /// <br/> Defaults to 7.
        /// </summary>
        public virtual int DamageHitCap { get; set; } = 7;
        /// <summary>
        /// The cap on the amount of damage lost each hit.
        /// <br/> Defaults to 0.3f.
        /// </summary>
        public virtual float DamageMin { get; set; } = 0.3f;
        /// <summary>
        /// The width, in degrees, of the sword swing.
        /// <br/> Defaults to 180
        /// </summary>
        public virtual int swingWidth { get; set; } = 180;
        /// <summary>
        /// How many frames the sword swing should take.
        /// <br/> Defaults to 20
        /// </summary>
        public virtual int swingTime { get; set; } = 20;
        /// <summary>
        /// If the swing should alternate directions each use.
        /// <br/> Defaults to <see langword="true"/>.
        /// </summary>
        public virtual bool AlternateSwings { get; set; } = true;
        /// <summary>
        /// How far the held sword should be offset from the player
        /// <br/> Defaults to 0
        /// </summary>
        public virtual int OffsetDistance { get; set; } = 0;
        /// <summary>
        /// What item this projectile uses as a base.
        /// </summary>
        public virtual Item BaseItem { get; set; }
        /// <summary>
        /// Whether or not this projectile uses a base item at all.
        /// <br/> Defaults to <see langword="true"/>.
        /// </summary>
        public virtual bool UsesBaseItem { get; set; } = true;

        /// <summary>
        /// Length of after-image trail left by the projectile.
        /// <br/> Defaults to 0
        /// </summary>
        public virtual int AfterImageLength { get; set; } = 0;

        /// <summary>
        /// Whether or not this should get melee speed bonuses
        /// <br/> Defaults to <see langword="true"/>.
        /// </summary>
        public virtual bool UseAttackSpeed { get; set; } = true;

        /// <summary>
        /// Whether or not this should get melee size bonuses (Titan Glove)
        /// <br/> Defaults to <see langword="true"/>.
        /// </summary>
        public virtual bool UseMeleeSize { get; set; } = true;

        /// <summary>
        /// How long before the weapon should begin it's actual swing once used.
        /// </summary>
        public virtual int StartupTime { get; set; } = 0;
        /// <summary>
        /// How long the weapon should "cool down" after swinging before ending the item use.
        /// </summary>
        public virtual int CooldownTime { get; set; } = 0;
        /// <summary>
        /// Speed at which the projectile should rotate to match the mouse angle during StartupTime.
        /// <br/> Set to 0 to disable.
        /// <br/> Defaults to 0.5f.
        /// </summary>
        public virtual float RotateInStartup { get; set; } = 0.5f;

        /// <summary>
        /// Speed at which the projectile should rotate to match the mouse angle during Cooldown.
        /// <br/> Set to 0 to disable.
        /// <br/> Defaults to 0.5f.
        /// </summary>
        public virtual float RotateInCooldown { get; set; } = 0.5f;

        /// <summary>
        /// What sound to use when the sword begins the actual swing (after startup frames)
        /// </summary>
        public virtual SoundStyle? UseSound { get; set; } = null;
        /// <summary>
        /// The length (from the player) of the projectile's line collision.
        /// This helps to prevent blindspots.
        /// <br/> Defaults to 0.
        /// </summary>
        public virtual float lineCollisionLength { get; set; } = 0;

        public virtual Color AfterImageColor { get; set; } = Color.White;
        /// <summary>
        /// Flips the sprite if set to true.
        /// <br/> <b> Doesn't work for now.</b>
        /// <br/> Defaults to <see langword="false"/>.
        /// </summary>
        public virtual bool FlipHoldoutSprite { get; set; } = false;

        #endregion

        #region Fields

        /// <summary>
        /// Angle of the swing. By default, gets set to mouse angle. Can be set in Spawn(IEntitySource) for fixed angles.
        /// </summary>
        public Vector2 angle { get; set; } = Vector2.Zero;
        /// <summary>
        /// The projectile's center based on offset to the player the previous update
        /// Can be used for effects that stay consistent in motion
        /// </summary>
        public Vector2 oldPlayerOffset { get; set; }
        /// <summary>
        /// Internal timer for the projectile's entire lifespan
        /// </summary>
        public int timer { get; set; } = 0;

        /// <summary>
        /// Internal timer for the projectile's swing animation
        /// </summary>
        internal int swingTimer = 0;

        public float baseScale;
        /// <summary>
        /// Old weapon scales used to track for trail drawing
        /// </summary>
        public List<float> oldScale = new List<float>();
        List<float> oldProjectileRot = new List<float> { };
        List<Vector2> oldProjectilePos = new List<Vector2> { };

        internal int ExistsTime = 20;
        public bool inStartup => timer < StartupTime;

        public bool inCooldown => timer > CooldownStartFrame;

        public bool inSwing => !(inStartup || inCooldown);

        public int CooldownStartFrame => swingTime + StartupTime;

        public int CooldownTimer => timer - CooldownStartFrame;

        public float StartupCompletion => timer / (float)StartupTime;
        public float SwingCompletion => swingTimer / (float)swingTime;
        public float CooldownCompletion => CooldownTimer / (float)CooldownTime;

        private bool hasFakedOnSpawn = false;

        private int syncTimer;
        private Vector2 mousePos;

        #endregion

        #region Overridable Methods  
        /// <summary>
        /// Called after movement but before timer increases. Use as to not cancel the default AI behavior.
        /// </summary>
        public virtual void AdditionalAI() { }
        /// <summary>
        /// Called at the beginning of AI the first frame.
        /// </summary>
        public virtual void Spawn() { }
        /// <summary>
        /// Called after SetDefaults. Use as not to cancel default SetDefaults behavior.
        /// </summary>
        public virtual void Defaults() { }
        /// <summary>
        /// Returns the swing offset from the center angle in radians. Automatically will be inverted if AlternateSwings is enabled.
        /// </summary>
        /// <returns></returns>
        public virtual float SwingFunction()
        {
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth / 2, swingWidth / 2, swingTimer / (float)swingTime));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// DO NOT OVERRIDE IN MOST SITUATIONS
        /// use Defaults() instead.
        /// That will set defaults after everything is set in the base projectile.
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.timeLeft = swingTime * 2;
            if (UsesBaseItem)
                Projectile.width = Projectile.height = Math.Max(BaseItem.height, BaseItem.width);
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 0;
            Projectile.aiStyle = -2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.tileCollide = false;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            Defaults();
        }
        /// <summary>
        /// This hook runs at the beginning of AI the first time through.
        /// </summary>
        private void FakeOnSpawn()
        {
            var player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
                Projectile.netUpdate = true;
            }
            if (ProjectilePosition != Vector2.Zero) angle = (ProjectilePosition - mousePos).SafeNormalize(Vector2.One);
            else angle = (player.MountedCenter - mousePos).SafeNormalize(Vector2.One);
            Projectile.velocity = Vector2.Zero;
            if (angle.X < 0)
            {
                if (ProjectilePosition == Vector2.Zero)
                {
                    player.direction = 1;
                    Projectile.spriteDirection = 1 * (int)player.gravDir;
                }
                else
                    Projectile.spriteDirection = 1;
            }
            else
            {
                if (ProjectilePosition == Vector2.Zero)
                {
                    player.direction = -1;
                    Projectile.spriteDirection = -1 * (int)player.gravDir;
                }
                else
                    Projectile.spriteDirection = -1;
            }
            if (AlternateSwings && player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 1)
                Projectile.spriteDirection *= -1;
            if (AlternateSwings && (ProjectilePosition == Vector2.Zero))
                player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum++;
            swingTime = Main.player[Projectile.owner].HeldItem.useTime;
            Spawn();
            StartupTime *= Projectile.MaxUpdates;
            CooldownTime *= Projectile.MaxUpdates;
            swingTime *= Projectile.MaxUpdates;
            if (UseAttackSpeed)
            {
                var speed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
                float cap = 5f;
                if (speed > cap)
                    speed = cap;

                if (speed != 0f)
                    speed = 1f / speed;

                swingTime = (int)(swingTime * speed);
                if (swingTime < 1)
                    swingTime = 1;
                StartupTime = (int)(StartupTime * speed);
                CooldownTime = (int)(CooldownTime * speed);
            }
            if (UseMeleeSize)
                Projectile.scale *= player.GetMeleeScale();
            baseScale = Projectile.scale;
            ExistsTime = swingTime + StartupTime + CooldownTime;
            Projectile.timeLeft = ExistsTime * 2;
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// DO NOT OVERRIDE IN MOST SITUATIONS
        /// use AdditionalAI() instead
        /// That will run AI code at the right time.
        /// </summary>
        public override void AI()
        {
            if (!hasFakedOnSpawn)
            {
                FakeOnSpawn();
                hasFakedOnSpawn = true;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 2)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }
            //else
            //{
            //    Projectile.Center += Projectile.velocity * 20;
            //    return;
            //}
            var player = Main.player[Projectile.owner];
            Projectile.gfxOffY = player.gfxOffY;
            player.MogMod().mouseWorldListener = true;
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            Vector2 position = ProjectilePosition != Vector2.Zero ? ProjectilePosition : player.MountedCenter;
            float adust = MathHelper.ToRadians(225);
            if (timer < StartupTime || timer > StartupTime + swingTime)
            {
                if (inStartup)
                    angle = Vector2.Lerp(angle, (position - mousePos).SafeNormalize(Vector2.One), RotateInStartup);
                if (inCooldown)
                    angle = Vector2.Lerp(angle, (position - mousePos).SafeNormalize(Vector2.One), RotateInCooldown);
                if (angle.X < 0)
                {
                    if (ProjectilePosition == Vector2.Zero)
                    {
                        player.direction = 1;
                        Projectile.spriteDirection = 1 * (int)player.gravDir;
                    }
                    else
                        Projectile.spriteDirection = 1;
                }
                else
                {
                    if (ProjectilePosition == Vector2.Zero)
                    {
                        player.direction = -1;
                        Projectile.spriteDirection = -1 * (int)player.gravDir;
                    }
                    else
                        Projectile.spriteDirection = -1;
                }
                if (AlternateSwings && player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 1)
                    Projectile.spriteDirection *= -1;
            }
            if (Projectile.spriteDirection == -1)
                adust = MathHelper.ToRadians(-45);
            var armCenter = position - new Vector2(5 * player.direction, 2);
            if (AfterImageLength > 0)
            {
                oldProjectileRot.Add(Projectile.rotation);
                oldProjectilePos.Add(Projectile.Center + new Vector2(0, Projectile.gfxOffY));
                if (oldProjectileRot.Count > AfterImageLength)
                {
                    oldProjectileRot.RemoveAt(0);
                    oldProjectilePos.RemoveAt(0);
                }
            }
            if (inSwing && swingTimer == 1 && UseSound != null)
                SoundEngine.PlaySound((SoundStyle)UseSound, player.Center);
            var angle2 = (AlternateSwings && modplayer.swingNum % 2 == 1 ? SwingFunction() : SwingFunction());
            Projectile.Center = armCenter - (angle * OffsetDistance * (1 + (Projectile.scale - 1) * 0.75f)).RotatedBy(Projectile.spriteDirection * angle2);
            Projectile.rotation = angle.RotatedBy(Projectile.spriteDirection * angle2).ToRotation() + adust;
            AdditionalAI();
            if (!Projectile.active)
                return;
            oldPlayerOffset = Projectile.Center - position;
            player.itemTime = ExistsTime + 2 - timer;
            player.itemAnimation = ExistsTime + 2 - timer;
            if (timer > ExistsTime)
            {
                player.itemTime = 0;
                player.itemAnimation = 0;
                Projectile.Kill();
            }
            timer++;
            if (timer >= StartupTime && timer < StartupTime + swingTime)
                swingTimer++;
            var armDir = armCenter - Projectile.Center;
            armDir.Y *= player.gravDir;
            if (ProjectilePosition == Vector2.Zero)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armDir.ToRotation() + MathHelper.ToRadians(90));
            oldScale.Insert(0, Projectile.scale);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (AfterImageLength > 0)
            {
                for (int i = 0; i < oldProjectileRot.Count; i++)
                {
                    var col = Projectile.Opacity * (i / (float)AfterImageLength) * 0.1f;
                    if (Projectile.spriteDirection == 1)
                    {
                        Main.EntitySpriteDraw(texture, oldProjectilePos[i] - Main.screenPosition, null, AfterImageColor * col, oldProjectileRot[i], texture.Size() / 2, oldScale[i], SpriteEffects.None, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(texture, oldProjectilePos[i] - Main.screenPosition, null, AfterImageColor * col, oldProjectileRot[i], texture.Size() / 2, oldScale[i], SpriteEffects.FlipHorizontally, 0);
                    }
                }
            }
            if (ProjectilePosition == Vector2.Zero) Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            var center = hitbox.Center.ToVector2();
            hitbox.Height = (int)(Projectile.height * Projectile.scale);
            hitbox.Width = (int)(Projectile.width * Projectile.scale);
            hitbox.Location = (center - new Vector2(hitbox.Width / 2, hitbox.Height / 2)).ToPoint();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (lineCollisionLength > 0)
            {
                var player = Main.player[Projectile.owner];
                var armcenter = ProjectilePosition != Vector2.Zero ? ProjectilePosition - new Vector2(5 * player.direction, 2) : player.MountedCenter - new Vector2(5 * player.direction, 2);
                var swordDir = armcenter.DirectionTo(Projectile.Center);
                var collisionline = new Vector2(lineCollisionLength / 2f, 0).RotatedBy(swordDir.ToRotation()) * Projectile.scale;
                bool c = Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + collisionline);
                if (c && !float.IsNaN(collisionline.X) && !float.IsNaN(collisionline.Y))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //modifiers.HitDirectionOverride = ((Main.player[Projectile.owner].DirectionTo(target.Center)).X >= 0 ? 1 : -1);
            if (ProjectilePosition != Vector2.Zero) modifiers.HitDirectionOverride = target.position.X > ProjectilePosition.X ? 1 : -1;
            else modifiers.HitDirectionOverride = target.position.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
            float damageMult = Utils.Remap(Projectile.numHits, 0, DamageHitCap, 1, DamageMin, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool? CanDamage() => inSwing;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(ProjectilePosition);
            writer.WriteVector2(mousePos);
            writer.Write(Projectile.rotation);
            writer.WriteVector2(angle);
            writer.Write((sbyte)Projectile.spriteDirection);
            //writer.Write(swingTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ProjectilePosition = reader.ReadVector2();
            Vector2 buffer;
            buffer = reader.ReadVector2();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }

            Projectile.rotation = reader.ReadSingle();
            angle = reader.ReadVector2();
            Projectile.spriteDirection = reader.ReadSByte();
            //swingTimer = reader.ReadInt32();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Spawns projectiles from the sword swing, automatically spacing them out on the if an amount is set.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="velocity">Velocity of the projectile. Automatically converted into a Vector2 in the direction of swing.</param>
        /// <param name="damagemod">The modifier from the source projectile's damage for this projectile</param>
        /// <param name="amount">The amount of projectiles for the sword to shoot. If set, it will space out those projectiles evenly. If unset, will force a shot.</param>
        /// <param name="negate"></param>
        /// <returns></returns>
        public void ShootCheck(int type = 0, float velocity = 1, float damagemod = 1, int amount = 0, int negate = 0, int ai0 = 0)
        {
            if (negate == 0)
            {
                negate = Projectile.spriteDirection;
            }
            if (amount == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation + negate * MathHelper.ToRadians(45 - negate * 90)).ToRotationVector2() * velocity, type, (int)(Projectile.damage * damagemod), Projectile.knockBack, Projectile.owner, ai0);
                return;
            }
            amount += 1;
            if (swingTimer % (swingTime / amount) == 0 && swingTimer > 0 && swingTimer < swingTime - swingTime / amount / 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation + negate * MathHelper.ToRadians(45 - negate * 90)).ToRotationVector2() * velocity, type, (int)(Projectile.damage * damagemod), Projectile.knockBack, Projectile.owner, ai0);
            }
        }
        #endregion
    }
}