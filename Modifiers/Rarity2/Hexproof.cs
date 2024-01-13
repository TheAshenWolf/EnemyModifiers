﻿using FargoEnemyModifiers.Utilities;
using Terraria;

namespace FargoEnemyModifiers.Modifiers
{
    public class HexProof : Modifier
    {
        public override string Key => "Hex-proof";
        public override RarityID Rarity => RarityID.Uncommon;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.lifeRegen >= 0)
                return;

            npc.lifeRegen = 0;
            damage = 0;
        }
    }
}