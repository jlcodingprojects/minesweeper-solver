using System.Drawing;
using static Mindsweeper.Constants;

namespace Mindsweeper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            while (true)
            {
                GoSolve();
                if (!TryComplexSolve())
                {
                    Console.WriteLine("Finished all that i can do. Press any key to continue if fixed mistakes.");
                    Console.ReadKey();
                }
            }
        }

        private static Random Random = new Random();
        private static void GoSolve()
        {
            var solver = new BoardEngine();

            var safe = solver.GetSafeSquares();
            var dangerous = solver.GetDangerousSquares();

            if (safe.Count == 0 && dangerous.Count == 0)
            {
                var posX = (Random.NextInt64() % solver.Width) - 5 + 1;
                var posY = (Random.NextInt64() % solver.Height) - 5 + 1;
                solver.ClickAllSquares(
                    new List<Point> { new Point((int)posX, (int)posY), },
                    Clicker.ClickType.Left);
            }

            // just to avoid any random infinite loops which could potentially occur if the user moves the mouse or the screen changes
            int safeloop = 99;

            while (safeloop > 0)
            {
                dangerous = solver.GetDangerousSquares();
                var proxy = dangerous.Select(c => new KeyValuePair<Point, int>(c, BombValues.Bomb));
                // can only calculate safe after we've calculated dangerous
                // but dont want to waste time clicking all bombs then screen grabbing and recalculating etc
                safe = solver.UseProxy(proxy).GetSafeSquares();
                solver.ClearProxy();

                if (safe.Count == 0 && dangerous.Count == 0)
                    safeloop = 0;
                solver.ClickAllSquares(safe, Clicker.ClickType.Left);
                solver.ClickAllSquares(dangerous, Clicker.ClickType.Right);

                safeloop--;
            }
        }

        private static bool TryComplexSolve()
        {
            var solver = new BoardEngine();
            bool foundAnswers = false;

            var clues = solver.GetAmbivalentClueSquares();

            // for each clue, create proxy of all possible combinations
            // then solve surround clues against each proxy
            foreach (var clue in clues)
            {
                int value = solver.GetMatrix()[clue.X, clue.Y];
                var neighbours = AlgorithmicSolver.ListUnknownNeighbors(solver.GetMatrix(), clue.X, clue.Y);
                (int unknownCount, int bombCount) = AlgorithmicSolver.CountNeighbors(solver.GetMatrix(), clue.X, clue.Y);

                var requiredFlags = value - bombCount;
                var bombCombos = neighbours
                    .Select(c => new KeyValuePair<Point, int>(c, BombValues.Bomb))
                    .AsCombinatorics(requiredFlags);

                bool initialised = false;
                var safeFound = new HashSet<Point>();
                var dangerousFound = new HashSet<Point>();

                foreach (var combination in bombCombos)
                {
                    solver.UseProxy(combination);

                    bool isValid = AlgorithmicSolver.IsBoardValid(solver.GetMatrix());
                    if (!isValid)
                        continue;

                    var dangerous = solver.GetDangerousSquares();
                    var safe = solver.GetSafeSquares();

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
                    solver.ClickAllSquares(dangerousFound.ToList(), Clicker.ClickType.Right);
                    solver.ClickAllSquares(safeFound.ToList(), Clicker.ClickType.Left);
                    foundAnswers = true;
                }
            }

            return foundAnswers;
        }
    }

}
