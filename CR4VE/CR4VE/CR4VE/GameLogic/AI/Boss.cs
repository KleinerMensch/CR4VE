using CR4VE.GameLogic.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class Boss : Character
    {
        #region inherited Constructors
        //base ist fuer Vererbungskram
        public Boss() : base() { }
        public Boss(Vector3 pos, String modelName, ContentManager cm): base(pos,modelName,cm){ }
        public Boss(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound): base(pos,modelName, cm, bound){ }
        #endregion
    }
}
