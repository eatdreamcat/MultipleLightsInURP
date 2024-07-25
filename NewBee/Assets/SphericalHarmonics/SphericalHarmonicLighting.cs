using System;
using System.Collections;
using System.Collections.Generic;
using SphericalHarmonics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SphericalHarmonicLighting : MonoBehaviour
{
    public bool debugCosineWeightSample = false;
    [Range(0, 128)]
    public int sampleCount = 0;

    [Range(1, 14)]
    public int directionCount = 1;
    
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
            for (int i = 0; i < directionCount; ++i)
            {
                Gizmos.color = Color.yellow;
                var debugNormal = SHLightingRenderFeature.SphericalHarmonicLightingRenderPass.s_SamplerDirs[i];
                Gizmos.DrawRay(transform.position, debugNormal * RayLength);
                Gizmos.color = Color.green;
                for (int j = 0; j < sampleCount; ++j)
                {
                    var dir = SphericalHarmonicsUtils.GetCosineWeightedRandomDirection(
                        HaltonSequence.Get((j & 1023) + 1, 2), 
                        HaltonSequence.Get((j & 1023) + 1, 3), debugNormal, transform.position, out var _);
                    Gizmos.DrawRay(transform.position, dir * RayLength);
                }
            }
            Gizmos.color = oldColor;
        }
    }
}
