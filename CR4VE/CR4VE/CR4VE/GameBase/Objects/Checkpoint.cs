using CR4VE.GameBase.Camera;
using CR4VE.GameLogic;
using CR4VE.GameLogic.GameStates;
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
        private String type;

        //realign parameters
        private Vector2 saveCamPosition;
        private Matrix saveFrusMat;
        #endregion

        #region Properties
        public Vector2 savedCameraPosition2D
        { 
            get { return saveCamPosition; } 
        }
        public Matrix savedFrustumMatrix
        {
            get { return saveFrusMat; }
        }
        #endregion

        #region Constructor
        public Checkpoint(Vector3 pos, String modelName, ContentManager cm)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Checkpoints/" + modelName);
            this.boundary = new BoundingBox(pos + new Vector3(-2.5f, -2.5f, -2.5f), pos + new Vector3(2.5f, 2.5f, 2.5f));

            //type
            if (modelName == "checkpoint_hell")
                this.type = "hell";
            else
                this.type = "crystal";

            //realign parameters
            this.saveCamPosition = Camera2D.Position2D;
            this.saveFrusMat = Camera2D.BoundFrustum.Matrix;
        }
        #endregion

        #region Methods
        public void Update()
        {
            if (this.Boundary.Intersects(Singleplayer.player.Boundary))
            {
                Singleplayer.lastCheckpoint = this;

                //save realign parameters
                this.saveCamPosition = Camera2D.Position2D;
                this.saveFrusMat = Camera2D.BoundFrustum.Matrix;

                //update SaveGame.txt
                if (this.type == "hell")
                    SaveGame.setHellReset(this.Position);
                else
                    SaveGame.setCrystalReset(this.Position);
            }
        }
        #endregion
    }
}
