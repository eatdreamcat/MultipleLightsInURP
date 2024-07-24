using System;
using SphericalHarmonics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class SHLightingRenderFeature : ScriptableRendererFeature
{
    [Serializable]
    public class FeatureSettings
    {
        [Range(1, 128)]
        public int sampleCount = 4;
    }
    public class SphericalHarmonicLightingRenderPass : ScriptableRenderPass
    {
        private static int _SHLightingCoefficients = Shader.PropertyToID("_SHLightingCoefficients");
        private const int k_Level = 3;
        private const int k_CoefficientsCount = k_Level * k_Level;
        
        public static Transform s_Center;
        
        private Vector4[] m_Coefficients = new Vector4[k_CoefficientsCount];

        private FeatureSettings m_FeatureSettings;

        public SphericalHarmonicLightingRenderPass(FeatureSettings settings)
        {
            m_FeatureSettings = settings;
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        static Color SampleLight(Vector3 direction, Vector3 position, Light light)
        {
            switch (light.type)
            {
                case LightType.Spot:
                    return light.color * light.intensity *
                           Mathf.Max(0, Vector3.Dot(direction, -light.transform.forward));
                case LightType.Point:
                    return light.color * light.intensity *
                           Mathf.Max(0, Vector3.Dot(direction, (position - light.transform.position).normalized));
            }
            
            return Color.black;
        }
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using var profiler = new ProfilingScope(cmd, new ProfilingSampler("SphericalHarmonicLighting"));
            
            for (int coefficientIndex = 0; coefficientIndex < k_CoefficientsCount; ++coefficientIndex)
            {
                m_Coefficients[coefficientIndex] = Vector4.zero;
            }
            
            var visibleLights = renderingData.cullResults.visibleLights;
            for (int i = 0; i < visibleLights.Length; ++i)
            {
                var light = visibleLights[i].light;
                if (light.type != LightType.Point && light.type != LightType.Spot)
                {
                    continue;
                }

                var dir = light.transform.position - s_Center.position;
                for (int sampleIndex = 0; sampleIndex < m_FeatureSettings.sampleCount; ++sampleIndex)
                {
                    var sampleDir = SphericalHarmonicsUtils.GetCosineWeightedRandomDirection(
                        Random.value, Random.value, dir, s_Center.position, out var pdf
                    );
                    var radiance = SampleLight(sampleDir, s_Center.position, light) * pdf;
                    for (int coefficientIndex = 0; coefficientIndex < k_CoefficientsCount; ++coefficientIndex)
                    {
                        m_Coefficients[coefficientIndex].x +=
                            SphericalHarmonicsUtils.SHBasicFull(sampleDir, coefficientIndex) * radiance.r *
                            SphericalHarmonicsUtils.BasicConstant(coefficientIndex);
                        m_Coefficients[coefficientIndex].y +=
                            SphericalHarmonicsUtils.SHBasicFull(sampleDir, coefficientIndex) * radiance.g *
                            SphericalHarmonicsUtils.BasicConstant(coefficientIndex);
                        m_Coefficients[coefficientIndex].z +=
                            SphericalHarmonicsUtils.SHBasicFull(sampleDir, coefficientIndex) * radiance.b *
                            SphericalHarmonicsUtils.BasicConstant(coefficientIndex);
                    }
                }
            }
            
            cmd.SetGlobalVectorArray(_SHLightingCoefficients, m_Coefficients);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    SphericalHarmonicLightingRenderPass m_ScriptablePass;

    [SerializeField] 
    public FeatureSettings settings;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new SphericalHarmonicLightingRenderPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRendering;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (SphericalHarmonicLightingRenderPass.s_Center == null)
        {
            return;
        }
        
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


