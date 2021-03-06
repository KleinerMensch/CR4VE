﻿using System;
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
        public static SoundEffect checkpointCrystal;
        public static SoundEffect fireball;
        public static SoundEffect claws;
        public static SoundEffect freezingIce;
        public static SoundEffect whip;
        public static SoundEffect laser;
        public static SoundEffect SeraphinScream;
        public static SoundEffect thunder;
        public static SoundEffect spear;
        public static SoundEffect mainMenu;
        public static SoundEffectInstance menu;
        public static SoundEffect punch;
        public static SoundEffect waterSound;
        public static SoundEffectInstance water;
        public static SoundEffect spawn;
        public static SoundEffect minionsFraktus;
        public static SoundEffect goldCave;
        public static SoundEffectInstance cave;
        public static SoundEffect crystalBackground;
        public static SoundEffectInstance crystalBG;
 
        #endregion

        #region Methods
        public static void Initialize(ContentManager content) 
        {
            scream = content.Load<SoundEffect>("Assets/Sounds/deathScream");
            collectSound = content.Load<SoundEffect>("Assets/Sounds/itemCollect");
            checkpointHell = content.Load<SoundEffect>("Assets/Sounds/CheckpointHell");
            checkpointCrystal = content.Load<SoundEffect>("Assets/Sounds/CheckpointCristall");
            fireball = content.Load<SoundEffect>("Assets/Sounds/Fireball");
            freezingIce = content.Load<SoundEffect>("Assets/Sounds/freezingIce");
            whip = content.Load<SoundEffect>("Assets/Sounds/whip");
            laser = content.Load<SoundEffect>("Assets/Sounds/laser");
            SeraphinScream = content.Load<SoundEffect>("Assets/Sounds/SeraphinScream");
            thunder = content.Load<SoundEffect>("Assets/Sounds/thunder");
            spear = content.Load<SoundEffect>("Assets/Sounds/spear");
            claws = content.Load<SoundEffect>("Assets/Sounds/claws");
            mainMenu = content.Load<SoundEffect>("Assets/Sounds/mainMenu");
            menu = mainMenu.CreateInstance();
            menu.IsLooped = true;
            punch = content.Load<SoundEffect>("Assets/Sounds/punch");
            waterSound = content.Load<SoundEffect>("Assets/Sounds/water");
            water = waterSound.CreateInstance();
            spawn = content.Load<SoundEffect>("Assets/Sounds/spawn");
            minionsFraktus = content.Load<SoundEffect>("Assets/Sounds/minions");
            goldCave = content.Load<SoundEffect>("Assets/Sounds/GoldCave");
            cave = goldCave.CreateInstance();
            //cave.IsLooped = true;

            //background crystal
            crystalBackground = content.Load<SoundEffect>("Assets/Sounds/crystalBackground");
            crystalBG = crystalBackground.CreateInstance();
            crystalBG.Volume = 0.25f;
            crystalBG.IsLooped = true;            
        }
        #endregion
    }
}
