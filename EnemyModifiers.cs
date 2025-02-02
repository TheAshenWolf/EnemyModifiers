using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FargoEnemyModifiers.Modifiers;
using FargoEnemyModifiers.NetCode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoEnemyModifiers
{
    public class EnemyModifiers : Mod
    {
        public static Mod Instance;

        //alphabetical list for forcing specific one
        //public static List<Modifier> Modifiers;
        public static Dictionary<ModifierID, Modifier> Modifiers;


        public static TModifier GetModifier<TModifier>() where TModifier : Modifier =>
            (TModifier) Modifiers.FirstOrDefault(x => x.Value.GetType() == typeof(TModifier)).Value;

        public static Modifier GetModifier(Modifier modifier) =>
            Modifiers.FirstOrDefault(x => x.Value.GetType() == modifier.GetType()).Value;

        public override void PostSetupContent()
        {
            Instance = this;
            
            Modifiers = new Dictionary<ModifierID, Modifier>();

            foreach (Type type in this.Code.GetTypes().Where(x =>
                !x.IsAbstract && x.IsSubclassOf(typeof(Modifier)) && x.GetConstructor(new Type[0]) != null))
            {
                if (Activator.CreateInstance(type) is Modifier modifier && modifier.AutoLoad())
                {
                    Modifiers.Add(modifier.ModifierID, modifier);
                }
            }
        }

        public override void Unload()
        {
            Modifiers = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketID id = (PacketID)reader.ReadByte();

            switch (id)
            {
                // Packet:
                // byte: packet id
                // byte: npc whoAmI
                // byte: modifier amount
                // byte[]: modifiers
                case PacketID.MobSpawn: // Server is sending modifier data to clients
                    if (Main.netMode != NetmodeID.MultiplayerClient) return;


                    int npcIndex = reader.ReadByte();
                    NPC npc = Main.npc[npcIndex]; // npc whoAmI
                    if (!npc.active)
                    {
                        reader.ReadBytes(reader.ReadByte()); // skip modifiers
                        return; // npc is dead
                    }
                    EnemyModifiersGlobalNPC modNpc = npc.GetGlobalNPC<EnemyModifiersGlobalNPC>();
                    
                    int[] modifiers = new int[reader.ReadByte()]; // modifier amount
                    for (int i = 0; i < modifiers.Length; i++)
                    {
                        modifiers[i] = reader.ReadByte();
                    }
                    
                    foreach (ModifierID modifier in modifiers)
                    {
                        modNpc.modifierTypes.Add(modifier);
                        modNpc.ApplyModifier(npc, modifier);
                    }
                
                    modNpc.finalizeModifierName(npc);
                    
                    break;
                // Packet:
                // byte: packet id
                // byte: npc whoAmI
                case PacketID.ClientCausedDespawn:
                    if (Main.netMode != NetmodeID.Server) return;
                    
                    npcIndex = reader.ReadByte();
                    NPC npcToDespawn = Main.npc[npcIndex];
                    
                    npcToDespawn.active = false;
                    break;
            }
        }
    }
}