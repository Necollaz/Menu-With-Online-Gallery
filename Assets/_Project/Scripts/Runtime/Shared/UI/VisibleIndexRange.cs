public readonly struct VisibleIndexRange
{
    public VisibleIndexRange(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }
    
    public int StartIndex { get; }
    public int EndIndex { get; }
    public bool IsValid => StartIndex >= 0 && EndIndex >= StartIndex;

    public bool Contains(int index)
    {
        return index >= StartIndex && index <= EndIndex;
    }
}