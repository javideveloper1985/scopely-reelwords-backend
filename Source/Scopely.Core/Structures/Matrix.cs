namespace Scopely.Core.Structures;

public class Matrix<ItemType>
{
    private readonly ItemType[,] data;

    public Matrix(int rows, int columns)
    {
        if (rows <= 0 || columns <= 0)
            throw new ArgumentException("The number of rows and columns must be greater than zero.");

        data = new ItemType[rows, columns];
    }

    public ItemType this[int row, int column]
    {
        get { return data[row, column]; }
        set { data[row, column] = value; }
    }

    public int GetLength(int dimension) => data.GetLength(dimension);

    public void Shuffle(Random? randomizer = null)
    {
        int rows = GetLength(0);
        int cols = GetLength(1);
        int items = rows * cols;

        Random rand = randomizer ?? new();
        for (int i = 0; i < items - 1; i++)
        {
            int j = rand.Next(i, items);

            int row_i = i / cols;
            int col_i = i % cols;
            int row_j = j / cols;
            int col_j = j % cols;

            (this[row_j, col_j], this[row_i, col_i]) = (this[row_i, col_i], this[row_j, col_j]);
        }
    }
}
