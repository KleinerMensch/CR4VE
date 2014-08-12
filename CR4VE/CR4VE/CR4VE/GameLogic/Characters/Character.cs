using CR4VE.GameBase.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.Characters
{
    class Character : Entity
    {
        #region inherited Constructors
        //base ist fuer Vererbungskram
        public Character():base() { }
        public Character(Vector3 pos, String modelName, ContentManager cm):base(pos, modelName, cm) { }
        public Character(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound):base(pos, modelName, cm) { }
        #endregion

        public virtual void Update(GameTime time) { }
        public virtual void MeleeAttack(GameTime time) { }
        public virtual void RangedAttack(GameTime time) { }
        public virtual void SpecialAttack(GameTime time) { }
    }
}
