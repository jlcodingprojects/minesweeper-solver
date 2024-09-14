using System.Drawing;
using static Mindsweeper.Constants;

namespace Mindsweeper
{
    internal static class AlgorithmicSolver
    {
        public static List<Point> FindAllDangerousSquares(IMatrix matrix)
        {
            List<Point> results = new List<Point>();
            int rows = matrix.Width;
            int cols = matrix.Height;

            foreach (var clue in matrix.GetClues())
            {
                int value = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountNeighbors(matrix, clue.X, clue.Y);

                if (value == unknownCount + bombCount && unknownCount > 0)
                {
                    var unknownPositions = ListUnknownNeighbors(matrix, clue.X, clue.Y);
                    foreach (var deadlyPos in unknownPositions)
                    {
                        results.Add(new Point(deadlyPos.X, deadlyPos.Y));
                    }

                }
            }

            return results.Distinct().ToList();
        }

        public static List<Point> FindAllSafeSquares(IMatrix matrix)
        {
            List<Point> results = new List<Point>();
            int rows = matrix.Width;
            int cols = matrix.Height;

            var cellsToCheck = new List<(int X, int Y)>();

            foreach (var clue in matrix.GetClues())
            {
                int val = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountNeighbors(matrix, clue.X, clue.Y);
                if (unknownCount > 0 && matrix[clue.X, clue.Y] == bombCount)
                {
                    cellsToCheck.Add((clue.X, clue.Y));
                }
            }

            foreach (var cell in cellsToCheck)
            {
                var unknown = ListUnknownNeighbors(matrix, cell.X, cell.Y);
                results.AddRange(unknown);
            }

            return results;
        }

        public static List<Point> GetAmbivalentClueSquares(IMatrix matrix)
        {
            List<Point> results = new List<Point>();
            int rows = matrix.Width;
            int cols = matrix.Height;

            foreach (var clue in matrix.GetClues())
            {
                int val = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountNeighbors(matrix, clue.X, clue.Y);
                if (unknownCount > 0 && unknownCount > val - bombCount)
                {
                    results.Add(new Point(clue.X, clue.Y));
                }
            }

            return results;
        }

        public static bool IsBoardValid(IMatrix matrix)
        {
            List<Point> results = new List<Point>();

            int rows = matrix.Width;
            int cols = matrix.Height;

            foreach (var clue in matrix.GetClues())
            {
                int value = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountNeighbors(matrix, clue.X, clue.Y);
                if (bombCount > value)
                    return false;
            }

            return true;
        }

        public static (int Unknown, int Bombs) CountNeighbors(IMatrix board, int row, int col)
        {
            int unknownCount = 0;
            int bombCount = 0;
            int rows = board.Width;
            int cols = board.Height;

            for (int i = Math.Max(0, row - 1); i <= Math.Min(rows - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(cols - 1, col + 1); j++)
                {
                    if (i == row && j == col) continue;
                    if (board[i, j] == BombValues.Unknown) unknownCount++;
                    if (board[i, j] == BombValues.Bomb) bombCount++;
                }
            }

            return (unknownCount, bombCount);
        }

        public static List<Point> ListUnknownNeighbors(IMatrix board, int row, int col)
        {
            var results = new List<Point>();
            int rows = board.Width;
            int cols = board.Height;

            for (int i = Math.Max(0, row - 1); i <= Math.Min(rows - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(cols - 1, col + 1); j++)
                {
                    if (i == row && j == col) continue;
                    if (board[i, j] == -1) results.Add(new Point(i, j));
                }
            }

            return results;
        }
    }
}