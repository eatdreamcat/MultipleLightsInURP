using System;
using System.Collections;
using System.Collections.Generic;
using SphericalHarmonics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SphericalHarmonicLighting : MonoBehaviour
{
    public bool debugCosineWeightSample = false;
    [Range(0, 128)]
    public int sampleCount = 0;

    public Vector3 debugNormal;
    [Range(0.5f, 5f)]
    public float RayLength = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        SHLightingRenderFeature.SphericalHarmonicLightingRenderPass.s_Center = transform;
    }

    private void OnDestroy()
    {
        SHLightingRenderFeature.SphericalHarmonicLightingRenderPass.s_Center = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (debugCosineWeightSample)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, debugNormal * RayLength);
            Gizmos.color = Color.green;
            for (int i = 0; i < sampleCount; i++)
            {
                var dir = SphericalHarmonicsUtils.GetCosineWeightedRandomDirection(
                    Random.value, Random.value, debugNormal, transform.position, out var _);
                Gizmos.DrawRay(transform.position, dir * RayLength);
            }
            Gizmos.color = oldColor;
        }
    }
}
