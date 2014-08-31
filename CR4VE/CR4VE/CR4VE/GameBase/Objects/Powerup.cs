using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.Objects
{
    public class Powerup : Entity
    {
        #region Attributes
        public String type;
        int amount;

        //Animationsparameter
        public readonly float offsetHeight = 0.03f;
        public float deltaHeight = 0f;
        public float rotatedDegree = 0f;

        bool upAnim = true;
        #endregion

        #region Constructor
        public Powerup(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound, String _type, int _amount)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Powerups/" + modelName);
            this.boundary = bound;

            this.type = _type;
            this.amount = _amount;
        }
        #endregion

        #region Methods
        public void Update()
        {
            //check powerup type to determine movement
            if (type == "health")
                rotatedDegree += 0.03f;
            else
            {
                //checking if animation is up- or downward
                if (deltaHeight <= 0f)
                    upAnim = true;
                if (deltaHeight >= 2f)
                    upAnim = false;

                //moving powerup accordingly
                if (upAnim)
                {
                    deltaHeight += offsetHeight;
                    this.move(new Vector3(0, offsetHeight, 0));
                }
                else
                {
                    deltaHeight -= offsetHeight;
                    this.move(new Vector3(0, -offsetHeight, 0));
                }
            }
        }

        public void Draw()
        {
            if (this.type == "health")
                this.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, this.rotatedDegree, 0);
            else
                this.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, MathHelper.ToRadians(-90), 0);
        }
        #endregion
    }
}
