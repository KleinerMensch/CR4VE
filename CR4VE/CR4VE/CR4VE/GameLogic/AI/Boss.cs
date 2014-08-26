using CR4VE.GameLogic.Characters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class Boss : Character
    {
        public void Irgendwas(GameTime time)
        {

            this.RangedAttack(time);
        }
    }
}
