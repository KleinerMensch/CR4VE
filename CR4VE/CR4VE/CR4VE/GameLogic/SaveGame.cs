using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic
{
    public static class SaveGame
    {
        #region Attributes
        private static Vector3 resetPosCrystal;
        private static Vector3 resetPosHell;
        private static bool fractusUnlocked;
        private static bool seraphinUnlocked;
        #endregion

        #region Properties
        public static Vector3 ResetPositionCrystal
        {
            get { return resetPosCrystal; }
        }
        public static Vector3 ResetPositionHell
        {
            get { return resetPosHell; }
        }
        public static bool Fractus
        {
            get { return fractusUnlocked; }
        }
        public static bool Seraphin
        {
            get { return seraphinUnlocked; }
        }
        #endregion

        #region Methods
        public static void Load()
        {
            string[] lines = System.IO.File.ReadAllLines("Content/Assets/Text/SaveGame.txt");
            
            //reset point in Crystal Level (lines 1-2)
            resetPosCrystal.X = Convert.ToSingle(lines[1]);
            resetPosCrystal.Y = Convert.ToSingle(lines[2]);
            resetPosCrystal.Z = 0;

            //reset point in Hell Level (lines 3-4)
            resetPosHell.X = Convert.ToSingle(lines[4]);
            resetPosHell.Y = Convert.ToSingle(lines[5]);
            resetPosHell.Z = 0;

            //check unlocked content
            fractusUnlocked = Convert.ToBoolean(lines[7]);
            seraphinUnlocked = Convert.ToBoolean(lines[9]);
        }

        public static void Reset()
        {
            //default values
            String[] resetedLines = { "Reset Crystal:", "0", "0",
                                    "Reset Hell:", "0", "0",
                                    "Fractus:", "false",
                                    "Seraphin:", "false"};

            System.IO.File.WriteAllLines("Content/Assets/Text/SaveGame.txt", resetedLines);

            Load();
        }

        public static void setCrystalReset(Vector3 resetPos)
        {
            string[] lines = System.IO.File.ReadAllLines("Content/Assets/Text/SaveGame.txt");

            String crystalX = Convert.ToString(resetPos.X);
            String crystalY = Convert.ToString(resetPos.Y);

            String hellX = lines[4];
            String hellY = lines[5];

            String fractus = lines[7];
            String seraphin = lines[9];

            String[] refreshedLines = { "Reset Crystal:", crystalX, crystalY,
                                    "Reset Hell:", hellX, hellY,
                                    "Fractus:", fractus,
                                    "Seraphin:", seraphin};

            System.IO.File.WriteAllLines("Content/Assets/Text/SaveGame.txt", refreshedLines);

            Load();
        }
        public static void setHellReset(Vector3 resetPos)
        {
            string[] lines = System.IO.File.ReadAllLines("Content/Assets/Text/SaveGame.txt");

            String crystalX = lines[1];
            String crystalY = lines[2];

            String hellX = Convert.ToString(resetPos.X);
            String hellY = Convert.ToString(resetPos.Y);

            String fractus = lines[7];
            String seraphin = lines[9];

            String[] refreshedLines = { "Reset Crystal:", crystalX, crystalY,
                                    "Reset Hell:", hellX, hellY,
                                    "Fractus:", fractus,
                                    "Seraphin:", seraphin};

            System.IO.File.WriteAllLines("Content/Assets/Text/SaveGame.txt", refreshedLines);

            Load();
        }
        public static void setFractusUnlock(bool state)
        {
            string[] lines = System.IO.File.ReadAllLines("Content/Assets/Text/SaveGame.txt");

            String crystalX = lines[1];
            String crystalY = lines[2];

            String hellX = lines[4];
            String hellY = lines[5];

            String fractus = Convert.ToString(state);
            String seraphin = lines[9];

            String[] refreshedLines = { "Reset Crystal:", crystalX, crystalY,
                                    "Reset Hell:", hellX, hellY,
                                    "Fractus:", fractus,
                                    "Seraphin:", seraphin};

            System.IO.File.WriteAllLines("Content/Assets/Text/SaveGame.txt", refreshedLines);

            Load();
        }
        public static void setSeraphinUnlock(bool state)
        {
            string[] lines = System.IO.File.ReadAllLines("Content/Assets/Text/SaveGame.txt");

            String crystalX = lines[1];
            String crystalY = lines[2];

            String hellX = lines[4];
            String hellY = lines[5];

            String fractus = lines[7];
            String seraphin = Convert.ToString(state);

            String[] refreshedLines = { "Reset Crystal:", crystalX, crystalY,
                                    "Reset Hell:", hellX, hellY,
                                    "Fractus:", fractus,
                                    "Seraphin:", seraphin};

            System.IO.File.WriteAllLines("Content/Assets/Text/SaveGame.txt", refreshedLines);

            Load();
        }
        #endregion
    }
}
