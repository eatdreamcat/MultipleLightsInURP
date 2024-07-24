using UnityEngine;

namespace SphericalHarmonics
{
    public static class SphericalHarmonicsUtils
    {
        private static readonly float INV_PI = 1 / 3.1416f;
        private static readonly float PI =  3.1416f;
        private static readonly float TWO_PI = 3.1416f * 2;
        
        public static float BasicConstant(int level)
        {
            switch (level)
            {
                // l = 0
                case 0:
                    return 0.5f * Mathf.Sqrt(INV_PI);
                // l = 1
                case 1:
                    return Mathf.Sqrt(3f / 4f * INV_PI);
                case 2:
                    return Mathf.Sqrt(3f / 4f * INV_PI);
                case 3:
                    return Mathf.Sqrt(3f / 4f * INV_PI);
                // l = 2
                case 4:
                    return 0.5f * Mathf.Sqrt(15f * INV_PI);
                case 5:
                    return 0.5f * Mathf.Sqrt(15f * INV_PI);
                case 6:
                    return 0.25f * Mathf.Sqrt(5f * INV_PI);
                case 7:
                    return 0.5f * Mathf.Sqrt(15f * INV_PI);
                case 8:
                    return 0.25f * Mathf.Sqrt(15f * INV_PI);
                // l = 3
                case 9:
                    return 0.25f * Mathf.Sqrt(35f / 2f * INV_PI);
                case 10:
                    return 0.5f * Mathf.Sqrt(105f * INV_PI);
                case 11:
                    return 0.25f * Mathf.Sqrt(21f / 2f * INV_PI);
                case 12:
                    return 0.25f * Mathf.Sqrt(7f * INV_PI);
                case 13:
                    return 0.25f * Mathf.Sqrt(21f / 2f * INV_PI);
                case 14:
                    return 0.25f * Mathf.Sqrt(105f * INV_PI) ;
                case 15:
                    return 0.25f * Mathf.Sqrt(35f / 2f * INV_PI);
                default:
                    return 0f;
            }
        }
        
        public static float SHBasicFull(Vector3 normal, int level)
        {
            normal.Normalize();
            float x = normal.x;
            float y = normal.y;
            float z = normal.z;
            switch (level)
            {
                // l = 0 
                case 0:
                    return 0.5f * Mathf.Sqrt(INV_PI);
                // l = 1
                case 1:
                    return Mathf.Sqrt(3f / 4f * INV_PI) * x;
                case 2:
                    return Mathf.Sqrt(3f / 4f * INV_PI) * y;
                case 3:
                    return Mathf.Sqrt(3f / 4f * INV_PI) * z;
                // l = 2
                case 4:
                    return 0.5f * Mathf.Sqrt(15f * INV_PI) * x * y;
                case 5:
                    return 0.5f * Mathf.Sqrt(15f * INV_PI) * z * y;
                case 6:
                    return 0.25f * Mathf.Sqrt(5f * INV_PI) * (z * z * 3 - 1);
                case 7:
                    return 0.5f * Mathf.Sqrt(15f * INV_PI) * z * x;
                case 8:
                    return 0.25f * Mathf.Sqrt(15f * INV_PI) * (x * x - y * y);
                // l = 3
                case 9:
                    return 0.25f * Mathf.Sqrt(35f / 2f * INV_PI) * (3 * x * x * y - y * y * y);
                case 10:
                    return 0.5f * Mathf.Sqrt(105f * INV_PI) * x * y * z;
                case 11:
                    return 0.25f * Mathf.Sqrt(21f / 2f * INV_PI) * (5 * z * z * y - y);
                case 12:
                    return 0.25f * Mathf.Sqrt(7f * INV_PI) * 5 * z * z * z - 3 * z;
                case 13:
                    return 0.25f * Mathf.Sqrt(21f / 2f * INV_PI) * (5 * z * z * x - x);
                case 14:
                    return 0.25f * Mathf.Sqrt(105f * INV_PI) * (x * x * z - y * y * z);
                case 15:
                    return 0.25f * Mathf.Sqrt(35f / 2f * INV_PI) * (x * x * x- 3 * y * y * x);
                default:
                    return 0f;
            }
        }
        
        public static Vector3 SampleSphereUniform(float u1, float u2, Vector3 sphereCenter)
        {
            float phi      = TWO_PI * u2;
            float cosTheta = 1.0f - 2.0f * u1;
            return new Vector3(
                Mathf.Cos(phi) + sphereCenter.x,
                Mathf.Sin(phi) + sphereCenter.y,
                cosTheta + sphereCenter.z
                );
        }
        
        public static Vector3 GetCosineWeightedRandomDirection(float u1, float u2, Vector3 normalWS, Vector3 originWS, out float pdf)
        {
            normalWS.Normalize();
            var positionInSphere = SampleSphereUniform(u1, u2, originWS + normalWS);
            var biasVector = positionInSphere - originWS;
            biasVector.Normalize();
            var sampleDir = (normalWS + biasVector).normalized;
            pdf = INV_PI * Mathf.Max(0, Vector3.Dot(sampleDir, normalWS)); 
            return sampleDir;
        }
    }
}