using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace KataNeo
{
    public static class MonoHelp
    {
        #region Input Helpers
        public static KeyboardState prevState;
        public static KeyboardState curState;
        public static GamePadState[] prevGamePadStates;
        public static GamePadState[] gamePadStates;

        /// <summary>
        /// Get keyboard key press
        /// </summary>
        /// <param name="key">Key to get</param>
        /// <returns>If the key is currently held down</returns>
        public static bool GetKey(Keys key)
        {
            return curState.IsKeyDown(key);
        }

        /// <summary>
        /// Get controller button press
        /// </summary>
        /// <param name="playerIndex">Controller index</param>
        /// <param name="button">Button to get</param>
        /// <returns>If button is held down</returns>
        public static bool GetButton(PlayerIndex playerIndex, Buttons button)
        {
            return gamePadStates[(int)playerIndex].IsButtonDown(button);
        }

        /// <summary>
        /// Get controller button press
        /// </summary>
        /// <param name="controlType">Controller index (in controltype, used for players)</param>
        /// <param name="button">Button to get</param>
        /// <returns>If button is held down</returns>
        public static bool GetButton(ControlType controlType, Buttons button)
        {
            if (controlType == ControlType.Keyboard) throw new System.Exception("Keyboard type can't be used in this function");
            else return gamePadStates[(int)controlType - 1].IsButtonDown(button);
        }

        /// <summary>
        /// Gets if the key was pressed
        /// </summary>
        /// <param name="key">Key to get</param>
        /// <returns>If the key was pressed</returns>
        public static bool GetKeyDown(Keys key)
        {
            if (prevState.IsKeyUp(key) && curState.IsKeyDown(key)) return true;
            else return false;
        }

        /// <summary>
        /// Gets if the button was pressed
        /// </summary>
        /// <param name="playerIndex">Controller index</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was pressed</returns>
        public static bool GetButtonDown(PlayerIndex playerIndex, Buttons button)
        {
            if (prevGamePadStates[(int)playerIndex].IsButtonUp(button) && gamePadStates[(int)playerIndex].IsButtonDown(button)) return true;
            else return false;
        }

        /// <summary>
        /// Gets if the button was pressed
        /// </summary>
        /// <param name="controlType">Controller index (in controltype, used for players)</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was pressed</returns>
        public static bool GetButtonDown(ControlType controlType, Buttons button)
        {
            if (prevGamePadStates[(int)controlType - 1].IsButtonUp(button) && gamePadStates[(int)controlType - 1].IsButtonDown(button)) return true;
            else return false;
        }

        /// <summary>
        /// Gets if the key was released
        /// </summary>
        /// <param name="key">Key to get</param>
        /// <returns>If the key was released</returns>
        public static bool GetKeyUp(Keys key)
        {
            if (prevState.IsKeyDown(key) && curState.IsKeyUp(key)) return true;
            else return false;
        }

        /// <summary>
        /// Gets if the button was released
        /// </summary>
        /// <param name="playerIndex">Controller index</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was released</returns>
        public static bool GetButtonUp(PlayerIndex playerIndex, Buttons button)
        {
            if (prevGamePadStates[(int)playerIndex].IsButtonDown(button) && gamePadStates[(int)playerIndex].IsButtonUp(button)) return true;
            else return false;
        }

        /// <summary>
        /// Gets if the button was released
        /// </summary>
        /// <param name="controlType">Controller index (in controltype, used for players)</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was released</returns>
        public static bool GetButtonUp(ControlType controlType, Buttons button)
        {
            if (prevGamePadStates[(int)controlType - 1].IsButtonDown(button) && gamePadStates[(int)controlType - 1].IsButtonUp(button)) return true;
            else return false;
        }
        #endregion

        //TODO in this region
        #region Content Loading Helpers
        /// <summary>
        /// Load all content from the specified folder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameWindow">GameWindow to load the content with</param>
        /// <param name="path">Path to the folder</param>
        /// <param name="recursive">Check the subfolders?</param>
        /// <returns></returns>
        public static T[] LoadAllContent<T>(GameWindow gameWindow, string path, bool recursive)
        {
            List<T> contents = new List<T>();
            foreach (var content in Directory.GetFiles(path))
            {
                var subString = content.Remove(0, 8);
                subString = subString.Remove(subString.Length - 4);
                contents.Add(gameWindow.Content.Load<T>(subString));
            }
            if (recursive)
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    LoadAllContent<T>(gameWindow, dir, true);
                }
            }
            return contents.ToArray();
        }

        /// <summary>
        /// Load all content from the specified folder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameWindow">GameWindow to load the content with</param>
        /// <param name="path">Path to the folder</param>
        /// <param name="recursive">Check the subfolders?</param>
        /// <param name="returnpath">Return relative paths to contents?</param>
        /// <returns>List of lists of content inside each folder and directories to them</returns>
        public static (T[][], string[]) LoadAllContent<T>(GameWindow gameWindow, string path, bool recursive, bool returnpath)
        {
            if (!path.Contains("Content/")) path = "Content/" + path;
            List<T[]> contents = new List<T[]>();
            List<string> paths = new List<string>();
            List<T> rootContents = new List<T>();
            foreach (var content in Directory.GetFiles(path))
            {
                var subString = content.Remove(0, 8);
                subString = subString.Remove(subString.Length - 4);
                rootContents.Add(gameWindow.Content.Load<T>(subString));
            }
            if (rootContents.Count > 0)
            {
                contents.Add(rootContents.ToArray());
                paths.Add(path);
            }

            if (recursive)
            {
                //Fix the returning cuz it probably will return badly structured lists
                foreach (var dir in Directory.GetDirectories(path))
                {
                    T[] data = LoadAllContent<T>(gameWindow, dir, true);
                    if (data.Length > 0)
                    {
                        contents.Add(data);
                        paths.Add(dir);
                    }
                }
            }
            return (contents.ToArray(), paths.ToArray());
        }
    }
    #endregion
}