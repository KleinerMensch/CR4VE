using CR4VE.GameBase.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class Enemy : Entity, AIInterface
    {
        #region Attributes
        //alle Gegner haben 3Leben -> wird vererbt
        public float hp = 3;
        #endregion

        #region Properties
        public virtual bool isDead { get; set; }
        public virtual float Health { get; set; }
        #endregion

        #region Constructors
        public Enemy() : base() { }
        public Enemy(Vector3 pos, String modelName, ContentManager cm)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Enemies/" + modelName);
        }
        public Enemy(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Enemies/" + modelName);
            this.boundary = bound;
        }
        #endregion

        public virtual void UpdateSingleplayer(GameTime gameTime) { }
        public virtual void UpdateArena(GameTime gameTime) { }
        public virtual void Draw() { }
        public virtual void DrawInArena() { }
        public virtual void Destroy() { }
    }
}
