using UnityEngine;
namespace Bottle.Extensions.Helper
{
    public static class VectorHelper
    {
        /// <summary>
        /// Generate a random vector in a direction
        /// </summary>
        /// <param name="slope">The slope for random generation</param>
        /// <param name="direction">The direction</param>
        /// <returns>A random vector</returns>
        public static Vector3 GenerateRandomVectorInDirection(float slope, Vector3 direction)
        {
            return (new Vector3(
                Random.Range(-slope, slope), 
                Random.Range(-slope, slope), 
                Random.Range(-slope, slope)) / 100f + direction).normalized;
        }
        /// <summary>
        /// Rotate a point around an axis, the axis is the perpendicular vector of inputNormal vector and newNormal
        /// vector
        /// </summary>
        /// <param name="input">The point position</param>
        /// <param name="inputNormal">The first vector normal</param>
        /// <param name="newNormal">The second vector normal</param>
        /// <returns></returns>
        public static Vector3 RotateAPointAroundAnAxis(Vector3 input, Vector3 inputNormal, Vector3 newNormal)
        {
            float angle = Vector3.Angle(inputNormal, newNormal);
            Vector3 axis = Vector3.Cross(inputNormal, newNormal);
            Quaternion rot = Quaternion.AngleAxis(angle, axis);
            return rot * input;
        }
    }
}
