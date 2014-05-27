using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;


namespace CR4VE.GameBase.Objects
{
    public class Entity
    {
        #region Attributes
        private Vector3 position;
        private Model model;
        private BoundingBox boundary;
        #endregion

        #region Constructors
        //Konstruktor laedt das Modell anhand eines Strings aus dem Model-Verzeichnis
        public Entity(Vector3 pos, String modelName, ContentManager cm)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/" + modelName);
        }
        public Entity(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound)
        {
            this.position = pos;
            this.model = cm.Load<Model>("Assets/Models/" + modelName);
            this.boundary = bound;
        }
        #endregion

        #region Properties
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Model Model
        {
            get { return this.model; }
            set { this.model = value; }
        }

        public BoundingBox Boundary
        {
            get { return this.boundary; }
            set { this.boundary = value; }
        }
        #endregion

        #region Methods
        public void move(Vector2 offset)
        {
            this.Position += new Vector3(offset, 0);
        }
        public void move(Vector3 offset)
        {
            this.Position += offset;
        }

        //zeichnet Objekt im Bezug auf den Viewport
        public void drawOn2DScreen(Vector3 scale)
        {
            Matrix view = Matrix.CreateLookAt(Camera2D.CamPosition3D, Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1, 10f, 1000);
            Matrix worldMatrix = Matrix.CreateScale(scale) * 1 * Matrix.CreateTranslation(this.position);

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = worldMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        //zeichnet Objekt in Bezug auf die Spielwelt
        public void drawIn2DWorld(Vector3 scale)
        { 
            Matrix view = Matrix.CreateLookAt(Camera2D.CamPosition3D, Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1, 10f, 1000);
            Matrix worldMatrix = Matrix.CreateScale(scale) * 1 * Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = worldMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        //zeichnet Objekt in Bezug auf die Arenakamera
        public void drawInArena(Vector3 scale)
        {
            Matrix view = Matrix.CreateLookAt(CameraArena.Position, Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1, 10f, 1000);
            Matrix worldMatrix = Matrix.CreateScale(scale) * 1 * Matrix.CreateTranslation(this.Position);

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = worldMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
        #endregion
    }
}
