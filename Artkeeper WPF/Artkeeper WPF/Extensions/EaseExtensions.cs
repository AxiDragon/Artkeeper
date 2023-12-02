using System;

namespace Artkeeper.Extensions
{
    public static class EaseExtensions
    {
        /// <summary>
        /// Returns a float between 0 and 1, representing the progress of the ease in sine function.
        /// </summary>
        /// <param name="t">The absolute progress in the bounds of 0 (start) and 1 (end).</param>
        public static float ReturnEaseInSine(this float t)
        {
            return (float)(1f - Math.Cos(t * Math.PI / 2f));
        }
    }
}
