using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Ammo;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.MeleeProjectiles;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.NPCs.Global
{
    public class MogModGlobalNPC : GlobalNPC
    {
        // make skadi, aghs and wings do more npc effects somehow;

        // debuffs ID
        public int divineDebuff = 0;
        public int skadiDebuff = 0;
        public int freezingDebuff = 0;
        public int aghDebuff = 0;
        public int wingsOfLightDebuff = 0;
        public int ghostflameDebuff = 0;

        public NPC.HitInfo hitInfo;
        public int maxBlood = 1000;
        public int currentBlood = 0;
        public int blackBladeDebuff = 0;

        public int bashCap = 30;
        public int shivCap = 400;

        public bool markedByMarker;

        public bool bashProc = false;
        public bool shivProc = false;

        Random rand = new Random();

        // apparently neccessary according to calamity
        public override bool InstancePerEntity => true;

        public static readonly SoundStyle BloodCrit = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/BloodCrit")
        {
            Volume = .7f,
            PitchVariance = .2f,
        };

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            MogModGlobalNPC myClone = (MogModGlobalNPC)base.Clone(npc, npcClone);
            myClone.divineDebuff = divineDebuff;
            myClone.skadiDebuff = skadiDebuff;
            myClone.freezingDebuff = freezingDebuff;
            myClone.aghDebuff = aghDebuff;
            myClone.wingsOfLightDebuff = wingsOfLightDebuff;
            myClone.ghostflameDebuff = ghostflameDebuff;
            return myClone;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            maxBlood = Convert.ToInt32(npc.lifeMax * .05 + npc.defense); //(This scaling will definitely change as I test)
            if (maxBlood < 150) //Sets lower bound of possible max blood
            {
                maxBlood = 150;
            }
            MogGlobalItem globalItem = item.GetGlobalItem<MogGlobalItem>();
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                
            if (mogPlayer.exultationEquipped)
            {
                currentBlood += globalItem.bloodDamage + Convert.ToInt32(globalItem.bloodDamage * .15f);
            } else
            {
                currentBlood += globalItem.bloodDamage;
            }
                
            doBleedProc(npc);

            if (item.type == ModContent.ItemType<TheMarker>())
            {
            spawnMarkerProjectile(npc, player, item);
            }

            int itemDamage = player.HeldItem.damage;
            int enemyMaxHP = npc.lifeMax;
            if (itemDamage <= 30)
                bashCap = itemDamage;
            else
                bashCap = 50;
            if (Convert.ToInt32(enemyMaxHP * 0.01) <= 400)
                shivCap = Convert.ToInt32(enemyMaxHP * 0.01) + 20;
            else
                shivCap = 400;

            // skull basher
            var source = player.GetSource_OnHit(npc);
            bashProc = rand.Next(7) == 0;
            if (bashProc && mogPlayer.wearingGiantsMaul)
            {
                int bash = Projectile.NewProjectile(source, npc.Center, new Vector2(10f, 10f), ModContent.ProjectileType<SkullBashProjectile>(), bashCap, 0f, player.whoAmI);
                Rectangle r = new Rectangle((int)npc.position.X, (int)npc.position.Y - 50, npc.width, npc.height);
                Color textColor = new Color(255, 0, 100);
                CombatText.NewText(r, textColor, "Bash!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.BashProcTextSync);
                    packet.Write(npc.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
            }

            // serrated shiv
            shivProc = rand.Next(5) == 0;
            if (shivProc && mogPlayer.wearingSerratedShiv)
            {
                hitInfo = new NPC.HitInfo
                {
                    Damage = shivCap,
                    Knockback = 0,
                    HitDirection = 0,
                    Crit = false,
                    DamageType = DamageClass.Default
                };
                npc.StrikeNPC(hitInfo);
                NetMessage.SendStrikeNPC(npc, hitInfo);
                Rectangle r = new Rectangle((int)npc.position.X, (int)npc.position.Y - 50, npc.width, npc.height);
                Color textColor = new Color(210, 180, 140);
                CombatText.NewText(r, textColor, "True Strike!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.TrueStrikeProcTextSync);
                    packet.Write(npc.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
                doTrueStrikeFX(npc.Center);
            }

            // powder that decreases enemy armor from summoner attacks

        }

        public void spawnMarkerProjectile(NPC target, Player player, Item item)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (target.type != NPCID.TargetDummy)
            {
                if (!mogPlayer.markerProjOut)
                {
                    Vector2 velocity = new Vector2(20f, 20f);
                    Vector2 rotatedVelocity = velocity.RotateRandom(MathHelper.ToRadians(360));
                    rotatedVelocity.Normalize();
                    rotatedVelocity *= 10f;
                    int proj = Projectile.NewProjectile(target.GetSource_FromAI(), target.Center, rotatedVelocity, ModContent.ProjectileType<MarkerTargetProj>(), Convert.ToInt32(item.damage * 1.75), 0f, player.whoAmI);
                    mogPlayer.markerProjOut = true;
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.TryGetGlobalNPC<MogModGlobalNPC>(out var g))
                            g.markedByMarker = false;
                    }

                    markedByMarker = true;
                }
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            maxBlood = Convert.ToInt32(npc.lifeMax * .05 + npc.defense);
            if (maxBlood < 150)
            {
                maxBlood = 150;
            }

            MogModGlobalProjectileBleed globalProjectile = projectile.GetGlobalProjectile<MogModGlobalProjectileBleed>();

            if (Main.netMode == NetmodeID.MultiplayerClient) //All this stuff is so lord of blood's exultation works, and works in multiplayer
            {
                if (projectile.owner != 255)
                {
                    var ply = projectile.owner;
                    Player player = Main.player[ply];
                    MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();

                    if (mogPlayer.exultationEquipped)
                    {
                        currentBlood += globalProjectile.bloodDamage + Convert.ToInt32(globalProjectile.bloodDamage * .15f);
                    }
                    else
                    {
                        currentBlood += globalProjectile.bloodDamage;
                    }
                }
            } 
            else
            {
                Player player = Main.LocalPlayer;
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();

                if (mogPlayer.exultationEquipped)
                {
                    currentBlood += globalProjectile.bloodDamage + Convert.ToInt32(globalProjectile.bloodDamage * .15f);
                }
                else
                {
                    currentBlood += globalProjectile.bloodDamage;
                }
            }
            doBleedProc(npc);
        }
        public void doBleedProc(NPC npc)
        {
            if (currentBlood >= maxBlood){
                hitInfo = new NPC.HitInfo
                {
                    Damage = Convert.ToInt32(npc.lifeMax * .085) + 50,
                    Knockback = 0,
                    HitDirection = 0,
                    Crit = false,
                    DamageType = DamageClass.Generic
                };
                npc.StrikeNPC(hitInfo);
                NetMessage.SendStrikeNPC(npc, hitInfo);
                currentBlood = 0;
                Rectangle r = new Rectangle((int)npc.position.X, (int)npc.position.Y - 50, npc.width, npc.height);
                Color textColor = new Color(255, 0, 0);
                CombatText.NewText(r, textColor, "Bleed!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.BleedProcTextSync);
                    packet.Write(npc.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
                doBloodFX(npc.Center);
            }
        }
        public static void doBloodFX(Vector2 position)
        {
            SoundEngine.PlaySound(BloodCrit, position);
            for (int i = 0; i < 80; i++)
            {
                int blood = Dust.NewDust(position, 20, 20, DustID.Blood, 0, 0, 0, default, 2f);
                Main.dust[blood].noGravity = false;
            }
        }
        public static void doTrueStrikeFX(Vector2 position)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath56, position);
            for (int i = 0; i < 40; i++)
            {
                int strike = Dust.NewDust(position, 20, 20, DustID.CopperCoin, 0, 0, 100, default, 2f);
                Main.dust[strike].velocity.Y *= 1.05f;
                Main.dust[strike].noGravity = true;
            }
        }

        // actual debuff effect
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (divineDebuff > 0)
            {
                ApplyDPSDebuff(600, 100, ref npc.lifeRegen, ref damage);
            }
            if (skadiDebuff > 0)
            {
                ApplyDPSDebuff(200, 40, ref npc.lifeRegen, ref damage);
            }
            if (aghDebuff > 0)
            {
                ApplyDPSDebuff(480, 80, ref npc.lifeRegen, ref damage);
            }
            if (wingsOfLightDebuff > 0)
            {
                ApplyDPSDebuff(200, 10, ref npc.lifeRegen, ref damage);
            }
            if (blackBladeDebuff > 0)
            {
                ApplyDPSDebuff(200, 20, ref npc.lifeRegen, ref damage);
            }
            if (ghostflameDebuff > 0)
            {
                ApplyDPSDebuff(170, 7, ref npc.lifeRegen, ref damage);
            }
        }

        // not quite sure what this does, but its in calamity mod so it has to be important
        public override void PostAI(NPC npc)
        {
            if (divineDebuff > 0)
                divineDebuff--;
            if (skadiDebuff > 0)
            {
                skadiDebuff--;
                npc.velocity *= 0.988f;
            }
            if (freezingDebuff > 0)
            {
                freezingDebuff--;
                npc.velocity *= 0.985f;
            }
            if (aghDebuff > 0)
            {
                aghDebuff--;
            }
            if (wingsOfLightDebuff > 0)
            {
                wingsOfLightDebuff--;
            }
            if (blackBladeDebuff > 0) 
            { 
                blackBladeDebuff--;
            }
            if (ghostflameDebuff > 0)
            {
                ghostflameDebuff--;
            }
        }

        // LEDX and REDX chance to drop
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new CommonDrop(ModContent.ItemType<LedX>(), 10000, 1, 1, 1));
            globalLoot.Add(new CommonDrop(ModContent.ItemType<RedX>(), 100000, 1, 1, 1));
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Tim)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlintstoneArc>(), 3, 1, 1));
            }
            if (npc.type == NPCID.RuneWizard)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlintstoneArc>(), 1, 1, 1));
            }
            if (npc.type == NPCID.CrimsonAxe)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExplosiveGhostflame>(), 15, 1, 1));
            }
            if (npc.type == NPCID.CursedHammer)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExplosiveGhostflame>(), 15, 1, 1));
            }
            if (npc.type == NPCID.Golem)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LizhardBloodVial>(), 1, 1, 2));
            }
            if (npc.type == NPCID.Shark)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 10, 1, 1));
            }
            if (npc.type == NPCID.Squid)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 10, 1, 1));
            }
            if (npc.type == NPCID.BlueJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 10, 1, 1));
            }
            if (npc.type == NPCID.GreenJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 10, 1, 1));
            }
            if (npc.type == NPCID.PinkJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 10, 1, 1));
            }
            if (npc.type == NPCID.Crab)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 10, 1, 1));
            }
            if (npc.type == NPCID.DarkCaster)
            {
                npcLoot.Add(npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlinkDagger>(), 10, 1, 1)));
            }
        }

        // modifies vanilla npc shop
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.SkeletonMerchant)
                shop.Add(new Item(ModContent.ItemType<AstrologersStaff>()));
        }
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                hurtInfo = new Player.HurtInfo
                {
                    Damage = 1,
                    Knockback = 0,
                    HitDirection = 0,
                    Dodgeable = false,
                    SoundDisabled = true
                };

                var hitInfo = new NPC.HitInfo
                {
                    Damage = 20,
                    Knockback = 5,
                    HitDirection = target.direction,
                    Crit = false,
                    DamageType = DamageClass.Generic
                };
                npc.StrikeNPC(hitInfo); //Must use this instead of modifying the npc's life stat
                NetMessage.SendStrikeNPC(npc, hitInfo);
            }
        }

        // debuff visual effects
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            drawColor = npc.GetNPCColorTintedByBuffs(drawColor);
            if (divineDebuff > 0)
            {
                DivineMightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.NavajoWhite;
            }
            if (skadiDebuff > 0)
            {
                EyeOfSkadiDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.DarkSlateBlue;
            }
            if (freezingDebuff > 0)
            {
                FreezingDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.LightBlue;
            }
            if (aghDebuff > 0)
            {
                AghanimHexDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.BlueViolet;
            }
            if (wingsOfLightDebuff > 0)
            {
                WingsOfLightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.LightGoldenrodYellow;
            }
            if (blackBladeDebuff > 0)
            {
                BlackBladeDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.DarkRed;
            }
            if (ghostflameDebuff > 0)
            {
                GhostflameDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.WhiteSmoke;
            }
            if (markedByMarker) //TODO: Give this a custom effect
            {
                WingsOfLightDebuff.DrawEffects(npc, ref drawColor);
                drawColor = Color.Gold;
            }
        }

        // QOL for making debuff damage easier
        public void ApplyDPSDebuff(int lifeRegenValue, int damageValue, ref int lifeRegen, ref int damage)
        {
            if (lifeRegen > 0)
                lifeRegen = 0;

            lifeRegen -= lifeRegenValue;

            if (damage < damageValue)
                damage = damageValue;
        }
        public static void DrawAfterimage(NPC npc, SpriteBatch spriteBatch, Color startingColor, Color endingColor, Texture2D texture = null, Func<NPC, int, float> rotationCalculation = null, bool directioning = false, bool invertedDirection = false)
        {
            if (NPCID.Sets.TrailingMode[npc.type] != 1)
                return;

            SpriteEffects spriteEffects = SpriteEffects.None;

            if (npc.spriteDirection == -1 && directioning)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (invertedDirection)
                spriteEffects ^= SpriteEffects.FlipHorizontally; // Same as x XOR 1, or x XOR TRUE, which inverts the bit. In this case, this reverses the horizontal flip

            // Set the rotation calculation to a predefined value. The null default is solely so that
            if (rotationCalculation is null)
                rotationCalculation = (nPC, afterimageIndex) => nPC.rotation;

            endingColor.A = 0;

            Color drawColor = npc.GetAlpha(startingColor);
            Texture2D npcTexture = texture ?? TextureAssets.Npc[npc.type].Value;
            Vector2 screenOffset = npc.IsABestiaryIconDummy ? Vector2.Zero : Main.screenPosition;
            int afterimageCounter = 1;
            while (afterimageCounter < NPCID.Sets.TrailCacheLength[npc.type] && MogClientConfig.Instance.Afterimages)
            {
                Color colorToDraw = Color.Lerp(drawColor, endingColor, afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type]);
                colorToDraw *= afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type];
                spriteBatch.Draw(npcTexture,
                                 npc.oldPos[afterimageCounter] + npc.Size / 2f - screenOffset + Vector2.UnitY * npc.gfxOffY,
                                 npc.frame,
                                 colorToDraw,
                                 rotationCalculation.Invoke(npc, afterimageCounter),
                                 npc.frame.Size() * 0.5f,
                                 npc.scale,
                                 spriteEffects,
                                 0f);
                afterimageCounter++;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                MogPlayer mogPlayer = target.GetModPlayer<MogPlayer>();
                mogPlayer.doParry(target, target.Center);
                modifiers.Cancel();

                var hitInfo = new NPC.HitInfo
                {
                    Damage = 20,
                    Knockback = 5,
                    HitDirection = target.direction,
                    Crit = false,
                    DamageType = DamageClass.Generic
                };
                npc.StrikeNPC(hitInfo); //Must use this instead of modifying the npc's life stat
                NetMessage.SendStrikeNPC(npc, hitInfo);
            }
        }
    }
}
