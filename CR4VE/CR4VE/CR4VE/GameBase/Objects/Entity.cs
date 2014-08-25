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
using CR4VE.GameBase.Objects.Terrain;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.GameStates;

namespace CR4VE.GameBase.Objects
{
    public class Entity
    {
        #region Attributes
        public Vector3 position;
        public Model model;
        public BoundingBox boundary;

        //Blickrichtung fuer Angriffe
        public Vector3 viewingDirection = new Vector3(1, 0, 0);
        #endregion

        #region Constructors
        public Entity() { }

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

            //moving bounding box (Sidescroller)
            //(noch hart gecoded fuer sphereD5)
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                //Console.WriteLine(this.Boundary);
                this.boundary.Min = this.Position + new Vector3(-2.5f, -2.5f, -2.5f);
                this.boundary.Max = this.Position + new Vector3(2.5f, 2.5f, 2.5f);
            }
            if (Game1.currentState.Equals(Game1.EGameState.Arena))
            {
                this.boundary.Min = this.Position + new Vector3(-2.5f, -2.5f, -2.5f);
                this.boundary.Max = this.Position + new Vector3(2.5f, 2.5f, 2.5f);
            }
        }
        /*public void moveTo(Vector3 destination)
        {
            this.Position = destination;

            //moving bounding box (Sidescroller)
            //(noch hart gecoded fuer sphereD5)
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                this.boundary.Min = this.Position + new Vector3(-2.5f, -2.5f, -2.5f);
                this.boundary.Max = this.Position + new Vector3(2.5f, 2.5f, 2.5f);
            }
        }*/

        //checks if entity boundary intersects with terrain tile boundary top plane beneath it
        public bool checkFooting()
        {
            List<Tile> visibles = Tilemap.getVisibleTiles();

            List<Tile> possibleCollisions = new List<Tile>();

            foreach (Tile t in visibles)
            {
                Vector3[] boundCornTile = t.Boundary.GetCorners();
                Vector3[] boundCornEnt = this.Boundary.GetCorners();

                if (boundCornTile[0].Y <= boundCornEnt[2].Y && boundCornTile[1].X > boundCornEnt[0].X && boundCornTile[0].X < boundCornEnt[1].X)
                    possibleCollisions.Add(t);
            }

            if (possibleCollisions.Count != 0)
            {
                foreach (Tile t in possibleCollisions)
                {
                    Vector3[] boundCornTile = t.Boundary.GetCorners();

                    Plane topTilePlane = new Plane(boundCornTile[0], boundCornTile[1], boundCornTile[4]);

                    if (topTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Intersecting)
                    {
                        return true;
                    }
                }
            }           
            
            return false;
        }
        //checks if entity collides with terrain in specified direction and sets it back accordingly
        public bool handleTerrainCollisionInDirection(String direction, Vector3 moveVecEntity)
        {
            List<Tile> visibles = Tilemap.getVisibleTiles();

            switch (direction)
            {
                #region left
                case "left":
                    {
                        List<Tile> possibleCollisions = new List<Tile>();

                        foreach (Tile t in visibles)
                        {
                            Vector3[] boundCornTile = t.Boundary.GetCorners();
                            Vector3[] boundCornEnt = this.Boundary.GetCorners();

                            if (boundCornTile[1].X < boundCornEnt[0].X && boundCornTile[0].Y > boundCornEnt[2].Y && boundCornTile[2].Y < boundCornEnt[0].Y)
                                possibleCollisions.Add(t);
                        }

                        if (possibleCollisions.Count != 0)
                        {
                            List<Tile> collisions = new List<Tile>();

                            foreach (Tile t in possibleCollisions)
                            {
                                Vector3[] boundCornTile = t.Boundary.GetCorners();

                                Plane rightTilePlane = new Plane(boundCornTile[1], boundCornTile[2], boundCornTile[5]);

                                if (rightTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Front)
                                {
                                    BoundingBox newBound = new BoundingBox(this.Boundary.Min + moveVecEntity, this.Boundary.Max + moveVecEntity);

                                    if (rightTilePlane.Intersects(newBound) == PlaneIntersectionType.Intersecting)
                                    {
                                        collisions.Add(t);

                                        float deltaX = Math.Abs(t.Boundary.Max.X - this.Boundary.Min.X);

                                        this.move(new Vector3(-deltaX,0,0));
                                    }
                                }
                            }

                            if (collisions.Count != 0)
                                return true;
                            else
                                return false;
                        }
                    } break;
                #endregion

                #region right
                case "right":
                    {
                        List<Tile> possibleCollisions = new List<Tile>();

                        foreach (Tile t in visibles)
                        {
                            Vector3[] boundCornTile = t.Boundary.GetCorners();
                            Vector3[] boundCornEnt = this.Boundary.GetCorners();

                            if (boundCornTile[0].X > boundCornEnt[1].X && boundCornTile[0].Y > boundCornEnt[2].Y && boundCornTile[2].Y < boundCornEnt[0].Y)
                                possibleCollisions.Add(t);
                        }

                        if (possibleCollisions.Count != 0)
                        {
                            List<Tile> collisions = new List<Tile>();

                            foreach (Tile t in possibleCollisions)
                            {
                                Vector3[] boundCornTile = t.Boundary.GetCorners();

                                Plane leftTilePlane = new Plane(boundCornTile[0], boundCornTile[3], boundCornTile[4]);

                                if (leftTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Back)
                                {
                                    BoundingBox newBound = new BoundingBox(this.Boundary.Min + moveVecEntity, this.Boundary.Max + moveVecEntity);

                                    if (leftTilePlane.Intersects(newBound) == PlaneIntersectionType.Intersecting)
                                    {
                                        collisions.Add(t);

                                        float deltaX = Math.Abs(t.Boundary.Min.X - this.Boundary.Max.X);

                                        this.move(new Vector3(deltaX, 0, 0));
                                    }
                                }
                            }

                            if (collisions.Count != 0)
                                return true;
                            else
                                return false;
                        }
                    } break;
                #endregion

                #region up
                case "up":
                    {
                        List<Tile> possibleCollisions = new List<Tile>();

                        foreach (Tile t in visibles)
                        {
                            Vector3[] boundCornTile = t.Boundary.GetCorners();
                            Vector3[] boundCornEnt = this.Boundary.GetCorners();

                            if (boundCornTile[2].Y > boundCornEnt[0].Y && boundCornTile[1].X > boundCornEnt[0].X && boundCornTile[0].X < boundCornEnt[1].X)
                                possibleCollisions.Add(t);
                        }

                        if (possibleCollisions.Count != 0)
                        {
                            List<Tile> collisions = new List<Tile>();
                            
                            foreach (Tile t in possibleCollisions)
                            {
                                Vector3[] boundCornTile = t.Boundary.GetCorners();

                                Plane bottomTilePlane = new Plane(boundCornTile[3], boundCornTile[7], boundCornTile[2]);

                                if (bottomTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Front)
                                {
                                    BoundingBox newBound = new BoundingBox(this.Boundary.Min + moveVecEntity, this.Boundary.Max + moveVecEntity);

                                    if (bottomTilePlane.Intersects(newBound) == PlaneIntersectionType.Intersecting)
                                    {
                                        collisions.Add(t);

                                        //float deltaY = Math.Abs(t.Boundary.Min.Y - this.Boundary.Max.Y);

                                        //this.move(new Vector3(0, deltaY, 0));
                                    }
                                }
                            }

                            if (collisions.Count != 0)
                                return true;
                            else
                                return false;
                        }
                    } break;
                #endregion

                #region down
                case "down":
                    {
                        List<Tile> possibleCollisions = new List<Tile>();

                        foreach (Tile t in visibles)
                        {
                            Vector3[] boundCornTile = t.Boundary.GetCorners();
                            Vector3[] boundCornEnt = this.Boundary.GetCorners();

                            if (boundCornTile[0].Y < boundCornEnt[2].Y && boundCornTile[1].X > boundCornEnt[0].X && boundCornTile[0].X < boundCornEnt[1].X)
                                possibleCollisions.Add(t);
                        }

                        if (possibleCollisions.Count != 0)
                        {
                            List<Tile> collisions = new List<Tile>();

                            foreach (Tile t in possibleCollisions)
                            {
                                Vector3[] boundCornTile = t.Boundary.GetCorners();

                                Plane topTilePlane = new Plane(boundCornTile[0], boundCornTile[1], boundCornTile[4]);

                                if (topTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Front)
                                {
                                    BoundingBox newBound = new BoundingBox(this.Boundary.Min + moveVecEntity, this.Boundary.Max + moveVecEntity);

                                    if (topTilePlane.Intersects(newBound) == PlaneIntersectionType.Intersecting)
                                    {
                                        collisions.Add(t);

                                        float deltaY = Math.Abs(t.Boundary.Max.Y - this.Boundary.Min.Y);

                                        this.move(new Vector3(0, -deltaY, 0));
                                    }
                                }
                            }

                            if (collisions.Count != 0)
                                return true;
                            else
                                return false;
                        }
                    } break;
                #endregion
            }

            return false;
        }

        #region DRAW Methods
        //zeichnet 3D Objekt im Bezug auf den Viewport
        public void drawOn2DScreen(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix view = Camera2D.ViewMatrix;
            Matrix projection = Camera2D.ProjectionMatrix;
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);

            Matrix world = Matrix.CreateScale(scale) * rotation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = world;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        //zeichnet 3D Objekt in Bezug auf die Spielwelt
        public void drawIn2DWorldWithoutBones(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix view = Camera2D.ViewMatrix;
            Matrix projection = Camera2D.ProjectionMatrix;
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = world;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
        public void drawIn2DWorld(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            //http://gamedev.stackexchange.com/questions/38637/models-from-3ds-max-lose-their-transformations-when-input-into-xna
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix view = Camera2D.ViewMatrix;
            Matrix projection = Camera2D.ProjectionMatrix;
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        //zeichnet 3D Objekt in Bezug auf die Arenakamera
        public void drawInArenaWithoutBones(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix view = CameraArena.ViewMatrix;
            Matrix projection = CameraArena.ProjectionMatrix;
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(this.Position);

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = world;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
        public void drawInArena(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix view = CameraArena.ViewMatrix;
            Matrix projection = CameraArena.ProjectionMatrix;
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(this.Position);

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            Vector3 lightPos0 = new Vector3(-50,-50,50);

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.EnableDefaultLighting();

                    //Advanced Lighting Parameters
                    /*effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = lightPos0;

                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                    Console.Clear();
                    Console.WriteLine("0: " + effect.DirectionalLight0.Direction);
                    Console.WriteLine("1: " + effect.DirectionalLight1.Direction);
                    Console.WriteLine("2: " + effect.DirectionalLight2.Direction);*/
                }
                mesh.Draw();
            }
        }
        #endregion

        #endregion
    }
}