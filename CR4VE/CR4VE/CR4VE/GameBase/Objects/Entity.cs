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
        public Texture2D texture;

        //Blickrichtung
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
        public void move(Vector3 offset)
        {
            this.Position += offset;

            this.boundary.Min += offset;
            this.boundary.Max += offset;
        }
        public void moveTo(Vector3 destination)
        {
            Vector3 deltaPos = destination - this.Position;

            this.Position = destination;

            this.boundary.Min = this.Boundary.Min + deltaPos;
            this.boundary.Max = this.Boundary.Max + deltaPos;
        }

        //checks if entity boundary intersects with terrain tile boundary
        public bool checkFooting(List<Tile> visibles)
        {
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
        public bool checkRightBorder(List<Tile> visibles)
        {
            List<Tile> possibleCollisions = new List<Tile>();

            foreach (Tile t in visibles)
            {
                Vector3[] boundCornTile = t.Boundary.GetCorners();
                Vector3[] boundCornEnt = this.Boundary.GetCorners();

                if (boundCornTile[0].X <= boundCornEnt[1].X && boundCornTile[0].Y > boundCornEnt[2].Y && boundCornTile[2].Y < boundCornEnt[1].Y)
                    possibleCollisions.Add(t);
            }

            if (possibleCollisions.Count != 0)
            {
                foreach (Tile t in possibleCollisions)
                {
                    Vector3[] boundCornTile = t.Boundary.GetCorners();

                    Plane leftTilePlane = new Plane(boundCornTile[0], boundCornTile[3], boundCornTile[4]);

                    if (leftTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Intersecting)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool checkLeftBorder(List<Tile> visibles)
        {
            List<Tile> possibleCollisions = new List<Tile>();

            foreach (Tile t in visibles)
            {
                Vector3[] boundCornTile = t.Boundary.GetCorners();
                Vector3[] boundCornEnt = this.Boundary.GetCorners();

                if (boundCornTile[1].X >= boundCornEnt[0].X && boundCornTile[0].Y > boundCornEnt[2].Y && boundCornTile[2].Y < boundCornEnt[1].Y)
                    possibleCollisions.Add(t);
            }

            if (possibleCollisions.Count != 0)
            {
                foreach (Tile t in possibleCollisions)
                {
                    Vector3[] boundCornTile = t.Boundary.GetCorners();

                    Plane rightTilePlane = new Plane(boundCornTile[1], boundCornTile[2], boundCornTile[5]);

                    if (rightTilePlane.Intersects(this.Boundary) == PlaneIntersectionType.Intersecting)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //checks if entity collides with terrain in specified direction and sets it back accordingly
        public bool handleTerrainCollisionInDirection(String direction, Vector3 moveVecEntity, List<Tile> visibles)
        {
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

                                        Camera2D.realign(new Vector3(-deltaX, 0, 0), this.Position);
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

                                        Camera2D.realign(new Vector3(deltaX, 0, 0), this.Position);
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

                                        float deltaY = Math.Abs(t.Boundary.Min.Y - this.Boundary.Max.Y);

                                        //this.move(new Vector3(0, deltaY, 0));

                                        Camera2D.realign(new Vector3(0, deltaY, 0), this.Position);
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

                                        Camera2D.realign(new Vector3(0, -deltaY, 0), this.Position);

                                        //handle damaging tiles beneath player
                                        switch (t.Type)
                                        {
                                            case "lava":
                                                Singleplayer.hud.isBurning = true;
                                                break;

                                            case "water":
                                                Singleplayer.hud.isSwimming = true;
                                                break;

                                            case "ground":
                                                Singleplayer.hud.healthLeft = 0;
                                                break;
                                            
                                            default:
                                                break;
                                        }
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
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);

            Matrix world = Matrix.CreateScale(scale) * rotation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = Camera2D.ViewMatrix;
                    effect.Projection = Camera2D.ProjectionMatrix;
                    effect.World = world;

               //Beleuchtung
                    // effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
                    effect.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularPower = 40.0f;
                    effect.Alpha = 0.4f;

                  
                    if (effect.LightingEnabled)
                    {
                        effect.DirectionalLight0.Enabled = true; 
                        if (effect.DirectionalLight0.Enabled)
                        {
                            // richtung x(licht nach rechts)
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f); 
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 0, 0));
                            // punkte aus dem licht auf den ursprung der szene
                            effect.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight1.Enabled = true;
                        if (effect.DirectionalLight1.Enabled)
                        {
                            // richtung y(licht von oben)
                            effect.DirectionalLight1.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
                            effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                            effect.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight2.Enabled = true;
                        if (effect.DirectionalLight2.Enabled)
                        {
                            // richtung z(licht von vorne)+y
                            effect.DirectionalLight2.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                            effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1, -1));
                            effect.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
        }
        //zeichnet 3D Objekt in Bezug auf die Spielwelt
        public void drawIn2DWorldWithoutBones(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = Camera2D.ViewMatrix;
                    effect.Projection = Camera2D.ProjectionMatrix;
                    effect.World = world;

               //beleuchtung x,y,z 
                   // effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
                    effect.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularPower = 40.0f;
                    effect.Alpha = 0.4f;

                    if (effect.LightingEnabled)
                    {
                        effect.DirectionalLight0.Enabled = true; 
                        if (effect.DirectionalLight0.Enabled)
                        {
                            // richtung x
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f); 
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 0, 0));
                            effect.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight1.Enabled = true;
                        if (effect.DirectionalLight1.Enabled)
                        {
                            // richtung y
                            effect.DirectionalLight1.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
                            effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                            effect.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight2.Enabled = true;
                        if (effect.DirectionalLight2.Enabled)
                        {
                            // richtung y,z
                            effect.DirectionalLight2.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                            effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1, -1));
                            effect.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
        }
        public void drawIn2DWorld(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            //http://gamedev.stackexchange.com/questions/38637/models-from-3ds-max-lose-their-transformations-when-input-into-xna
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = Camera2D.ViewMatrix;
                    effect.Projection = Camera2D.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * world;

                    Vector3 lightPos0 = new Vector3(-50, -50, 50);

                 // Beleuchtung x,y,z
                    //effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
                    effect.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularPower = 40.0f;
                    effect.Alpha = 0.4f;

                   
                    if (effect.LightingEnabled)
                    {
                        effect.DirectionalLight0.Enabled = true; 
                        if (effect.DirectionalLight0.Enabled)
                        {
                            // richtung x
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f); 
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 0, 0));
                            effect.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight1.Enabled = true;
                        if (effect.DirectionalLight1.Enabled)
                        {
                            // richtung y
                            effect.DirectionalLight1.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
                            effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                            effect.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight2.Enabled = true;
                        if (effect.DirectionalLight2.Enabled)
                        {
                            // richtung z +y
                            effect.DirectionalLight2.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                            effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1, -1));
                            effect.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
        }
        //zeichnet 3D Objekt in Bezug auf die Arenakamera
        public void drawInArenaWithoutBones(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(this.Position);

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = CameraArena.ViewMatrix;
                    effect.Projection = CameraArena.ProjectionMatrix;
                    effect.World = world;

                  //beleuchtung
                    //   effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;

                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
                    effect.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularPower = 60.0f;
                    effect.Alpha = 0.4f;

                    
                    if (effect.LightingEnabled)
                    {
                        effect.DirectionalLight0.Enabled = true; 
                        if (effect.DirectionalLight0.Enabled)
                        {
                            // richtung x
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.7f, 0.7f); 
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 1, 0));
                            effect.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight1.Enabled = true;
                        if (effect.DirectionalLight1.Enabled)
                        {
                            // richtung y
                            effect.DirectionalLight1.DiffuseColor = new Vector3(1f, 0.8f, 0.8f);
                            effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                            effect.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight2.Enabled = true;
                        if (effect.DirectionalLight2.Enabled)
                        {
                            // richtung z +y
                            effect.DirectionalLight2.DiffuseColor = new Vector3(0.3f, 0.3f, 0.3f);
                            effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1, -1));
                            effect.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
        }
        public void drawInArena(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(this.Position);

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            Vector3 lightPos0 = new Vector3(-50,-50,50);

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = CameraArena.ViewMatrix;
                    effect.Projection = CameraArena.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    

                    //Advanced Lighting Parameters
                    /*effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = lightPos0;

                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                    Console.Clear();
                    Console.WriteLine("0: " + effect.DirectionalLight0.Direction);
                    Console.WriteLine("1: " + effect.DirectionalLight1.Direction);
                    Console.WriteLine("2: " + effect.DirectionalLight2.Direction);*/
                    
                 //Beleuchtung
                    //   effect.EnableDefaultLighting();

                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
                    effect.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularPower = 60.0f;
                    effect.Alpha = 0.4f;

                    effect.LightingEnabled = true;
                    if (effect.LightingEnabled)
                    {
                        effect.DirectionalLight0.Enabled = true; 
                        if (effect.DirectionalLight0.Enabled)
                        {
                            // richtung x
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.7f, 0.7f); 
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 1, 0));
                            effect.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight1.Enabled = true;
                        if (effect.DirectionalLight1.Enabled)
                        {
                            // richtung y
                            effect.DirectionalLight1.DiffuseColor = new Vector3(1f, 0.8f, 0.8f);
                            effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                            effect.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight2.Enabled = true;
                        if (effect.DirectionalLight2.Enabled)
                        {
                            // richtung z+y
                            effect.DirectionalLight2.DiffuseColor = new Vector3(0.3f, 0.3f, 0.3f);
                            effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1, -1));
                            effect.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
        }

        //public void DrawModelWithEffect(Vector3 scale, float rotX, float rotY, float rotZ)
        //{
        //    Matrix[] transforms = new Matrix[model.Bones.Count];
        //    model.CopyAbsoluteBoneTransformsTo(transforms);

        //    Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
        //    Matrix translation = Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

        //    Matrix world = Matrix.CreateScale(scale) * rotation * translation;

        //    foreach (ModelMesh mesh in model.Meshes)
        //    {
        //        foreach (ModelMeshPart part in mesh.MeshParts)
        //        {
        //            part.Effect = Singleplayer.effect_directLight;
        //            Singleplayer.effect_directLight.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * world);
        //            Singleplayer.effect_directLight.Parameters["View"].SetValue(Camera2D.ViewMatrix);
        //            Singleplayer.effect_directLight.Parameters["Projection"].SetValue(Camera2D.ProjectionMatrix);
        //            //Singleplayer.effect.Parameters["ModelTexture"].SetValue(texture);


        //        }
        //        mesh.Draw();
        //    }
        //}

        public void drawInMainMenu(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(CameraMenu.transform3D(this.Position));

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = CameraMenu.ViewMatrix;
                    effect.Projection = CameraMenu.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                   

                    //Advanced Lighting Parameters
                    /*effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = lightPos0;

                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                    Console.Clear();
                    Console.WriteLine("0: " + effect.DirectionalLight0.Direction);
                    Console.WriteLine("1: " + effect.DirectionalLight1.Direction);
                    Console.WriteLine("2: " + effect.DirectionalLight2.Direction);*/


               //beleuchtung
                   // effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
                    effect.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularColor = new Vector3(1f, 1f, 1f);
                    effect.SpecularPower = 40.0f;
                    effect.Alpha = 0.4f;

                    effect.LightingEnabled = true;
                    if (effect.LightingEnabled)
                    {
                        effect.DirectionalLight0.Enabled = true;
                        if (effect.DirectionalLight0.Enabled)
                        {
                            // richtung x
                            effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f); 
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, 0, 0));
                            effect.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight1.Enabled = true;
                        if (effect.DirectionalLight1.Enabled)
                        {
                            // richtung y
                            effect.DirectionalLight1.DiffuseColor = new Vector3(0.4f, 0.4f, 0.4f);
                            effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                            effect.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        effect.DirectionalLight2.Enabled = true;
                        if (effect.DirectionalLight2.Enabled)
                        {
                            // richtung z+y
                            effect.DirectionalLight2.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                            effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1, -1));
                            effect.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
        }
        #endregion

        #endregion
    }
}