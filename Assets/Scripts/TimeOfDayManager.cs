using UnityEngine;

public enum TimeOfDayPhase
{
    Dawn,   // Amanecer
    Day,    // Día
    Dusk,   // Atardecer
    Night   // Noche
}

public class TimeOfDayManager : MonoBehaviour
{
    public static TimeOfDayManager Instance { get; private set; }

    public float dayDurationMinutes = 5f;

     [Range(0f, 1f)]
    public float startTimeOfDay = 0.0f;
  public Light mainLight;

     public Gradient lightColorOverDay;

     public AnimationCurve lightIntensityOverDay = AnimationCurve.Linear(0, 0.2f, 1, 0.2f);

     public Gradient ambientColorOverDay;

     public float minSunAngle = -30f;

    public float maxSunAngle = 60f;

    [Range(0f, 1f)]
    public float timeOfDay01; // 0-1 dentro del ciclo actual

    public TimeOfDayPhase currentPhase;

    private float _secondsPerDay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _secondsPerDay = Mathf.Max(1f, dayDurationMinutes * 60f);
        timeOfDay01 = Mathf.Repeat(startTimeOfDay, 1f);
        UpdateLighting(0f, true);
    }

    private void Update()
    {
        if (_secondsPerDay <= 0f)
        {
            _secondsPerDay = Mathf.Max(1f, dayDurationMinutes * 60f);
        }

        float delta = Time.unscaledDeltaTime / _secondsPerDay;
        timeOfDay01 = Mathf.Repeat(timeOfDay01 + delta, 1f);

        UpdateLighting(delta, false);
    }

    private void UpdateLighting(float delta, bool forcePhaseRecalc)
    {
        // Actualizar luz direccional
        if (mainLight != null)
        {
            if (lightColorOverDay != null)
            {
                mainLight.color = lightColorOverDay.Evaluate(timeOfDay01);
            }

            if (lightIntensityOverDay != null)
            {
                mainLight.intensity = lightIntensityOverDay.Evaluate(timeOfDay01);
            }

            float sunAngle = Mathf.Lerp(minSunAngle, maxSunAngle, Mathf.Sin(timeOfDay01 * Mathf.PI));
            mainLight.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
        }

        // Actualizar luz ambiente global
        if (ambientColorOverDay != null)
        {
            RenderSettings.ambientLight = ambientColorOverDay.Evaluate(timeOfDay01);
        }

        // Actualizar fase (amanecer/día/atardecer/noche)
        var newPhase = CalculatePhase(timeOfDay01);
        if (newPhase != currentPhase || forcePhaseRecalc)
        {
            currentPhase = newPhase;
        }
    }

    public void SkipToMorning()
    {
        // Fijamos la hora a un valor típico de mañana / pleno día
        timeOfDay01 = 0.3f; // dentro del rango Day
        UpdateLighting(0f, true);
    }

    public bool PuedeDormir()
    {
        return currentPhase == TimeOfDayPhase.Dusk || currentPhase == TimeOfDayPhase.Night;
    }

    private TimeOfDayPhase CalculatePhase(float t)
    {
        // t en [0,1]
        // Diseño simple de rangos:
        // 0.00 - 0.20  → Amanecer
        // 0.20 - 0.55  → Día
        // 0.55 - 0.75  → Atardecer
        // 0.75 - 1.00  → Noche
        if (t < 0.20f)
            return TimeOfDayPhase.Dawn;
        if (t < 0.55f)
            return TimeOfDayPhase.Day;
        if (t < 0.75f)
            return TimeOfDayPhase.Dusk;
        return TimeOfDayPhase.Night;
    }
}
