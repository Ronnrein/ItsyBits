using System;

namespace ItsyBits.Helpers {

    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions {

        /// <summary>
        /// Clamp value between two other values
        /// </summary>
        /// <typeparam name="T">Type that implments IComparable</typeparam>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <returns>Clamped value</returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        } 

    }
}