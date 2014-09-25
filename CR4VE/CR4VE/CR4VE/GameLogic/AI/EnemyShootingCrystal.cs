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
        new Vector3 viewingDirection = new Vector3(-1, 0, 0);
        public bool checkedStartPositionOnce = false;
        public Vector3 startPosition;
        public float movingRange = 20;
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
            if (!checkedStartPositionOnce)
            {
                startPosition = this.Position;
                checkedStartPositionOnce = true;
            }

            spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.move(new Vector3(moveSpeed, 0, 0));
            if (this.position.X < startPosition.X - movingRange || this.position.X > startPosition.X + movingRange)
            {
                moveSpeed *= -1;
                viewingDirection.X *= -1;
                rotationY += MathHelper.ToRadians(180);
            }

            //updating laserList
            this.LoadLaser(this.position, Singleplayer.cont);

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }
        }

        public void LoadLaser(Vector3 Crystalposition, ContentManager content)
        {
            #region spawning laser
            //spawning laser every second
            //bisher nur einzelne Nadeln gespawnt
            if (spawn > 1)
            {
                spawn = 0;
                if (laserList.Count() < 10)
                    laserList.Add(new Entity(Crystalposition, "Enemies/ImaFirinMahLaserr", content));
                laserList.Last().viewingDirection = this.viewingDirection;
            }
            #endregion
            #region deleting laser
            for (int i = 0; i < laserList.Count; i++)
            {
                //laser verschwindet <=> bestimmte Entfernung von Startposition erreicht
                if (laserList[i].position.X >= this.position.X + 50 || laserList[i].position.X <= this.position.X - 50)
                {
                    laserList.RemoveAt(i);
                    i--;
                }
            }
            #endregion
            #region Collision with Laser
            for (int i = 0; i < laserList.Count; i++)
            {
                laserList[i].boundary = new BoundingBox(laserList[i].position + new Vector3(-2, -2, -2), laserList[i].position + new Vector3(2, 2, 2));
                laserList[i].position.X += laserList[i].viewingDirection.X;
                if (Singleplayer.player.boundary.Intersects(laserList[i].boundary))
                {
                    Singleplayer.hud.healthLeft -= 30;
                    laserList.Remove(laserList[i]);
                    i--;
                }
            }
            #endregion
        }

        public override void Draw()
        {
            this.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, rotationY , 0);
            foreach (Entity laser in laserList)
            {
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(-90) * laser.viewingDirection.X);
            }
        }

        public override void Destroy()
        {
            laserList.Clear();
        }
    }
}
