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
    }

    [System.Serializable]
    public struct FloatRangeAttribute
    {
        public float Min;
        public float Max;
    }
}