using Scopely.Core.Extensions;
using Scopely.Core.Structures;
using Scopely.Core.Tests.Mocks;

namespace Scopely.Core.Tests.ValueObjects;

public class CharExtensionsTests
{
    [Fact]
    public void RemoveLastFoundItem_WhenLetterDoesNotExist_ShouldReturnNotFoundValue()
    {
        var array = new char[] { 'a', 'b', 'c' };

        var idx = CharExtensions.RemoveLastFoundItem(ref array, 'd');

        Assert.Equal(-1, idx);
        Assert.Equal(3, array.Length);
    }

    [Fact]
    public void RemoveLastFoundItem_WhenItemIsRepeated_ShouldRemoveLastOccurrence()
    {
        var array = new char[] { 'a', 'b', 'c' };
        var expectedResult = new char[] { 'b', 'c' };

        var idx = CharExtensions.RemoveLastFoundItem(ref array, 'a');

        Assert.Equal(0, idx);
        Assert.Equal(expectedResult, array);
    }

    [Fact]
    public void GetLastCharIndexesOfWord_WhenWordExists_ShouldReturnCorrectIndexes()
    {
        var array = new char[] { 'a', 'v', 'a', 'c', 'a' };
        var wordToCheck = "cava";
        var expectedResult = new List<int>() { 3, 4, 1, 2 };

        var result = array.GetLastCharIndexesOfWord(wordToCheck);

        Assert.Equal(expectedResult, result);
    }

}