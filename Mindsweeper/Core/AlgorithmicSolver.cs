using System.Drawing;
using static Mindsweeper.Misc.Constants;

namespace Mindsweeper.Core
{
    internal static class AlgorithmicSolver
    {
        public static List<Point> FindAllDangerousTiles(IMatrix matrix)
        {
            List<Point> results = new List<Point>();

            foreach (var clue in matrix.GetClues())
            {
                int value = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountAdjacentTiles(matrix, clue.X, clue.Y);

                if (value == unknownCount + bombCount && unknownCount > 0)
                {
                    var unknownPositions = ListUnknownAdjacentTiles(matrix, clue.X, clue.Y);
                    foreach (var deadlyPos in unknownPositions)
                    {
                        results.Add(new Point(deadlyPos.X, deadlyPos.Y));
                    }

                }
            }

            return results.Distinct().ToList();
        }

        public static List<Point> FindAllSafeTiles(IMatrix matrix)
        {
            List<Point> results = new List<Point>();

            var tilesToCheck = new List<(int X, int Y)>();

            foreach (var clue in matrix.GetClues())
            {
                int val = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountAdjacentTiles(matrix, clue.X, clue.Y);
                if (unknownCount > 0 && matrix[clue.X, clue.Y] == bombCount)
                {
                    tilesToCheck.Add((clue.X, clue.Y));
                }
            }

            foreach (var tile in tilesToCheck)
            {
                var unknown = ListUnknownAdjacentTiles(matrix, tile.X, tile.Y);
                results.AddRange(unknown);
            }

            return results;
        }

        public static List<Point> GetAmbivalentClueTiles(IMatrix matrix)
        {
            List<Point> results = new List<Point>();

            foreach (var clue in matrix.GetClues())
            {
                int val = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountAdjacentTiles(matrix, clue.X, clue.Y);
                if (unknownCount > 0 && unknownCount > val - bombCount)
                {
                    results.Add(new Point(clue.X, clue.Y));
                }
            }

            return results;
        }

        public static bool IsMatrixValid(IMatrix matrix)
        {
            List<Point> results = new List<Point>();

            foreach (var clue in matrix.GetClues())
            {
                int value = matrix[clue.X, clue.Y];

                (int unknownCount, int bombCount) = CountAdjacentTiles(matrix, clue.X, clue.Y);
                if (bombCount > value)
                    return false;
            }

            return true;
        }

        public static (int Unknown, int Bombs) CountAdjacentTiles(IMatrix matrix, int row, int col)
        {
            int unknownCount = 0;
            int bombCount = 0;

            for (int i = Math.Max(0, row - 1); i <= Math.Min(matrix.Width - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(matrix.Height - 1, col + 1); j++)
                {
                    if (i == row && j == col) continue;
                    if (matrix[i, j] == BombValues.Unknown) unknownCount++;
                    if (matrix[i, j] == BombValues.Bomb) bombCount++;
                }
            }

            return (unknownCount, bombCount);
        }

        public static List<Point> ListUnknownAdjacentTiles(IMatrix matrix, int row, int col)
        {
            var results = new List<Point>();

            for (int i = Math.Max(0, row - 1); i <= Math.Min(matrix.Width - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(matrix.Height - 1, col + 1); j++)
                {
                    if (i == row && j == col) continue;
                    if (matrix[i, j] == -1) results.Add(new Point(i, j));
                }
            }

            return results;
        }
    }
}