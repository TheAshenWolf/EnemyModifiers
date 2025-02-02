﻿using Microsoft.Xna.Framework;
using System;
using FargoEnemyModifiers.Utilities;
using Terraria;
using Terraria.ID;

namespace FargoEnemyModifiers.Modifiers
{
    public class Splitting : Modifier
    {
        public override ModifierID ModifierID => ModifierID.Splitting;
        public override string Key => "Splitting";
        public override RarityID Rarity => RarityID.Common;

        public override bool PreNPCLoot(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return false;

            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                int index = NPC.NewNPC(npc.GetSource_FromAI(), (int) npc.Center.X, (int) npc.Center.Y, npc.type);
                NPC baby = Main.npc[index];
                baby.lifeMax /= 2;
                baby.life = baby.lifeMax;
                baby.defense /= 2;
                baby.damage /= 2;
                baby.scale = .75f;

                baby.velocity = new Vector2(Main.rand.Next(-2, 3), -2);

                baby.GetGlobalNPC<EnemyModifiersGlobalNPC>().firstTick = false;

                foreach (ModifierID modifierType in npc.GetGlobalNPC<EnemyModifiersGlobalNPC>().modifierTypes)
                {
                    Modifier newMod = (Activator.CreateInstance(EnemyModifiers.Modifiers[modifierType].GetType()) as Modifier);

                    if (newMod.Key == "Splitting")
                    {
                        continue;
                    }

                    baby.GetGlobalNPC<EnemyModifiersGlobalNPC>().ApplyModifier(baby, modifierType);
                }

                baby.GetGlobalNPC<EnemyModifiersGlobalNPC>().finalizeModifierName(npc);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
            }

            return false;
        }
    }
}