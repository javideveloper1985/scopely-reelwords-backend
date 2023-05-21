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

    public void Shuffle()
    {
        int num_rows = GetLength(0);
        int num_cols = GetLength(1);
        int num_cells = num_rows * num_cols;

        Random rand = new();
        for (int i = 0; i < num_cells - 1; i++)
        {
            int j = rand.Next(i, num_cells);

            int row_i = i / num_cols;
            int col_i = i % num_cols;
            int row_j = j / num_cols;
            int col_j = j % num_cols;

            (this[row_j, col_j], this[row_i, col_i]) = (this[row_i, col_i], this[row_j, col_j]);
        }
    }
}
