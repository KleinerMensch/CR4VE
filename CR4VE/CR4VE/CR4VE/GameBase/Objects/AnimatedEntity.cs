using CR4VE.GameBase.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameBase.Objects
{
    class AnimatedEntity : Entity
    {
        #region Member
        AnimationPlayer animationPlayer;
        #endregion

        #region Constructors
        //Konstruktor laedt das Modell anhand eines Strings aus dem Model-Verzeichnis
        public AnimatedEntity(Vector3 pos, String modelName, ContentManager cm)
            : base(pos, modelName, cm)
        {
            // Look up our custom skinning information.
            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

            animationPlayer.StartClip(clip);
        }
        public AnimatedEntity(Vector3 pos, String modelName, ContentManager cm, BoundingBox bound)
            : base(pos, modelName, cm, bound)
        {
            // Look up our custom skinning information.
            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

            animationPlayer.StartClip(clip);
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
        }

        public void Draw(GameTime gameTime)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // Render the skinned mesh.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = Matrix.CreateLookAt(Camera2D.Position3D, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), (float)8 / 6, 10f, 1000);

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }
                mesh.Draw();
            }
        }
    }
}
