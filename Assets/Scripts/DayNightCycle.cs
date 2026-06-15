using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{

    [Header("¬рем€")]
    [Tooltip("Ќачальное врем€ суток (0-24)")]
    [Range(0f, 24f)] public float currentTime = 8f;
    [Tooltip("ѕродолжительность игрового дн€ в реальных секундах")]
    public float dayDurationSeconds = 120f;
    public bool isPlaying = true;

    [Header("—олнце и Ћуна")]
    [Tooltip("Directional Light Ч источник солнечного света")]
    public Light sunLight;
    [Tooltip("Directional Light Ч источник лунного света (опционально)")]
    public Light moonLight;

    [Header(" ривые интенсивности")]
    [Tooltip("»нтенсивность солнца в течение суток (0=полночь, 0.5=полдень, 1=полночь)")]
    public AnimationCurve sunIntensityCurve = DefaultSunCurve();
    [Tooltip("»нтенсивность луны в течение суток")]
    public AnimationCurve moonIntensityCurve = DefaultMoonCurve();
    [Tooltip("ћножитель пиковой интенсивности солнца (Lux)")]
    public float sunMaxIntensity = 100000f;
    [Tooltip("ћножитель пиковой интенсивности луны (Lux)")]
    public float moonMaxIntensity = 1f;

    [Header("÷вет солнца по времени суток")]
    public Gradient sunColorGradient = DefaultSunGradient();

    [Header("HDRP Volume")]
    [Tooltip("Volume с HDRI Sky, Fog и прочими эффектами")]
    public Volume skyVolume;

    [Header("Ќастройки неба")]
    [Tooltip("HDR текстура дневного неба (HDRI)")]
    public Cubemap daySkyHDRI;
    [Tooltip("HDR текстура ночного неба (HDRI)")]
    public Cubemap nightSkyHDRI;

    [Header("Ambient / освещение сцены")]
    [Tooltip("Ёкспозици€ неба днЄм")]
    [Range(-5f, 5f)] public float dayExposure = 0f;
    [Tooltip("Ёкспозици€ неба ночью")]
    [Range(-5f, 5f)] public float nightExposure = -2f;
    [Tooltip(" рива€ перехода экспозиции (0=ночь, 0.5=день, 1=ночь)")]
    public AnimationCurve exposureCurve = DefaultExposureCurve();

    [Header("“уман")]
    public bool controlFog = true;
    [Tooltip("÷вет тумана Ч дневной")]
    public Color fogDayColor = new Color(0.7f, 0.82f, 1f);
    [Tooltip("÷вет тумана Ч ночной")]
    public Color fogNightColor = new Color(0.04f, 0.05f, 0.12f);
    [Tooltip("ѕлотность тумана Ч дневна€")]
    [Range(0f, 1f)] public float fogDayAttenuation = 0.003f;
    [Tooltip("ѕлотность тумана Ч ночна€")]
    [Range(0f, 1f)] public float fogNightAttenuation = 0.01f;

    [Header("÷ветокоррекци€ (Color Adjustments)")]
    public bool controlColorAdjustments = true;
    [Tooltip("Ќасыщенность днЄм")]
    [Range(-100f, 100f)] public float daySaturation = 10f;
    [Tooltip("Ќасыщенность ночью")]
    [Range(-100f, 100f)] public float nightSaturation = -30f;
    [Tooltip(" онтраст ночью")]
    [Range(-100f, 100f)] public float nightContrast = 20f;
    [Tooltip("÷вет ночных теней")]
    public Color nightShadowColor = new Color(0.05f, 0.05f, 0.2f);

    public System.Action onSunrise;
    public System.Action onSunset;

    private HDAdditionalLightData sunHDData;
    private HDAdditionalLightData moonHDData;
    private PhysicallyBasedSky pbSky;
    private HDRISky hdriSky;
    private Fog fog;
    private ColorAdjustments colorAdjustments;
    private Exposure volumeExposure;

    private bool wasDaytime = false;
    private float normalizedTime => currentTime / 24f;

    void OnEnable()
    {
        CacheComponents();
    }

    void Update()
    {
        if (isPlaying && Application.isPlaying)
        {
            currentTime += (24f / dayDurationSeconds) * Time.deltaTime;
            if (currentTime >= 24f)
                currentTime -= 24f;
        }

        UpdateCycle();
        CheckDayNightEvents();
    }

    void UpdateCycle()
    {
        float t = normalizedTime;

        UpdateSunAndMoon(t);
        UpdateSkyExposure(t);

        if (controlFog) UpdateFog(t);
        if (controlColorAdjustments) UpdateColorGrading(t);
    }

    void UpdateSunAndMoon(float t)
    {
        float sunAngle = (t * 360f) - 90f;
        if (sunLight != null)
        {
            sunLight.transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0f);

            float intensity = sunIntensityCurve.Evaluate(t) * sunMaxIntensity;
            sunLight.color = sunColorGradient.Evaluate(t);

            if (sunHDData != null)
                sunHDData.SetIntensity(intensity, LightUnit.Lux);
            else
                sunLight.intensity = intensity;

            sunLight.enabled = intensity > 0.01f;
        }

        if (moonLight != null)
        {
            moonLight.transform.localRotation = Quaternion.Euler(sunAngle + 180f, 170f, 0f);

            float moonIntensity = moonIntensityCurve.Evaluate(t) * moonMaxIntensity;
            if (moonHDData != null)
                moonHDData.SetIntensity(moonIntensity, LightUnit.Lux);
            else
                moonLight.intensity = moonIntensity;

            moonLight.enabled = moonIntensity > 0.0001f;
        }
    }

    void UpdateSkyExposure(float t)
    {
        if (volumeExposure != null)
        {
            float exp = Mathf.Lerp(nightExposure, dayExposure, exposureCurve.Evaluate(t));
            volumeExposure.fixedExposure.value = exp;
        }

        if (hdriSky != null)
        {
            bool isDay = t > 0.25f && t < 0.75f;
            float blendT = Mathf.SmoothStep(0f, 1f, exposureCurve.Evaluate(t));

            if (daySkyHDRI != null && nightSkyHDRI != null)
            {
                hdriSky.hdriSky.value = blendT > 0.5f ? daySkyHDRI : nightSkyHDRI;
            }
        }
    }

    void UpdateFog(float t)
    {
        if (fog == null) return;

        float dayT = exposureCurve.Evaluate(t);
        fog.albedo.value = Color.Lerp(fogNightColor, fogDayColor, dayT);
        fog.meanFreePath.value = Mathf.Lerp(
            1f / fogNightAttenuation,
            1f / fogDayAttenuation,
            dayT);
    }

    void UpdateColorGrading(float t)
    {
        if (colorAdjustments == null) return;

        float dayT = exposureCurve.Evaluate(t);
        colorAdjustments.saturation.value = Mathf.Lerp(nightSaturation, daySaturation, dayT);
        colorAdjustments.contrast.value = Mathf.Lerp(nightContrast, 0f, dayT);
        colorAdjustments.colorFilter.value = Color.Lerp(nightShadowColor, Color.white, dayT);
    }

    void CheckDayNightEvents()
    {
        bool isDaytime = currentTime > 6f && currentTime < 20f;
        if (isDaytime && !wasDaytime)
            onSunrise?.Invoke();
        else if (!isDaytime && wasDaytime)
            onSunset?.Invoke();
        wasDaytime = isDaytime;
    }

    public void SetTime(float hour)
    {
        currentTime = Mathf.Clamp(hour, 0f, 24f);
        UpdateCycle();
    }

    public void TransitionToTime(float targetHour, float durationSeconds)
    {
        StartCoroutine(TransitionRoutine(targetHour, durationSeconds));
    }

    IEnumerator TransitionRoutine(float targetHour, float duration)
    {
        float startTime = currentTime;
        float elapsed = 0f;
        bool wasPlaying = isPlaying;
        isPlaying = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentTime = Mathf.Lerp(startTime, targetHour, elapsed / duration);
            UpdateCycle();
            yield return null;
        }

        currentTime = targetHour;
        isPlaying = wasPlaying;
    }

    void CacheComponents()
    {
        if (sunLight != null)
            sunHDData = sunLight.GetComponent<HDAdditionalLightData>();
        if (moonLight != null)
            moonHDData = moonLight.GetComponent<HDAdditionalLightData>();

        if (skyVolume != null)
        {
            skyVolume.profile.TryGet(out hdriSky);
            skyVolume.profile.TryGet(out pbSky);
            skyVolume.profile.TryGet(out fog);
            skyVolume.profile.TryGet(out colorAdjustments);
            skyVolume.profile.TryGet(out volumeExposure);
        }
    }

    static AnimationCurve DefaultSunCurve()
    {
        var curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0f, 0f));
        curve.AddKey(new Keyframe(0.25f, 0f));
        curve.AddKey(new Keyframe(0.29f, 0.3f));
        curve.AddKey(new Keyframe(0.5f, 1f));
        curve.AddKey(new Keyframe(0.71f, 0.3f));
        curve.AddKey(new Keyframe(0.75f, 0f));
        curve.AddKey(new Keyframe(1f, 0f));
        return curve;
    }

    static AnimationCurve DefaultMoonCurve()
    {
        var curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0f, 1f));
        curve.AddKey(new Keyframe(0.25f, 0.5f));
        curve.AddKey(new Keyframe(0.3f, 0f));
        curve.AddKey(new Keyframe(0.7f, 0f));
        curve.AddKey(new Keyframe(0.75f, 0.5f));
        curve.AddKey(new Keyframe(1f, 1f));
        return curve;
    }

    static AnimationCurve DefaultExposureCurve()
    {
        var curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0f, 0f));
        curve.AddKey(new Keyframe(0.25f, 0f));
        curve.AddKey(new Keyframe(0.33f, 1f));
        curve.AddKey(new Keyframe(0.67f, 1f));
        curve.AddKey(new Keyframe(0.75f, 0f));
        curve.AddKey(new Keyframe(1f, 0f));
        return curve;
    }

    static Gradient DefaultSunGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0f),
                new GradientColorKey(new Color(1f, 0.4f, 0.1f), 0.27f),
                new GradientColorKey(new Color(1f, 0.95f, 0.8f), 0.35f),
                new GradientColorKey(new Color(1f, 0.98f, 0.95f), 0.5f),
                new GradientColorKey(new Color(1f, 0.7f, 0.2f), 0.73f),
                new GradientColorKey(new Color(0.6f, 0.2f, 0.1f), 0.78f),
                new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 1f),
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return gradient;
    }
}