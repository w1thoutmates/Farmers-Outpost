using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayNightCycler : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)] public float currentTime = 12f;
    public float dayLengthInSeconds = 30f;
    public bool isPaused = false;

    [Header("Sun Reference")]
    public Light sunLight;
    private HDAdditionalLightData sunLightData;

    [Header("Sun Intensity Settings")]
    public Gradient sunColorGradient;
    public float maxSunIntensity = 150f;
    public float minSunIntensity = 0f;

    [Header("Volume Reference")]
    public Volume globalVolume;

    private GradientSky gradientSky;
    private Exposure exposure;
    private IndirectLightingController indirectLighting;

    public float DayPercent => currentTime / 24f;

    void Start()
    {
        if (sunLight != null)
        {
            sunLightData = sunLight.GetComponent<HDAdditionalLightData>();
            sunLightData.lightUnit = LightUnit.Lux;
        }

        if (globalVolume != null && globalVolume.profile != null)
        {
            InitVolumeComponents(globalVolume.profile);
        }
    }

    void InitVolumeComponents(VolumeProfile profile)
    {
        profile.TryGet<GradientSky>(out gradientSky);
        profile.TryGet<Exposure>(out exposure);
        profile.TryGet<IndirectLightingController>(out indirectLighting);
    }

    void Update()
    {
        if (!isPaused)
        {
            currentTime += (Time.deltaTime / dayLengthInSeconds) * 24f;
            if (currentTime >= 24f) currentTime = 0f;
        }

        UpdateLighting();
    }

    void UpdateLighting()
    {
        if (sunLight == null || sunLightData == null) return;

        float sunAngle = (DayPercent * 360f) - 90f;
        sunLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 45.0f, 0.0f));

        float correctPercent = DayPercent - 0.25f;
        float rawSin = Mathf.Sin(correctPercent * Mathf.PI * 2);

        float dayFactor = Mathf.Clamp01(rawSin);

        float globalAlpha = (rawSin + 1f) / 2f;

        if (currentTime >= 6f && currentTime <= 18f)
        {
            if (!sunLight.enabled) sunLight.enabled = true;

            sunLightData.intensity = Mathf.Lerp(minSunIntensity, maxSunIntensity, dayFactor);
            sunLight.color = sunColorGradient.Evaluate(DayPercent);
        }
        else
        {
            if (sunLight.enabled) sunLight.enabled = false;
        }

        if (gradientSky != null)
        {
            gradientSky.exposure.overrideState = true;

            float skyAlpha = (rawSin + 1f) / 2f;

            gradientSky.exposure.value = Mathf.Lerp(-2.5f, 2.0f, skyAlpha);
        }

        if (exposure != null && exposure.mode.value == ExposureMode.Fixed)
        {
            exposure.fixedExposure.overrideState = true;

            exposure.fixedExposure.value = Mathf.Lerp(-1.0f, 12.0f, globalAlpha);
        }

        if (indirectLighting != null)
        {
            indirectLighting.indirectDiffuseLightingMultiplier.overrideState = true;

            indirectLighting.indirectDiffuseLightingMultiplier.value = Mathf.Lerp(1.0f, 3.0f, globalAlpha);
        }
    }
}