using System.Drawing;
using Mindsweeper.Core;
using Mindsweeper.Misc;
using static Mindsweeper.Misc.Constants;

namespace Mindsweeper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Make sure you have minesweeper open on your first monitor and nothing covering the board.");
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            while (true)
            {
                GoSolve();
                if (!TryComplexSolve())
                {
                    Console.WriteLine("Unable to continue. Please manually solve another clue or two then continue.");
                    Console.ReadKey();
                }
            }
        }

        private static Random Random = new Random();
        private static void GoSolve()
        {
            var solver = new BoardEngine();

            var safe = solver.GetSafeTiles();
            var dangerous = solver.GetDangerousTiles();

            if (safe.Count == 0 && dangerous.Count == 0)
            {
                var posX = (Random.NextInt64() % (solver.Width - 5)) + 2;
                var posY = (Random.NextInt64() % (solver.Height - 5)) + 2;
                solver.ClickAllTiles(
                    new List<Point> { new Point((int)posX, (int)posY), },
                    Clicker.ClickType.Left);
            }

            // just to avoid any random infinite loops which could potentially occur if the user moves the mouse or the screen changes
            int safeloop = 99;

            while (safeloop > 0)
            {
                dangerous = solver.GetDangerousTiles();
                var proxy = dangerous.Select(c => new KeyValuePair<Point, int>(c, BombValues.Bomb));
                // can only calculate safe after we've calculated dangerous
                // but dont want to waste time clicking all bombs then screen grabbing and recalculating etc
                safe = solver.UseProxy(proxy).GetSafeTiles();
                solver.ClearProxy();

                if (safe.Count == 0 && dangerous.Count == 0)
                    safeloop = 0;
                solver.ClickAllTiles(safe, Clicker.ClickType.Left);
                solver.ClickAllTiles(dangerous, Clicker.ClickType.Right);

                safeloop--;
            }
        }

        private static bool TryComplexSolve()
        {
            var minesweeperEngine = new BoardEngine();
            bool foundAnswers = false;

            var clues = minesweeperEngine.GetAmbivalentClueTiles();

            // for each clue, create proxy of all possible combinations
            // then solve surround clues against each proxy
            foreach (var clue in clues)
            {
                bool initialised = false;
                var safeFound = new HashSet<Point>();
                var dangerousFound = new HashSet<Point>();

                // The number of adjacent bombs
                int value = minesweeperEngine.GetMatrix()[clue.X, clue.Y];
                var unknownAdjacentTiles = AlgorithmicSolver.ListUnknownAdjacentTiles(minesweeperEngine.GetMatrix(), clue.X, clue.Y);
                (int unknownCount, int bombCount) = AlgorithmicSolver.CountAdjacentTiles(minesweeperEngine.GetMatrix(), clue.X, clue.Y);
                var remainingAdjacentBombs = value - bombCount;

                // For all adjacent tiles, return every possible valid combination of bombs
                var bombCombos = unknownAdjacentTiles
                    .Select(c => new KeyValuePair<Point, int>(c, BombValues.Bomb))
                    .AsCombinations(remainingAdjacentBombs);

                foreach (var combination in bombCombos)
                {
                    // Temporarily proxy the matrix with this combination while we check if the game is still valid
                    minesweeperEngine.UseProxy(combination);

                    bool isValid = AlgorithmicSolver.IsMatrixValid(minesweeperEngine.GetMatrix());
                    if (!isValid)
                        continue;

                    var dangerous = minesweeperEngine.GetDangerousTiles();
                    var safe = minesweeperEngine.GetSafeTiles();

                    if (initialised)
                    {
                        safeFound.IntersectWith(safe);
                        dangerousFound.IntersectWith(dangerous);
                    }
                    else
                    {
                        safeFound = new HashSet<Point>(safe);
                        dangerousFound = new HashSet<Point>(dangerous);
                        initialised = true;
                    }

                    if (safeFound.Count == 0 && dangerousFound.Count == 0) break;
                }

                if (safeFound.Count != 0 || dangerousFound.Count != 0)
                {
                    minesweeperEngine.ClickAllTiles(dangerousFound.ToList(), Clicker.ClickType.Right);
                    minesweeperEngine.ClickAllTiles(safeFound.ToList(), Clicker.ClickType.Left);
                    foundAnswers = true;
                }
            }

            return foundAnswers;
        }
    }

}
