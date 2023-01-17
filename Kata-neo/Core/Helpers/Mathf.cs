using Microsoft.Xna.Framework;

namespace KataNeo
{
    //Helper math class
    public static class Mathf
    {
        #region Clamp
        /// <summary>
        /// Clamp the value between two values
        /// </summary>
        /// <param name="val">Value to clamp</param>
        /// <param name="min">Minimal value of val</param>
        /// <param name="max">Maximum value of val</param>
        /// <returns>Clamped value</returns>
        public static int Clamp(int val, int min, int max)
        {
            if (val < min) val = min;
            else if (val > max) val = max;
            return val;
        }

        /// <summary>
        /// Clamp the value between two values
        /// </summary>
        /// <param name="val">Value to clamp</param>
        /// <param name="min">Minimal value of val</param>
        /// <param name="max">Maximum value of val</param>
        /// <returns>Clamped value</returns>
        public static float Clamp(float val, float min, float max)
        {
            if (val < min) val = min;
            else if (val > max) val = max;
            return val;
        }
        #endregion

        #region Lerp
        /// <summary>
        /// Linearly interpolate between two values
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="t">Lerp percentage</param>
        /// <returns>Interpolated value</returns>
        public static float Lerp(float a, float b, float t)
        {
            if (t == 0) return a;
            else if (t == 1) return b;
            return a + (b - a) * t;
        }

        /// <summary>
        /// Linearly interpolate between two values
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="t">Lerp percentage</param>
        /// <returns>Interpolated value</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            if (t == 0) return a;
            else if (t == 1) return b;
            return new Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }
        #endregion
    }
}