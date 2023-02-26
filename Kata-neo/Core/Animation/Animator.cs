using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KataNeo.Animation
{
    public class Animator
    {
        public delegate void UpdateTexture(Texture2D texture);
        public Anim curAnim;
        UpdateTexture callback;
        private int index = 0;

        public Animator(ref Texture2D target, Anim anim, UpdateTexture callback)
        {
            curAnim = anim;
            target = curAnim.frames[0];
            this.callback = callback;
            MonoHelp.AddTimer(curAnim.intervals[index], NextFrameCallback);
        }

        #region Playback Manipulation
        /// <summary>
        /// Starts playing the current animation
        /// </summary>
        public void Play()
        {

        }

        /// <summary>
        /// Pauses the animation on the current frame
        /// </summary>
        public void Pause()
        {

        }

        /// <summary>
        /// Resets the animation to the first frame and pauses it
        /// </summary>
        public void Stop()
        {

        }
        #endregion

        public void ChangeAnim(Anim anim)
        {
            if (anim.Equals(curAnim)) return;
            curAnim = anim;
            index = 0;
        }

        void NextFrameCallback()
        {
            if (index == curAnim.frames.Length - 1) index = 0;
            else index++;
            callback(curAnim.frames[index]);
            MonoHelp.AddTimer(curAnim.intervals[index], NextFrameCallback);
        }
    }

    public class Anim
    {
        public Texture2D[] frames;
        public float[] intervals;

        public Anim(Texture2D[] frames, float[] intervals)
        {
            this.frames = frames;
            this.intervals = intervals;
        }
    }

    public class AnimData
    {
        public Anim[] anims;
        public string[] keys;

        public AnimData(Anim[] anims, string[] keys)
        {
            this.anims = anims;
            this.keys = keys;
        }

        public Anim GetAnim(int index)
        {
            try
            {
                return anims[index];
            }
            catch (System.IndexOutOfRangeException)
            {
                throw new System.IndexOutOfRangeException("Specified index was out of range");
            }
        }

        public Anim GetAnim(string key)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].Equals(key)) return anims[i];
            }
            throw new System.Exception("No animation with specified key");
        }
    }
}