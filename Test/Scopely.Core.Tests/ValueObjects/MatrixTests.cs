using Scopely.Core.Structures;
using Scopely.Core.Tests.Mocks;

namespace Scopely.Core.Tests.ValueObjects;

public class MatrixTestsTests
{
    [Fact]
    public void Shuffle_WhenExecuteMethod_ShouldShuffleValues()
    {
        var matrix = new Matrix<int>(2, 2);
        matrix[0, 0] = 1;
        matrix[0, 1] = 2;
        matrix[1, 0] = 3;
        matrix[1, 1] = 4;

        var fixedRandom = new RandomizerMock(3, 2, 1, 0);

        matrix.Shuffle(fixedRandom);

        Assert.Equal(4, matrix[0, 0]); 
        Assert.Equal(2, matrix[0, 1]);
        Assert.Equal(3, matrix[1, 0]);
        Assert.Equal(1, matrix[1, 1]);
    }
}