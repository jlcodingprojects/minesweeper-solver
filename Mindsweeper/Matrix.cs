using System.Drawing;

namespace Mindsweeper
{
    public interface IMatrix
    {
        int this[int x, int y] { get; set; }
        int Width { get; }
        int Height { get; }
        public IEnumerable<Point> GetClues();
    }

    public class Matrix : IMatrix
    {
        public int Width { get; }
        public int Height { get; }

        public int this[int x, int y] { get => _matrix[x, y]; set => _matrix[x, y] = value; }

        private int[,] _matrix;

        public Matrix(int[,] matrix)
        {
            _matrix = matrix;
            Width = matrix.GetLength(0);
            Height = matrix.GetLength(1);
        }

        public Proxy ToProxy(IEnumerable<KeyValuePair<Point, int>> maps)
        {
            return new Proxy(this, maps);
        }

        public IEnumerable<Point> GetClues()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (_matrix[i, j] > 0 && _matrix[i, j] <= 8)
                    {
                        yield return new Point(i, j);
                    }
                }
            }
        }
    }

    public class Proxy : IMatrix
    {
        private Matrix _matrix;
        private Dictionary<Point, int> _proxy;
        public Proxy(Matrix matrix, IEnumerable<KeyValuePair<Point, int>> maps)
        {
            _matrix = matrix;
            _proxy = maps.ToDictionary();
        }

        public int this[int x, int y]
        {
            get => _proxy.TryGetValue(new Point(x, y), out var value)
                ? value
                : _matrix[x, y];
            set => _matrix[x, y] = value;
        }

        public int Width => _matrix.Width;

        public int Height => _matrix.Height;

        public IEnumerable<Point> GetClues()
        {
            return _matrix.GetClues();
        }
    }
}
