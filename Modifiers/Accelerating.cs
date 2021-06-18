﻿using Terraria;

namespace FargoEnemyModifiers.Modifiers
{
    public class Accelerating : Modifier
    {
        public Accelerating()
        {
            name = "Accelerating";
        }

        private int counter;
        public override bool PreAI(NPC npc)
        {
            if (++counter > 30 && SpeedMultiplier < 4f)
            {
                SpeedMultiplier *= 1.05f;
                counter = 0;
            }

            return true;
        }

        public override void OnHitByItem(NPC npc, Player player)
        {
            SpeedMultiplier = 0.5f;
        }

        public override void OnHitPlayer(NPC npc, Player target)
        {
            SpeedMultiplier = 0.5f;
        }
    }
}
