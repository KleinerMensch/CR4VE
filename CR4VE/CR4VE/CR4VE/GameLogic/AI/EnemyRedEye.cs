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
    class EnemyRedEye : Enemy
    {
        #region Attributes
        public static List<Entity> laserList = new List<Entity>();
        Random random = new Random();
        float moveSpeed = -0.5f;
        float rotationY = MathHelper.ToRadians(-90);
        float spawn = 0;
        public new Vector3 viewingDirection = new Vector3(-1, 0, 0);
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
        public EnemyRedEye() : base() { }
        public EnemyRedEye(Vector3 pos, String modelName, ContentManager cm) : base(pos, modelName, cm) { }
        public EnemyRedEye(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound) : base(pos, modelName, cm, bound) { }
        #endregion

        public override void UpdateSingleplayer(GameTime gameTime)
        {
            spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.move(new Vector3(moveSpeed, 0, 0));
            if (this.position.X < 50 || this.position.X > 105)
            {
                moveSpeed *= -1;
                viewingDirection.X *= -1;
                rotationY += MathHelper.ToRadians(180);
            }

            //updating laserList
            this.LoadEnemies(this.position, Singleplayer.cont);

            if (Singleplayer.player.boundary.Intersects(this.boundary))
            {
                Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
            }

            #region Collision with Laser
            foreach (Entity laser in laserList)
            {
                laser.boundary = new BoundingBox(laser.position + new Vector3(-2, -2, -2), laser.position + new Vector3(2, 2, 2));
                laser.position.X += laser.viewingDirection.X;
                if (Singleplayer.player.boundary.Intersects(laser.boundary))
                {
                    Singleplayer.hud.healthLeft -= (int)(Singleplayer.hud.fullHealth * 0.01);
                }
            }
            #endregion
        }

        public override void UpdateArena(GameTime gameTime)
        {
            spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.move(new Vector3(moveSpeed, 0, 0));
            if (this.position.X < 50 || this.position.X > 105)
            {
                moveSpeed *= -1;
                viewingDirection.X *= -1;
                rotationY += MathHelper.ToRadians(180);
            }

            //updating laserList
            this.LoadEnemies(this.position, Arena.cont);

  
            if (Arena.player.boundary.Intersects(this.boundary))
            {
                Arena.opheliaHud.healthLeft -= (int)(Arena.opheliaHud.fullHealth * 0.01);
            }

            #region Collision with Laser
            foreach (Entity laser in laserList)
            {
                laser.boundary = new BoundingBox(laser.position + new Vector3(-2, -2, -2), laser.position + new Vector3(2, 2, 2));
                laser.position.X += laser.viewingDirection.X;
                if (Arena.player.boundary.Intersects(laser.boundary))
                {
                    Arena.opheliaHud.healthLeft -= (int)(Arena.opheliaHud.fullHealth * 0.01);
                }
            }
            #endregion
        }

        #region Laser vom Auge
        //bisher nur einzelne Nadeln gespawnt
        public void LoadEnemies(Vector3 EyePosition, ContentManager content)
        {
            // spawning laser every second
            if (spawn > 1)
            {
                spawn = 0;
                if (laserList.Count() < 10)
                    laserList.Add(new Entity(EyePosition, "Enemies/ImaFirinMahLaserr", content));
                laserList.Last().viewingDirection = this.viewingDirection;
            }
            for (int i = 0; i < laserList.Count; i++)
            {
                // wenn laser zu weit weg, dann verschwindet er
                // 'laser' verschwindet nocht nicht, wenn er den Spieler beruehrt
                if (laserList[i].position.X >= this.position.X + 50 || laserList[i].position.X <= this.position.X - 50)
                {
                    laserList.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion

        public override void Draw()
        {
            this.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, rotationY, 0);

            foreach (Entity laser in EnemyRedEye.laserList)
            {       
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(-90) * laser.viewingDirection.X);
            }
        }

        public override void DrawInArena()
        {
            this.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, rotationY, 0);
        }

        public override void Destroy()
        {
            laserList.RemoveRange(0, laserList.Count);
        }
    }
}
