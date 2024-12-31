using UnityEngine;

namespace SansyHuman.TWHG.Core
{
    /// <summary>
    /// Math library.
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// Returns the smallest integer greater to or equal to each component of the vector.
        /// </summary>
        public static Vector3Int Ceil(Vector3 vector)
        {
            return new Vector3Int(
                Mathf.CeilToInt(vector.x),
                Mathf.CeilToInt(vector.y),
                Mathf.CeilToInt(vector.z));
        }

        /// <summary>
        /// Returns the largest integer smaller than or equal to each component of the vector.
        /// </summary>
        public static Vector3Int Floor(Vector3 vector)
        {
            return new Vector3Int(
                Mathf.FloorToInt(vector.x),
                Mathf.FloorToInt(vector.y),
                Mathf.FloorToInt(vector.z));
        }
    }
}
