﻿using System;
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
using CR4VE.GameBase.Objects.Terrain;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.Characters;
using CR4VE.GameBase.HeadUpDisplay;

namespace CR4VE.GameLogic.GameStates
{
    class Singleplayer : GameStateInterface
    {
        #region Attribute
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;

        static float vibrTimer;

        public static Tilemap terrainMap;

        //Player
        public static Character ghost;
        public static Character player;

        //Checkpoints
        public static Checkpoint lastCheckpoint;
        public static Checkpoint c1_hell;

        //Powerups
        public static Powerup powerup_health;
        public static Powerup powerup_mana;
        
        //Enemies
        public static List<Enemy> enemyList = new List<Enemy>();
        public static List<Powerup> powerUpList = new List<Powerup>();

        //HUD
        public static HUD hud;
        #endregion

        #region Konstruktor
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;
            cont = content;

            #region Terrain
            terrainMap = new Tilemap();
            
            Tile.Content = content;

            int[,] layout = new int[,] {
        
           {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,96, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0},
           {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 3, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
           {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 0, 3, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 3, 4, 4, 3, 3, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0},
           {3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0},
           {3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0},
           {3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0}};

            int[,] layout2 = new int[,] { 

            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 5, 0, 0, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {3, 1, 0, 0, 5, 5, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 5, 0, 0, 5, 0, 0, 0, 5, 5, 0, 0},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 8, 8, 8, 8, 5, 5, 8, 8, 8, 8, 5, 9, 5, 0},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9 ,9, 5},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9},
            {3, 3, 3, 3, 3, 3, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 8, 8, 5, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 9}};


            int[,] layout3 = new int[,] { 

            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 0, 0, 0, 9, 0, 0, 9, 9, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 0, 0, 0, 9, 0, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 0, 0, 0, 0, 9},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9}};


            int[,] layout4 = new int[,] { 

            {0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13,13,13, 0, 0,13, 0, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,14,14, 0, 0, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13, 0, 0,13,13,13,13,13,13, 0, 0, 0, 0, 0,13, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13}};


            int[,] layout5 = new int[,] { 
          
           {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           { 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           { 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10,10, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10,10, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10,10,10, 0, 0, 0, 0,10, 0, 0, 0, 0},
           {13,13,13,13, 0, 0,10, 0, 0,10, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0,10,10, 0, 0, 0,10, 0, 0,10, 0,10,10, 0, 0,10,10, 0, 0, 0, 0, 0, 0, 0,10,10,10,10, 0, 0, 0},
           {13,13,13,13,14,14,10, 0,10,10,14,14,14,14,14,14,14,10,14,14,14,14,14,14,14,14,10,10,14,14,14,10,14,14,10 ,0, 0,10,14,14,14,14,14,14,14,14,14,14,14,10,10,10,10,14,14,14},
           {13,13,13,13,14,14,10, 0,10,14,14,14,14,14,14,14,14,10,14,14,14,14,14,14,14,14,10,10,14,14,14,10,14,14,10,10 ,0,10,10,10,10,10,14,14,14,14,14,14,14,10,10,10,10,14,14,14},
           {13,13,13,13,14,14,10,10,10,14,14,14,14,14,14,14,14,10,14,14,14,14,14,14,14,14,10,10,14,14,14,10,14,14,10,10, 0, 0, 0, 0, 0,10,14,14,14,14,14,14,14,10,10,10,10,14,14,14},
           {13,13,13,13,14,14,10,10,10,14,14,14,14,14,14,14,14,10,14,14,14,14,14,14,14,14,10,10,14,14,14,10,14,14,10,10,10,10,10,10,10,10,14,14,14,14,14,14,14,10,10,10,10,14,14,14}};


            int[,] layout6 = new int[,] { 
   
           { 13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {  0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {  0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13},
           {  0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           {  0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0,12,12, 0, 0,13},
           { 12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13, 13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0,12,12,0, 0,13},
           { 12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12,12,12,12, 0, 0,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12, 0, 0,12,12, 0, 0, 0, 0,12,12,12,12, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12, 0,12,12,12,12,12,12,12,12,12,12,12,12, 0,12,12,12,12,12,12, 0, 0,12,12, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12,12,11,11,11,11,11,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12,12,12,12,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12,11,11,12,12,12,12,12,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12, 0, 0,13},
           { 12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,12, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13}};


            int[,] layout7 = new int[,] { 
  
            { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0,13,13,13,13, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13},
            { 0,13,13, 0, 0, 0, 0, 0,19,19, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0,19, 0, 0,19, 0, 0, 0, 0, 0, 0, 0,19, 0, 0, 0, 0, 0, 0,13,13,13,13,13},
            { 0, 0, 0, 0, 0, 0, 0, 0,19, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13},
            { 0, 0, 0, 0, 0, 0,19, 0,19,19,19,19,19,19,19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,15,15,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19,19, 0, 0, 0,19, 0, 0,19,19, 0, 0, 0, 0, 0},
            { 15,15,15,15,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18,18,18,18,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {18,18,18,18,15, 0, 0, 0, 0, 0,15, 0, 0,15, 0, 0, 0, 0, 0, 0, 0,15,15, 0, 0,15,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19,19, 0, 0},
            {18,18,18,18,18,15,15, 0, 0,15,18,15,15,18,15, 0, 0,15,15,15, 0, 0,18, 0, 0,18,18, 0, 0, 0,15,15, 0, 0,15, 0, 0, 0,18,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {18,18,18,18,18,18,18,16,16,18,18,18,18,18,18,16,16,18, 0, 0, 0, 0,18,16,16,18,18,15,15,15,18,18,16,16,18,15,15,15,18,18,15,15,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16},
            {18,18,18,18,18,18,18,16,16,18,18,18,18,18,18,16,16,18,15,15,15,15,18,16,16,18,18,18,18,18,18,18,16,16,18,18,18,18,18,18,18,18,18,18,15, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16},
            {18,18,18,18,18,18,18,16,16,18,18,18,18,18,18,16,16,18,18,18,18,18,18,16,16,18,18,18,18,18,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16},
            {18,18,18,18,18,18,18,16,16,18,18,18,18,18,18,16,16,18,18,18,18,18,18,16,16,18,18,18,18,18,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16}};
     

            int[,] layout8 = new int[,] { 

            { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19, 0,19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19, 0,15,15},
            { 0, 0,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19,19, 0, 0, 0, 0, 0, 0, 0,19,19,19, 0, 0,19, 0, 0, 0, 0, 0, 0, 0, 0,15, 0, 0, 0, 0,15, 0, 0, 0, 0, 0,19, 0, 0,16,18,18},
            { 0,16,18, 0,15,15, 0, 0, 0,15, 0, 0, 0,15, 0, 0, 0, 0, 0,15, 0, 0,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0,15,15, 0, 0, 0,15,18,15, 0, 0,15,18, 0, 0, 0,15, 0, 0, 0,16,16,18,18},
            {16,16,18, 0,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18},
            {16,16,18,15,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18},
            {16,16,18,18,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18}};
                
            int boxSize = 10;

            terrainMap.Generate(layout, boxSize, new Vector3(0,0,0));
            terrainMap.Generate(layout2, boxSize, new Vector3(520,20,0));
            terrainMap.Generate(layout3, boxSize, new Vector3(1040,-50,0));
          //  terrainMap.Generate(layout4, boxSize, new Vector3(0,0,0));
           // terrainMap.Generate(layout5, boxSize, new Vector3(0,0,0));
          //  terrainMap.Generate(layout6, boxSize, new Vector3(0,0,0));
           // terrainMap.Generate(layout7, boxSize, new Vector3(0,0,0));
            //terrainMap.Generate(layout8, boxSize, new Vector3(0,0,0));
            #endregion            

            //Camera
            Camera2D.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            //SaveGame
            SaveGame.Reset();

            //Textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");

            //Player
            ghost = new Character(Vector3.Zero, "skull", content);
            player = new CharacterSeraphin(Vector3.Zero, "sphereD5", content, new BoundingBox(new Vector3(-2.5f, -2.5f, -2.5f), new Vector3(2.5f, 2.5f, 2.5f)));
            
            //Checkpoints
            lastCheckpoint = new Checkpoint(Vector3.Zero, "checkpoint_hell", content);
            
            #region Loading AI
            EnemyRedEye redEye;
            EnemySkull skull;
            EnemySpinningCrystal spinningCrystal;
            EnemyShootingCrystal shootingCrystal;

            redEye = new EnemyRedEye(new Vector3(80, 0, 0),"EnemyEye",content,new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            skull = new EnemySkull(new Vector3(400, 0, 0), "skull", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            spinningCrystal = new EnemySpinningCrystal(new Vector3(200, 0, 0), "enemySpinningNoAnim", content,new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            shootingCrystal = new EnemyShootingCrystal(new Vector3(500, 0, 0), "enemyShootingNoAnim", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));

            //fill list with enemies
            enemyList.Add(redEye);
            enemyList.Add(spinningCrystal);
            enemyList.Add(skull);
            enemyList.Add(shootingCrystal);
            #endregion

            #region Loading PowerUps
            powerup_health = new Powerup(new Vector3(10, 0, 0), "powerup_hell_health", content, new BoundingBox(Vector3.Zero, Vector3.One), "energy", 50);

            Powerup opheliaManaPowerUp;
            opheliaManaPowerUp = new Powerup(new Vector3(50, -30, 0), "powerup_hell_mana", content, new BoundingBox(new Vector3(50, -30, 0)+Vector3.Zero, new Vector3(50, -30, 0)+Vector3.One), "energy", 1);
            powerUpList.Add(opheliaManaPowerUp);
            #endregion

            //HUD
            hud = new OpheliaHUD(cont, graphics);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateSingleplayer(gameTime);

            #region HUD
            hud.Update();
            hud.UpdateMana();
            /*if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }*/
            #endregion

            //Player
            player.Update(gameTime);

            #region Updating Powerups
            foreach (Powerup p in terrainMap.PowerupList)
            {
                p.Update();
            }
            
            //Checkpoints
            foreach (Checkpoint c in terrainMap.CheckpointList)
            {
                c.Update();
            }
            foreach (Powerup powerUp in powerUpList)
            {
                powerUp.Update();
            }
            #endregion

            #region Updating Enemies
            foreach (Enemy enemy in enemyList)
            {
                enemy.UpdateSingleplayer(gameTime);
            }
            
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList.ElementAt(i).health <= 0)
                {
                    enemyList.ElementAt(i).Destroy();
                    enemyList.Remove(enemyList.ElementAt(i));
                }
            }
            #endregion



            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region Background
            spriteBatch.Begin();

            //width and height for spriteBatch rectangle needed to draw background texture
            Vector2 drawPos = new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y);
            int drawRecWidth = graphics.PreferredBackBufferWidth;
            int drawRecHeight = graphics.PreferredBackBufferHeight;
            Rectangle drawRec = new Rectangle((int)Camera2D.Position2D.X, (int)Camera2D.Position2D.Y, drawRecWidth, drawRecHeight);
            
            spriteBatch.Draw(background, drawPos, drawRec, Color.White);
            
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            //Terrain (includes Powerups and Checkpoints)
            terrainMap.Draw();

            //Player or Ghost
            if (GameControls.isGhost)
                ghost.drawIn2DWorld(new Vector3(0.05f, 0.05f, 0.05f), 0, 0, 0);
            else
                player.drawIn2DWorldWithoutBones(Vector3.One, 0, MathHelper.ToRadians(90) * player.viewingDirection.X, 0);
            player.DrawAttacks();

            foreach (Powerup powerUp in powerUpList)
            {
                powerUp.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, powerup_health.rotatedDegree, 0);
            }

            //enemies
            foreach (AIInterface enemy in enemyList)
            {
                enemy.Draw(gameTime);
            }

            //Minions etc.
            foreach (Entity laser in CR4VE.GameLogic.AI.EnemyRedEye.laserList)
            {
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(-90) * laser.viewingDirection.X);
            }
            foreach (Entity crystal in CR4VE.GameLogic.Characters.CharacterFractus.crystalList)
            {
                crystal.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
            }
            #endregion

            #region HUD
            spriteBatch.Begin();

            hud.DrawGenerals(spriteBatch);
            hud.Draw(spriteBatch);

            spriteBatch.End();
            #endregion
        }
        #endregion

        // loeschen aller grafischen Elemente
        public void Unload()
        {
            //throw new NotImplementedException();
        }

        // Freigabe der restlichen Standardressourcen
        // automatischer Aufruf des GarbageCollector beim Beenden des Spiels
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
