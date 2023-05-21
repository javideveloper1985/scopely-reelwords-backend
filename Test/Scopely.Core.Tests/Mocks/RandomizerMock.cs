namespace Scopely.Core.Tests.Mocks;

public class RandomizerMock : Random
{
    private readonly int[] _values;
    private int _index;

    public RandomizerMock(params int[] values)
    {
        _values = values;
        _index = 0;
    }

    public override int Next(int minValue, int maxValue)
    {
        int value = _values[_index];
        _index = (_index + 1) % _values.Length;
        return value;
    }
}