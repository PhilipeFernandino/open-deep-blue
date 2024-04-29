namespace Core.HealthSystem
{
    public readonly struct HealthChangedData
    {
        public readonly float HealthLastValue;
        public readonly float HealthNewValue;

        public HealthChangedData(float healthLastValue, float healthNewValue)
        {
            HealthLastValue = healthLastValue;
            HealthNewValue = healthNewValue;
        }
    }
}