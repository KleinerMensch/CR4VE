using CR4VE.GameBase.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class Enemy : Entity, AIInterface
    {
        //alle Gegner haben 3Leben -> wird vererbt
        public float health = 3;

        #region inherited Constructors
        public Enemy():base() { }
        public Enemy(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public Enemy(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm) { }
        #endregion

        public virtual void UpdateSingleplayer(Microsoft.Xna.Framework.GameTime gameTime) { }
        public virtual void UpdateArena(Microsoft.Xna.Framework.GameTime gameTime) { }
        public virtual void Draw(Microsoft.Xna.Framework.GameTime gameTime) { }
        public virtual void DrawInArena(Microsoft.Xna.Framework.GameTime gameTime) { }
        public virtual void Destroy() { }
    }
}
