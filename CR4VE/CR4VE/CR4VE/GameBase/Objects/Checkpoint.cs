using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.Objects
{
    class Checkpoint : Entity
    {
        #region Attributes
        //private 
        #endregion

        #region Constructor
        public Checkpoint(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Checkpoints/" + modelName);
            this.boundary = bound;
        }
        #endregion
    }
}
