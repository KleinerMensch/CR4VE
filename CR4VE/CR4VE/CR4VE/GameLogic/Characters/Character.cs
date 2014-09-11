using CR4VE.GameBase.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.Characters
{
    class Character : Entity
    {
        #region Attributes
        public List<Entity> attackList = new List<Entity>();
        public bool launchedMelee = false;
        public bool launchedRanged = false;
        public bool launchedSpecial = false;

        public static float manaLeft = 3;
        #endregion

        #region inherited Constructors
        //base ist fuer Vererbungskram
        public Character() : base() { }
        public Character(Vector3 pos, String modelName, ContentManager cm)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Players/" + modelName);
        }
        public Character(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Players/" + modelName);
            this.boundary = bound;
        }
        #endregion

        public virtual void Update(GameTime time) { }
        public virtual void MeleeAttack(GameTime time) { }
        public virtual void RangedAttack(GameTime time) { }
        public virtual void SpecialAttack(GameTime time) { }
        public virtual void DrawAttacks() { }
    }
}
