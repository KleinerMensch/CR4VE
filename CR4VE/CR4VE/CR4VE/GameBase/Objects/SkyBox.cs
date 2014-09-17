using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;


namespace CR4VE.GameBase.Objects
{
    public class SkyBox : Entity
    {

        public Model skyBox;
        public TextureCube skyBoxTexture;
        public Effect skyBoxEffect;
      //  public Texture2D texture;

        // public float size = 200f;
        public SkyBox() { }

        public SkyBox(Vector3 pos, string skyboxTexture, ContentManager Content)
        {
            this.position = pos;
            skyBox = Content.Load<Model>("Assets/Models/Terrain/10x10x10Box1");
            skyBoxTexture = Content.Load<TextureCube>(skyboxTexture);
            skyBoxEffect = Content.Load<Effect>("Assets/Effects/SkyBox");
            //  skyBoxEffect = Content.Load<Effect>("Assets/Effects/Rainfall");
        }

        
        public void Drawsky(Vector3 scale, float rotX, float rotY, float rotZ)
        {
            Matrix[] transforms = new Matrix[skyBox.Bones.Count];
            skyBox.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix rotation = Matrix.CreateRotationX(rotX) * Matrix.CreateRotationY(rotY) * Matrix.CreateRotationZ(rotZ);
            Matrix translation = Matrix.CreateTranslation(Camera2D.transform3D(this.Position));

            Matrix world = Matrix.CreateScale(scale) * rotation * translation;
           

            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * world); // * Matrix.CreateScale(size)
                        part.Effect.Parameters["View"].SetValue(Camera2D.ViewMatrix);
                        part.Effect.Parameters["Projection"].SetValue(Camera2D.ProjectionMatrix);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        //   part.Effect.Parameters["Camera2D"].SetValue(Camera2D.Position2D);
                    }

                    
                    mesh.Draw();
                }
            }
        }
    }
}
