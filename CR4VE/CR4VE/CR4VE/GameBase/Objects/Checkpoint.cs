using CR4VE.GameBase.Camera;
using CR4VE.GameLogic;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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

        private bool soundPlayed = false;
        private bool soundPlayedCrystal = false;
        #endregion

        #region Constructor
        public Checkpoint(Vector3 pos, String modelName, ContentManager cm)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/Checkpoints/" + modelName);
            this.boundary = new BoundingBox(pos + new Vector3(-2.5f, -2.5f, -2.5f), pos + new Vector3(2.5f, 7.5f, 2.5f));

            //type
            if (modelName == "checkpoint_hell")
                this.type = "hell";
            else
                this.type = "crystal";
        }
        #endregion

        #region Methods
        public void Update()
        {
            if (this.Boundary.Intersects(Singleplayer.player.Boundary))
            {
                Singleplayer.lastCheckpoint = this;

                //update SaveGame.txt
                if (this.type == "hell")
                {
                    if (!soundPlayed)
                    {
                        Sounds.checkpointHell.Play();

                        soundPlayed = true;
                    }
                    SaveGame.setHellReset(this.Position);
                }
                else
                {
                    if (!soundPlayedCrystal)
                    {
                        Sounds.checkpointCrystal.Play();

                        soundPlayedCrystal = true;
                    }
                    SaveGame.setCrystalReset(this.Position);
                }

               
            }
        }

        public void Draw()
        {
            if (type == "hell")
                this.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);
            else
                this.drawIn2DWorld(new Vector3(3f, 3f, 3f), 0, MathHelper.ToRadians(-90), 0);
        }
        #endregion
    }
}
