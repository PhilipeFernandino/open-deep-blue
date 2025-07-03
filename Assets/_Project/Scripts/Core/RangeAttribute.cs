namespace Core
{
    public struct RangeAttribute<T>
    {
        public T Min;
        public T Max;
    }

    [System.Serializable]
    public struct UShortRangeAttribute
    {
        public ushort Min;
        public ushort Max;

        public ushort Random => (ushort)UnityEngine.Random.Range(Min, Max);
    }

    [System.Serializable]
    public struct FloatRangeAttribute
    {
        public float Min;
        public float Max;

        public float Random => UnityEngine.Random.Range(Min, Max);
    }
}