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

        //Terrain
        public static Tilemap[] tileMaps = new Tilemap[]{};

        public static Tilemap activeTileMap1;
        public static Tilemap activeTileMap2;
        public static int activeIndex1 = 0;
        public static int activeIndex2 = 1;

        public static List<Tile> visibles;

        //Player
        public static Character ghost;
        public static Character player;

        //reset point if dead
        public static Checkpoint lastCheckpoint;
        
        //Enemies
        public static List<Enemy> enemyList = new List<Enemy>();

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
            int[,] layout1 = new int[,] {
        
           {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,96, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0},
           {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 3, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
           {1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0,98, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 0, 3, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 3, 4, 4, 3, 3, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0},
           {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0},
           {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0},
           {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 3, 4, 4, 3, 4, 4, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0}};

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
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 0, 0, 0, 9, 0, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 0, 0, 0, 9, 0, 0, 0, 9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0,13, 0, 0, 0, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 0, 0,13,13, 0, 0,13, 0, 0},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 0,13,13,13,13,13,13,13,13},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9,13},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9,13},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9,13},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9},
            {9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9}};



            int[,] layout4 = new int[,] { 
            {0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
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
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,13,13, 0, 0, 0, 0,13, 0, 0, 0,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14, 0, 0, 0, 0, 0, 0,13, 0,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14,14, 0, 0, 0, 0,13,13, 0, 0,13,13, 0, 0,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14,14,14, 0, 0,13,13,13, 0, 0, 0, 0, 0, 0,13,13},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14,14,14,14,13,13,13,13,13, 0, 0, 0, 0, 0,13,14},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14,14,14,14,14,13,13,13,13,13,13,13,14,14,14,14},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14,14,14,14,14,14,13,13,13,13,13,13,13,13,14,14},
           {13,13,13,13,13,13,13,13,13,14,14,13,13,14,14,14,14,13,13,13,14,14,13,13,13,13,13,14,14,13,13,13,13,13,13,14,14,14,14,14,14,14,14,14,14,14,13,13,13,13,13,13,13,13,13,14}};


            int[,] layout5 = new int[,] { 
          
           {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           { 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0,13,13,13,13,13, 0, 0, 0, 0,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           { 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0,13,13,13, 0, 0, 0, 0, 0,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13},
           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13},
           {13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           {13,13,13, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0,10, 0, 0, 0,10,10, 0, 0, 0,10, 0, 0, 0,10,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10,10,10, 0, 0, 0, 0,10, 0, 0, 0, 0},
           {13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,10,10, 0, 0,10,10, 0, 0, 0, 0, 0, 0, 0,10,10,10,10, 0, 0, 0},
           {14,13,13,13,13, 0,10, 0, 0,10,10, 0, 0, 0, 0, 0, 0,10, 0, 0, 0, 0, 0, 0, 0, 0,10,10, 0, 0, 0,10, 0, 0,10 ,0, 0,10, 0, 0, 0, 0,0, 0, 0, 0,14,14,14,10,10,10,10,14,14,14},
           {14,14,13,14,14,14,10, 0,10,10,14,14,14,14,14,14,14,10,14,14,14,14,14,14,14,14,10,10,14,14,14,10,14,14,10,10 ,0,10,10,10,10,10, 0,10, 0,14,14,14,14,10,10,10,10,14,14,14},
           {14,14,14,14,14,14,10, 0, 0,10,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,10,10, 0, 0, 0, 0, 0, 0, 0,10,14,14,14,14,14,10,10,10,10,14,14,14},
           {14,14,14,14,14,14,10,10, 0,10,10,10,10,10,10,10,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,10,10,10,10,10,10,10,10,10,10,14,14,14,14,14,10,10,10,10,14,14,14},
           {14,14,14,14,14,14,14,10, 0, 0, 0, 0, 0, 0, 0, 0,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,10,10,10,10,14,14,14,14,14,14,14,10,10,10,10,14,14,14},
           {14,14,14,14,14,14,14,10,10,10,10, 0, 0,10, 0,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,10,10,10,10,14,14,14,14,14,14,14,10,10,10,10,14,14,14},
           {14,14,14,14,14,14,14,14,14,14,10,10,10,10,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,10,10,10,10,14,14,14,14,14,14,14,10,10,10,10,14,14,14},
           {14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,10,10,10,10,14,14,14}};


            int[,] layout6 = new int[,] { 
   
           { 13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {  0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
           {  0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13},
           {  0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13},
           {  0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 10, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 10,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0,13,13, 0,13,13,13,13,13,13},
           { 10,10,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 12,12,12,12, 0, 0,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 12,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13, 0,13,13,13,13,13},
           { 12, 0, 0,12,12, 0, 0, 0, 0,12,12,12,12, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 12, 0,12,12,12,12,12,12,12,12,12,12,12,12, 0,12,12,12,12,12,12, 0, 0,12,12, 0, 0, 0, 0, 0,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 12,12,12,12,12,12,12,12,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0,12,12,12,12,12,12,12, 0,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0,13,13,13,13,13,13},
           { 11,11,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12,12,12,12, 0, 0,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 11,11,11,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12,12,12,12,12,12,12,12,12, 0,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 11,11,11,11,11,11, 0, 0, 12,12,12,0, 0, 0, 0,12,12,12,12,12,12,12,12,12, 0, 0, 0,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13, 0,13,13,13,13,13},
           { 11,11,11,11,11,11,11, 0, 0, 0,12,12,12, 0, 0, 0, 0,12,12,12,12,12, 0, 0, 0, 0,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13,13},
           { 11,11,11,11,11,11,11,11, 0, 0, 0, 0, 0, 0,11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0,13,13,13,13, 0},
           { 11,11,11,11,11,11,11,11,11, 0, 0, 0, 0,11,11,11, 0, 0, 0, 0, 0, 0, 0,12,12,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0},
           { 12,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11, 0, 0,12,12,12,12,12,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0},
           { 12,12,12,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,12,12,12,12,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0},
           { 12,12,12,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,12,12,12,12,12,12,12,12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13, 0, 0, 0, 0, 0, 0, 0}};


            int[,] layout7 = new int[,] { 
  
            { 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0, 0,13,13,13, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13},
            { 0, 0,13, 0, 0, 0, 0, 0,19,19, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13, 0, 0, 0, 0, 0, 0, 0, 0,19, 0, 0,19, 0, 0, 0, 0, 0, 0, 0,19, 0, 0, 0, 0, 0, 0,13,13,13,13,13},
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

            { 0,19,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            { 0,19,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,19,19,19,19,19},
            { 0,19,19,19,19,19,13,13,13,13,13,13,13,13,19,19,19,19,19,19,13,19,19,19,19,19,19,19,19,19,19,19,19,13,13,13,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0,19,19,19,19,19,19,19,19,19,19, 0, 0, 0, 0,19,19,19, 0, 0, 0, 0, 0, 0, 0, 0,19,19,19,19,19,19,19, 0, 0, 0, 0,19,19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19, 0,19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19, 0,15,15},
            { 0, 0,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,19,19, 0, 0, 0, 0, 0, 0, 0,19,19,19, 0, 0,19, 0, 0, 0, 0, 0, 0, 0, 0,15, 0, 0, 0, 0,15, 0, 0, 0, 0, 0,19, 0, 0,16,18,18},
            { 0,16,18, 0,15,15, 0, 0, 0,15, 0, 0, 0,15, 0, 0, 0, 0, 0,15, 0, 0,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0,15,15, 0, 0, 0,15,18,15, 0, 0,15,18, 0, 0, 0,15, 0, 0, 0,16,16,18,18},
            {16,16,18, 0,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18},
            {16,16,18,15,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18},
            {16,16,18,18,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,18,18}};

            //background layouts
            int[,] layout9 = new int[,] {
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
            {13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13}};







            int boxSize = 10;


            //Array of all generated Tilemaps
            tileMaps = new Tilemap[]
            {
                Tilemap.Generate(layout1, boxSize, new Vector3(-80, 0, 0)),
                Tilemap.Generate(layout2, boxSize, new Vector3(510, 10, 0)),
                Tilemap.Generate(layout3, boxSize, new Vector3(1030, -60, 0)),
                Tilemap.Generate(layout4, boxSize, new Vector3(1550, -150, 0)),
                Tilemap.Generate(layout5, boxSize, new Vector3(2110, -280, 0)),
                Tilemap.Generate(layout6, boxSize, new Vector3(2670, -290, 0)),
                Tilemap.Generate(layout7, boxSize, new Vector3(3110, -560, 0)),
                Tilemap.Generate(layout8, boxSize, new Vector3(3660, 0, -590)),
                Tilemap.Generate(layout9, boxSize, new Vector3(3110,-560,0)),
            };

            //indices of active Tilemaps
            activeTileMap1 = tileMaps[activeIndex1];
            activeTileMap2 = tileMaps[activeIndex2];

            visibles = new List<Tile>();
            #endregion            

            //Camera
            Camera2D.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            //SaveGame
            SaveGame.Reset();

            //Textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");

            //Player
            ghost = new Character(Vector3.Zero, "skull", content);
            player = new CharacterOphelia(new Vector3(0,0,5), "Ophelia", content, new BoundingBox(new Vector3(-2.5f, -9f, -2.5f), new Vector3(2.5f, 9f, 2.5f)));
            
            //Checkpoints (default = Startposition)
            lastCheckpoint = new Checkpoint(Vector3.Zero, "checkpoint_hell", content);
            
            #region Loading AI
            EnemyRedEye redEye;
            EnemySkull skull;
            //EnemySpinningCrystal spinningCrystal;
            //EnemyShootingCrystal shootingCrystal;

            redEye = new EnemyRedEye(new Vector3(80, 0, 0),"EnemyEye",content,new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            skull = new EnemySkull(new Vector3(400, 0, 0), "skull", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            //spinningCrystal = new EnemySpinningCrystal(new Vector3(200, 0, 0), "enemySpinningNoAnim", content,new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            //shootingCrystal = new EnemyShootingCrystal(new Vector3(500, 0, 0), "enemyShootingNoAnim", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));

            //fill list with enemies
            enemyList.Add(redEye);
            enemyList.Add(skull);
            #endregion

            //HUD
            hud = new OpheliaHUD(cont, graphics);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            //Viewport Culling
            visibles = Tilemap.getVisibleTiles();

            GameControls.updateSingleplayer(gameTime, visibles);

            //check if active Tilemaps have changed
            Tilemap.updateActiveTilemaps();

            #region HUD
            hud.Update();
            
            hud.UpdateMana();
            hud.UpdateLiquidPositionByResolution();
            
            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }
            #endregion

            //Player
            player.Update(gameTime);

            //Powerups
            foreach (Powerup p in tileMaps[activeIndex1].PowerupList)
            {
                p.Update();
            }
            foreach (Powerup p in tileMaps[activeIndex2].PowerupList)
            {
                p.Update();
            }
            
            //Checkpoints
            foreach (Checkpoint c in tileMaps[activeIndex1].CheckpointList)
            {
                c.Update();
            }
            foreach (Checkpoint c in tileMaps[activeIndex2].CheckpointList)
            {
                c.Update();
            }

            #region Enemies
            foreach (Enemy e in tileMaps[activeIndex1].EnemyList)
            {
                e.UpdateSingleplayer(gameTime);
            }
            foreach (Enemy e in tileMaps[activeIndex2].EnemyList)
            {
                e.UpdateSingleplayer(gameTime);
            }

            //remove dead enemies from active lists
            for (int i = 0; i < tileMaps[activeIndex1].EnemyList.Count; i++)
            {
                if (tileMaps[activeIndex1].EnemyList.ElementAt(i).isDead)
                {
                    tileMaps[activeIndex1].EnemyList.ElementAt(i).Destroy();
                    tileMaps[activeIndex1].EnemyList.Remove(tileMaps[activeIndex1].EnemyList.ElementAt(i));
                }
            }
            for (int i = 0; i < tileMaps[activeIndex2].EnemyList.Count; i++)
            {
                if (tileMaps[activeIndex2].EnemyList.ElementAt(i).isDead)
                {
                    tileMaps[activeIndex2].EnemyList.ElementAt(i).Destroy();
                    tileMaps[activeIndex2].EnemyList.Remove(tileMaps[activeIndex2].EnemyList.ElementAt(i));
                }
            }
            #endregion

            GameControls.updateVibration(gameTime);

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
            tileMaps[activeIndex1].Draw(visibles);
            tileMaps[activeIndex2].Draw(visibles);

            //Player or Ghost
            if (GameControls.isGhost)
                ghost.drawIn2DWorld(new Vector3(0.05f, 0.05f, 0.05f), 0, 0, 0);
            else
            {
                player.drawIn2DWorld(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) * player.viewingDirection.X, 0);
                player.DrawAttacks();
            }

            #region Enemies and Minions
            foreach (Enemy e in tileMaps[activeIndex1].EnemyList)
            {
                e.Draw();
            }
            foreach (Enemy e in tileMaps[activeIndex2].EnemyList)
            {
                e.Draw();
            }

            //Minions etc.
            foreach (Entity laser in EnemyRedEye.laserList)
            {
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(-90) * laser.viewingDirection.X);
            }
            foreach (Entity crystal in CharacterFractus.crystalList)
            {
                crystal.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
            }
            #endregion
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
