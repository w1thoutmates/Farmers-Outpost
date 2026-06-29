using UnityEngine;

public class Farmland : Ground
{
    public const float MIN_WATERING_LEVEL = 0;
    public const float MAX_WATERING_LEVEL = 100;
    
    [SerializeField]
    [Range(MIN_WATERING_LEVEL, MAX_WATERING_LEVEL)] 
    private float wateringLevel = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        
    }
    
    private void ReduceWateringLevel()
    {
        wateringLevel -= 25f;
        // change material
        // проверка не ноль ли уровень политости
        // +1 к этапу роста растения
    }

    public void UpdateWateringLevel(float amount)
    {
        wateringLevel += amount;
        // ограничение на 100 уровне политости
        // change material если новый этап политости:
        //                 (0-24 -> слабая политость
        //                  25-49 -> норм политость
        //                  50-74 -> сильная политость
        //                  75-100 -> кап политости)
    }

    private void OnEnable()
    {
        EventBus.onNewDayStarted += ReduceWateringLevel;
    }

    private void OnDisable()
    {
        EventBus.onNewDayStarted -= ReduceWateringLevel;
    }
    
}
