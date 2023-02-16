using KataNeo.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KataNeo
{
    public static class MonoHelp
    {
        public static GameWindow GameWindow { get; set; }
        public static ContentManager Content { get; set; }
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
            return prevState.IsKeyUp(key) && curState.IsKeyDown(key);
        }

        /// <summary>
        /// Gets if the button was pressed
        /// </summary>
        /// <param name="playerIndex">Controller index</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was pressed</returns>
        public static bool GetButtonDown(PlayerIndex playerIndex, Buttons button)
        {
            return prevGamePadStates[(int)playerIndex].IsButtonUp(button) && gamePadStates[(int)playerIndex].IsButtonDown(button);
        }

        /// <summary>
        /// Gets if the button was pressed
        /// </summary>
        /// <param name="controlType">Controller index (in controltype, used for players)</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was pressed</returns>
        public static bool GetButtonDown(ControlType controlType, Buttons button)
        {
            return prevGamePadStates[(int)controlType - 1].IsButtonUp(button) && gamePadStates[(int)controlType - 1].IsButtonDown(button);
        }

        /// <summary>
        /// Gets if the key was released
        /// </summary>
        /// <param name="key">Key to get</param>
        /// <returns>If the key was released</returns>
        public static bool GetKeyUp(Keys key)
        {
            return prevState.IsKeyDown(key) && curState.IsKeyUp(key);
        }

        /// <summary>
        /// Gets if the button was released
        /// </summary>
        /// <param name="playerIndex">Controller index</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was released</returns>
        public static bool GetButtonUp(PlayerIndex playerIndex, Buttons button)
        {
            return prevGamePadStates[(int)playerIndex].IsButtonDown(button) && gamePadStates[(int)playerIndex].IsButtonUp(button);
        }

        /// <summary>
        /// Gets if the button was released
        /// </summary>
        /// <param name="controlType">Controller index (in controltype, used for players)</param>
        /// <param name="button">Button to get</param>
        /// <returns>If the button was released</returns>
        public static bool GetButtonUp(ControlType controlType, Buttons button)
        {
            return prevGamePadStates[(int)controlType - 1].IsButtonDown(button) && gamePadStates[(int)controlType - 1].IsButtonUp(button);
        }

        /// <summary>
        /// Gets the specified axis value
        /// </summary>
        /// <param name="axisType">Type of axis to get</param>
        /// <returns>A float between -1 and 1</returns>
        /// <exception cref="System.Exception">Thrown when an incorrect AxisType is passed</exception>
        public static float GetAxis(AxisType axisType)
        {
            switch (axisType)
            {
                case AxisType.HorizontalKeyboard:
                    {
                        float vec = 0;
                        if (GetKey(Keys.D)) vec += 1f;
                        if (GetKey(Keys.A)) vec -= 1f;
                        return vec;
                    }
                case AxisType.VerticalKeyboard:
                    {
                        float vec = 0;
                        if (GetKey(Keys.W)) vec += 1f;
                        if (GetKey(Keys.S)) vec -= 1f;
                        return vec;
                    }
                default:
                    throw new System.Exception("Wrong enum");
            }
        }

        /// <summary>
        /// Gets the specified axis value
        /// </summary>
        /// <param name="axisType">Type of axis to get</param>
        /// <param name="controlType">Use it when getting a gamepad axis</param>
        /// <returns>A float between -1 and 1</returns>
        public static float GetAxis(AxisType axisType, ControlType controlType)
        {
            switch (axisType)
            {
                case AxisType.HorizontalKeyboard:
                {
                    float vec = 0;
                    if (GetKey(Keys.D)) vec += 1f;
                    if (GetKey(Keys.A)) vec -= 1f;
                    return vec;
                }
                case AxisType.VerticalKeyboard:
                {
                    float vec = 0;
                    if (GetKey(Keys.W)) vec += 1f;
                    if (GetKey(Keys.S)) vec -= 1f;
                    return vec;
                }
                case AxisType.GamePadLeftHorizontal:
                    return gamePadStates[(int)controlType - 1].ThumbSticks.Left.X;
                case AxisType.GamePadLeftVertical:
                    return gamePadStates[(int)controlType - 1].ThumbSticks.Left.Y;
                case AxisType.GamePadRightHorizontal:
                    return gamePadStates[(int)controlType - 1].ThumbSticks.Right.X;
                case AxisType.GamePadRightVertical:
                    return gamePadStates[(int)controlType - 1].ThumbSticks.Right.Y;
                default:
                    return 0f;
            }
        }
        #endregion

        //TODO in this region
        #region Content Loading Helpers
        /// <summary>
        /// Load all content from the specified folder
        /// </summary>
        /// <typeparam name="T">Type of the content to load</typeparam>
        /// <param name="path">Path to the folder</param>
        /// <param name="recursive">Check the subfolders?</param>
        /// <returns></returns>
        public static T[] LoadAllContent<T>(string path, bool recursive)
        {
            List<T> contents = new List<T>();
            foreach (var content in Directory.GetFiles(path))
            {
                var subString = content.Remove(0, 8);
                subString = subString.Remove(subString.Length - 4);
                contents.Add(Content.Load<T>(subString));
            }
            if (recursive)
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    LoadAllContent<T>(dir, true);
                }
            }
            return contents.ToArray();
        }

        /// <summary>
        /// Load all content from the specified folder
        /// </summary>
        /// <typeparam name="T">Type of the content to load</typeparam>
        /// <param name="path">Path to the folder</param>
        /// <param name="recursive">Check the subfolders?</param>
        /// <param name="returnpath">Return relative paths to contents?</param>
        /// <returns>List of lists of content inside each folder and directories to them</returns>
        public static (T[][], string[]) LoadAllContent<T>(string path, bool recursive, bool returnpath)
        {
            if (!path.Contains("Content/")) path = "Content/" + path;
            List<T[]> contents = new List<T[]>();
            List<string> paths = new List<string>();
            List<T> rootContents = new List<T>();
            foreach (var content in Directory.GetFiles(path))
            {
                var subString = content.Remove(0, 8);
                subString = subString.Remove(subString.Length - 4);
                rootContents.Add(Content.Load<T>(subString));
            }
            if (rootContents.Count > 0)
            {
                contents.Add(rootContents.ToArray());
                paths.Add(path);
            }

            if (recursive)
            {
                //TODO: Fix the returning, it won't work properly with subfolder depth greater than 1
                foreach (var dir in Directory.GetDirectories(path))
                {
                    T[] data = LoadAllContent<T>(dir, true);
                    if (data.Length > 0)
                    {
                        contents.Add(data);
                        paths.Add(dir);
                    }
                }
            }
            return (contents.ToArray(), paths.ToArray());
        }

        /// <summary>
        /// Get all animations from specified folder and its subdirectories
        /// </summary>
        /// <returns>Animation data</returns>
        /// <exception cref="System.Exception"></exception>
        public static AnimData GetAllAnims(string path)
        {
            //Get all of the animations with directories to them
            var sheets = MonoHelp.LoadAllContent<Texture2D>(path, true, true);
            List<Anim> anims = new List<Anim>();
            //Add all of the animations and get the intervals from their directories
            for (int i = 0; i < sheets.Item1.Length; i++)
            {
                anims.Add(new Anim(sheets.Item1[i], GetIntervals(sheets.Item2[i])));
            }
            if (anims.Count == 0) throw new System.Exception("There are no animations in directory");
            //Shorten keywords for easier animation referencing
            List<string> keys = new List<string>();
            foreach (var dir in sheets.Item2)
            {
                int slash = dir.LastIndexOf('/');
                int backslash = dir.LastIndexOf('\\');
                keys.Add(backslash > slash ? dir.Remove(0, backslash + 1) : dir.Remove(0, slash + 1));
            }
            return new AnimData(anims.ToArray(), keys.ToArray());
        }

        private static float[] GetIntervals(string path)
        {
            return JsonSerializer.Deserialize<float[]>(File.ReadAllText("Data/Animations" + path.Remove(0, 7) + "/intervals.json"));
        }
        #endregion
    }

    public enum AxisType
    {
        /// <summary>
        /// Horizontal axis for the keyboard. D is positive and A is negative
        /// </summary>
        HorizontalKeyboard,
        /// <summary>
        /// Vertical axis for the keyboard. W is positive and S is negative
        /// </summary>
        VerticalKeyboard,
        /// <summary>
        /// Horizontal axis of the left gamepad stick
        /// </summary>
        GamePadLeftHorizontal,
        /// <summary>
        /// Vertical axis of the left gamepad stick
        /// </summary>
        GamePadLeftVertical,
        /// <summary>
        /// Horizontal axis of the right gamepad stick
        /// </summary>
        GamePadRightHorizontal,
        /// <summary>
        /// Vertical axis of the right gamepad stick
        /// </summary>
        GamePadRightVertical,
    }
}