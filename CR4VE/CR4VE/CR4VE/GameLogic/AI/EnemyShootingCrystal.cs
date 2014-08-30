using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.AI
{
    class EnemyShootingCrystal : Enemy
    {
        #region Attributes
        public static List<Entity> laserList = new List<Entity>();
        Random random = new Random();
        float moveSpeed = -0.5f;
        float rotationY = MathHelper.ToRadians(-90);
        float spawn = 0;
        Vector3 viewingDirection = new Vector3(-1, 0, 0);
        #endregion

        #region Properties
        public override bool isDead
        {
            get { return this.hp <= 0; }
        }
        public override float Health
        {
            get { return this.hp; }
            set { this.hp = value; }
        }
        #endregion

        #region inherited Constructors
        public EnemyShootingCrystal() : base() { }
        public EnemyShootingCrystal(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public EnemyShootingCrystal(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion


        public override void UpdateSingleplayer(GameTime gameTime)
        {
        }

        public override void Draw()
        {
            this.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, rotationY , 0);
        }
    }
}
