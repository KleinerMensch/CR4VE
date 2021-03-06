﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.Objects.Terrain
{
    public class GroundSpikes : Tile
    {
        #region Attributes
        
        #endregion

        #region Properties
        public String Type
        {
            get { return type; }
        }
        #endregion

        #region Constructor
        public GroundSpikes(String spikeType, Vector3 pos, BoundingBox bound, int damage)
        {
            this.position = pos;
            this.model = Content.Load<Model>("Assets/Models/Terrain/spikes_" + spikeType);
            this.boundary = bound;
            this.dmg = damage;
            this.type = spikeType;
        }
        #endregion

        #region Methods
        public override void Draw()
        {
            float rotX = 0;

            switch (this.Type)
            {
                case "ground":
                    rotX = MathHelper.ToRadians(-90);
                    break;
            }
            
            this.drawIn2DWorldWithoutBones(new Vector3(1.1f, 1.1f, 1.1f), rotX, 0, 0);
        }
        #endregion
    }
}
