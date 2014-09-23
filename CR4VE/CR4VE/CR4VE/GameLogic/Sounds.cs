using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameLogic.GameStates;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.Characters;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace CR4VE.GameLogic
{
    static class Sounds
    {
        #region Attributes
        public static SoundEffect scream;
        public static SoundEffect collectSound;
        public static SoundEffect checkpointHell;
        public static SoundEffect fireball;
        public static SoundEffect freezingIce;
        public static SoundEffect whip;
        public static SoundEffect SeraphinScream;
        #endregion

        #region Properties
        #endregion

        #region Methods

        public static void Initialize(ContentManager content) 
        {
            scream = content.Load<SoundEffect>("Assets/Sounds/deathScream");
            collectSound = content.Load<SoundEffect>("Assets/Sounds/itemCollect");
            checkpointHell = content.Load<SoundEffect>("Assets/Sounds/CheckpointHell");
            fireball = content.Load<SoundEffect>("Assets/Sounds/Fireball");
            freezingIce = content.Load<SoundEffect>("Assets/Sounds/freezingIce");
            whip = content.Load<SoundEffect>("Assets/Sounds/whip");
            SeraphinScream = content.Load<SoundEffect>("Assets/Sounds/SeraphinScream");
        }

        public static void PlaySound(SoundEffect sound)
        {
            sound.Play();
        }

        public static void Update() 
        {
        
        }
        #endregion
    }
}
