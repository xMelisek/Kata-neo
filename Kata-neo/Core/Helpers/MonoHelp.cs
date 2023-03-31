using KataNeo.Animation;
using KataNeo.Aseprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace KataNeo
{
    public static class Input
    {
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

    public static class Timer
    {
        public delegate void TimerCallback();
        static float curTime;
        static List<TimerStruct> _toAdd = new List<TimerStruct>();
        static List<TimerStruct> _timers = new List<TimerStruct>();

        /// <summary>
        /// Add a timer and call the function after time passes
        /// </summary>
        /// <param name="seconds">After how many seconds should the callback fire up</param>
        /// <param name="callback">Function to callback to</param>
        public static void AddTimer(float seconds, TimerCallback callback) => _toAdd.Add(new TimerStruct(curTime + seconds, callback));

        public static void Update(GameTime gameTime)
        {
            List<TimerStruct> toDispose = new List<TimerStruct>();
            curTime = (float)gameTime.TotalGameTime.TotalSeconds;
            foreach (var timer in _timers)
            {
                if (gameTime.TotalGameTime.TotalSeconds >= timer.timerEnd)
                {
                    timer.callback();
                    toDispose.Add(timer);
                }
            }
            _timers = _timers.Except(toDispose).ToList();
            _timers.AddRange(_toAdd);
            _toAdd.Clear();
        }

        struct TimerStruct
        {
            public float timerEnd;
            public TimerCallback callback;

            public TimerStruct(float timerEnd, TimerCallback callback)
            {
                this.timerEnd = timerEnd;
                this.callback = callback;
            }
        }
    }

    public static class Content
    {
        static readonly string contentRoot = "Assets\\";

        #region Content Loading
        public static Texture2D LoadTexture2D(string path)
        {
            return Texture2D.FromFile(MonoHelp.GameWindow.GraphicsDevice, path);
        }

        /// <summary>
        /// Loads spritesheets and their metadata from the specified path
        /// </summary>
        /// <param name="path">Path to the folder containing the spritesheets and metadata</param>
        /// <returns></returns>
        public static Atlas LoadAtlas(string path)
        {
            path = contentRoot + path;
            List<Anim> anims = new List<Anim>();
            List<string> keys = new List<string>();

            string[] spritesheets = Directory.GetFiles(path, "*.png");
            string[] metadatas = Directory.GetFiles(path, "*.json");

            if (spritesheets.Length == 0)
                throw new System.Exception("There are no spritesheets inside the directory");

            if (spritesheets.Length != metadatas.Length)
                throw new System.Exception("Not enough metadata files or spritesheets. Number between the files is not equal.");

            for (int i = 0; i < spritesheets.Length; i++)
            {
                Texture2D spritesheet = LoadTexture2D(spritesheets[i]);
                //Get rects and intervals from aseprite metadata and convert them into anims
                List<Texture2D> frames = new List<Texture2D>();
                List<float> intervals = new List<float>();
                var metadata = JsonSerializer.Deserialize<AnimMetadata>(File.ReadAllText(metadatas[i]));
                foreach (var frameData in metadata.frames)
                {
                    frames.Add(MonoHelp.GetSubTexture(spritesheet, new Rectangle((int)frameData.frame.x, (int)frameData.frame.y, (int)frameData.frame.w, (int)frameData.frame.h)));
                    intervals.Add(frameData.duration * 0.001f);
                }
                anims.Add(new Anim(frames.ToArray(), intervals.ToArray()));

                keys.Add(spritesheets[i].Substring(spritesheets[i].LastIndexOf('\\') + 1, spritesheets[i].Length - spritesheets[i].LastIndexOf('\\') - 5));
            }

            return new Atlas(anims.ToArray(), keys.ToArray());
        }
        #endregion
    }

    public static class MonoHelp
    {
        public static GameWindow GameWindow { get; set; }
        public static ContentManager Content { get; set; }

        public static Texture2D GetSubTexture(Texture2D tex, Rectangle rect)
        {
            Color[] texData = new Color[tex.Width * tex.Height];
            tex.GetData(texData);

            Color[] rectData = new Color[rect.Width * rect.Height];
            for(int x = 0; x < rect.Width; x++)
                for(int y = 0; y < rect.Height; y++)
                    rectData[x + y * rect.Width] = texData[x + rect.X + (y + rect.Y) * tex.Width];
            var subTex = new Texture2D(GameWindow.GraphicsDevice, rect.Width, rect.Height);
            subTex.SetData(rectData);
            return subTex;
        }

        //TODO in this region (or not as it's obsolete for now)
        #region Monogame Content Loading Helpers
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
        public static Atlas GetAllAnims(string path)
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
            return new Atlas(anims.ToArray(), keys.ToArray());
        }

        private static float[] GetIntervals(string path)
        {
            return JsonSerializer.Deserialize<float[]>(File.ReadAllText("Data/Animations" + path.Remove(0, 7) + "/intervals.json"));
        }
        #endregion
    }
}