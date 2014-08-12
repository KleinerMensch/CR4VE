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
        public Vector3 viewingDirection = new Vector3(0, 0, 0);
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
                this.boundary.Min = this.Position + new Vector3(-2.5f, -2.5f, -2.5f);
                this.boundary.Max = this.Position + new Vector3(2.5f, 2.5f, 2.5f);
            }
        }
        public void moveTo(Vector3 destination)
        {
            this.Position = destination;

            //moving bounding box (Sidescroller)
            //(noch hart gecoded fuer sphereD5)
            if (Game1.currentState.Equals(Game1.EGameState.Singleplayer))
            {
                this.boundary.Min = this.Position + new Vector3(-2.5f, -2.5f, -2.5f);
                this.boundary.Max = this.Position + new Vector3(2.5f, 2.5f, 2.5f);
            }
        }

        public static void getTerrainCollisions(Entity entity)
        {
            Vector3[] boundCornEnt = entity.Boundary.GetCorners();

            List<Tile> visibles = Tilemap.getVisibleTiles();

            List<String> collisions = new List<String>();

            foreach (Tile t in visibles)
            {
                if (entity.Boundary.Intersects(t.Boundary))
                {
                    //relative Position bestimmen (Tile im Bezug auf die Entity)
                    String relativePos = "";

                    if (entity.Position.X < t.Position.X)
                        relativePos = "right";
                    else if (entity.Position.X > t.Position.X)
                        relativePos = "left";

                    if (!collisions.Contains(relativePos))
                    {
                        collisions.Add(relativePos);
                    }

                    if (entity.Position.Y < t.Position.Y)
                        relativePos = "above";
                    else if (entity.Position.Y > t.Position.Y)
                        relativePos = "beneath";

                    if (!collisions.Contains(relativePos))
                        collisions.Add(relativePos);

                    Vector3[] boundCornTile = t.Boundary.GetCorners();

                    //right
                    if (boundCornEnt[1].X >= boundCornTile[0].X && relativePos.Contains("right"))
                    {
                        KeyboardControls.borderedRight = true;

                        float deltaX = Math.Abs(boundCornEnt[1].X - boundCornTile[0].X);
                        entity.move(new Vector3(-deltaX, 0, 0));
                    }
                    //left
                    if (boundCornEnt[0].X <= boundCornTile[1].X && relativePos.Contains("left"))
                    {
                        KeyboardControls.borderedLeft = true;

                        float deltaX = Math.Abs(boundCornEnt[0].X - boundCornTile[1].X);
                        entity.move(new Vector3(deltaX, 0, 0));
                    }

                    //above
                    if (boundCornEnt[0].Y >= boundCornTile[2].Y && relativePos.Contains("above"))
                    {
                        KeyboardControls.borderedTop = true;

                        float deltaY = Math.Abs(boundCornEnt[0].Y - boundCornTile[2].Y);
                        entity.move(new Vector3(0, -deltaY, 0));
                    }
                    //beneath
                    if (boundCornEnt[2].Y <= boundCornTile[0].Y && relativePos.Contains("beneath"))
                    {
                        KeyboardControls.borderedBottom = true;
                        KeyboardControls.borderedTop = false;

                        KeyboardControls.isJumping = false;
                        KeyboardControls.isFalling = false;

                        float deltaY = Math.Abs(boundCornEnt[2].Y - boundCornTile[0].Y);
                        entity.move(new Vector3(0, deltaY, 0));
                    }
                }
            }

            //return collisions;
        }
        public static Vector3 influenceMovementByTerrainCollisions(Vector3 moveVecEntity, Entity ent)
        {
            List<Tile> visibles = Tilemap.getVisibleTiles();

            Vector3 result = moveVecEntity;

            foreach (Tile t in visibles)
            {
                if (ent.Boundary.Intersects(t.Boundary))
                {
                    //relative Position bestimmen (Tile im Bezug auf die Entity)
                    String relaPos = "";

                    if (ent.Position.X < t.Position.X)
                        relaPos += "right";
                    else if (ent.Position.X > t.Position.X)
                        relaPos += "left";

                    if (ent.Position.Y < t.Position.Y)
                        relaPos += "above";
                    else if (ent.Position.Y > t.Position.Y)
                        relaPos += "beneath";

                    //get corners of entity and tile boundaries
                    Vector3[] boundCornEnt = ent.Boundary.GetCorners();
                    Vector3[] boundCornTile = t.Boundary.GetCorners();

                    //Entity Planes
                    Plane entLeft = new Plane(boundCornEnt[0], boundCornEnt[3], boundCornEnt[4]);
                    Plane entRight = new Plane(boundCornEnt[1], boundCornEnt[2], boundCornEnt[5]);
                    Plane entTop = new Plane(boundCornEnt[0], boundCornEnt[1], boundCornEnt[4]);
                    Plane entBottom = new Plane(boundCornEnt[2], boundCornEnt[3], boundCornEnt[7]);
                    //Tile Planes
                    Plane tileLeft = new Plane(boundCornTile[0], boundCornTile[3], boundCornTile[4]);
                    Plane tileRight = new Plane(boundCornTile[1], boundCornTile[2], boundCornTile[5]);
                    Plane tileTop = new Plane(boundCornTile[0], boundCornTile[1], boundCornTile[4]);
                    Plane tileBottom = new Plane(boundCornTile[2], boundCornTile[3], boundCornTile[7]);

                    //influence moveVecPlayer according to plane collisions
                    //left
                    if (tileRight.Equals(entLeft) && relaPos.Contains("left"))
                        if (result.X < 0) result.X = 0;
                    //right
                    if (tileLeft.Equals(entRight) && relaPos.Contains("right"))
                        if (result.X > 0) result.X = 0;
                    //beneath
                    if (tileTop.Equals(entBottom) && relaPos.Contains("beneath"))
                    {
                        //isJumping = false;
                        if (result.Y < 0) result.Y = 0;
                    }
                    //above
                    if (tileBottom.Equals(entTop) && relaPos.Contains("above"))
                        if (result.Y > 0) result.Y = 0;
                }
            }

            return result;
        }
        public static Vector3 influenceMovementByTerrainCollisions2(Vector3 moveVecEntity, Entity ent)
        {
            List<Tile> visibles = Tilemap.getVisibleTiles();

            Vector3 result = moveVecEntity;

            foreach (Tile t in visibles)
            {
                if (ent.Boundary.Intersects(t.Boundary))
                {
                    //relative Position bestimmen (Tile im Bezug auf die Entity)
                    String relaPos = "";

                    if (ent.Position.X < t.Position.X)
                        relaPos += "right";
                    else if (ent.Position.X > t.Position.X)
                        relaPos += "left";

                    if (ent.Position.Y < t.Position.Y)
                        relaPos += "above";
                    else if (ent.Position.Y > t.Position.Y)
                        relaPos += "beneath";

                    //get corners of entity and tile boundaries
                    Vector3[] boundCornEnt = ent.Boundary.GetCorners();
                    Vector3[] boundCornTile = t.Boundary.GetCorners();

                    //Entity Planes
                    Plane entLeft = new Plane(boundCornEnt[0], boundCornEnt[3], boundCornEnt[4]);
                    Plane entRight = new Plane(boundCornEnt[1], boundCornEnt[2], boundCornEnt[5]);
                    Plane entTop = new Plane(boundCornEnt[0], boundCornEnt[1], boundCornEnt[4]);
                    Plane entBottom = new Plane(boundCornEnt[2], boundCornEnt[3], boundCornEnt[7]);
                    //Tile Planes
                    Plane tileLeft = new Plane(boundCornTile[0], boundCornTile[3], boundCornTile[4]);
                    Plane tileRight = new Plane(boundCornTile[1], boundCornTile[2], boundCornTile[5]);
                    Plane tileTop = new Plane(boundCornTile[0], boundCornTile[1], boundCornTile[4]);
                    Plane tileBottom = new Plane(boundCornTile[2], boundCornTile[3], boundCornTile[7]);

                    //influence moveVecPlayer according to plane collisions
                    //left
                    if (tileRight.Intersects(ent.Boundary) == PlaneIntersectionType.Intersecting && relaPos.Contains("left"))
                    {
                        KeyboardControls.borderedLeft = true;
                        if (result.X < 0) result.X = 0;
                    }
                    //right
                    else if (tileLeft.Intersects(ent.Boundary) == PlaneIntersectionType.Intersecting && relaPos.Contains("right"))
                    {
                        KeyboardControls.borderedRight = true;
                        if (result.X > 0) result.X = 0;
                    }
                    //beneath
                    if (tileTop.Intersects(ent.Boundary) == PlaneIntersectionType.Intersecting && relaPos.Contains("beneath"))
                    {
                        KeyboardControls.borderedBottom = true;
                        KeyboardControls.isJumping = false;
                        if (result.Y < 0) result.Y = 0;
                    }
                    //above
                    else if (tileBottom.Intersects(ent.Boundary) == PlaneIntersectionType.Intersecting && relaPos.Contains("above"))
                    {
                        KeyboardControls.borderedTop = true;
                        if (result.Y > 0) result.Y = 0;
                    }
                }
            }
            Console.WriteLine(result);
            return result;
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
        public void drawInArena(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix view = Camera2D.ViewMatrix;
            Matrix projection = Camera2D.ProjectionMatrix;
            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(this.Position);

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
        #endregion

        #endregion
    }
}