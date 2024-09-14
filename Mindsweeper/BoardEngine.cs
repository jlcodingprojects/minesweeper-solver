using System.Drawing;
using System.Drawing.Imaging;
using System.Management;
using static Mindsweeper.Clicker;
using static Mindsweeper.Constants;

namespace Mindsweeper
{
    public class BoardEngine
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private const int _blockSize = 20;
        private Point _boardPosition = new Point();
        private IMatrix _matrix;
        private bool matrixUpToDate = false;
        private int _screenWidth = 0;
        private int _screenHeight = 0;

        private Matrix _rawMatrix { get; set; }

        public IMatrix GetMatrix()
        {
            return _matrix;
        }

        public void ClearProxy()
        {
            _matrix = _rawMatrix;
        }

        public BoardEngine UseProxy(IEnumerable<KeyValuePair<Point, int>> maps)
        {
            _matrix = _rawMatrix.ToProxy(maps);
            return this;
        }

        public BoardEngine()
        {
            Console.WriteLine("Initialising...");

            ManagementObjectSearcher mydisplayResolution = new ManagementObjectSearcher("SELECT CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController");
            foreach (ManagementObject record in mydisplayResolution.Get())
            {
                Console.WriteLine("-----------------------Current Resolution---------------------------------");
                Console.WriteLine("CurrentHorizontalResolution  -  " + record["CurrentHorizontalResolution"]);
                Console.WriteLine("CurrentVerticalResolution  -  " + record["CurrentVerticalResolution"]);
                _screenWidth = int.Parse(record["CurrentHorizontalResolution"].ToString() ?? "0");
                _screenHeight = int.Parse(record["CurrentVerticalResolution"].ToString() ?? "0");
            }

            if (_screenWidth == 0)
                throw new Exception("Failed to get screen size :'(");

            Rectangle bounds = new Rectangle(0, 0, _screenWidth, _screenHeight);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                
                SetBoardPosition(bitmap);
                SetBoardDimensions(bitmap);
                var array = new int[Width, Height];
                _rawMatrix = new Matrix(array);
                _matrix = _rawMatrix;
            }
        }

        public List<Point> GetSafeTiles()
        {
            EnsureMatrixCurrent();
            return AlgorithmicSolver.FindAllSafeTiles(_matrix);
        }

        public List<Point> GetDangerousTiles()
        {
            EnsureMatrixCurrent();
            return AlgorithmicSolver.FindAllDangerousTiles(_matrix);
        }

        public List<Point> GetAmbivalentClueTiles()
        {
            EnsureMatrixCurrent();
            return AlgorithmicSolver.GetAmbivalentClueTiles(_matrix);
        }

        public void ClickAllTiles(List<Point> tiles, ClickType clickType)
        {
            matrixUpToDate = false;

            foreach (var tile in tiles)
            {
                Clicker.ClickAt(_boardPosition.X + (tile.X * _blockSize) + 8,
                    _boardPosition.Y + (tile.Y * _blockSize) + 8,
                    clickType);

                Thread.Sleep(20);
            }
        }

        private void SetBoardPosition(Bitmap bmp)
        {
            var colorW = Color.FromArgb(189, 189, 189);

            int posX = 0, posY = 0;
            // find smiley face and then calculate top of board
            for (int y = 50; y < bmp.Height - 500; y++)
            {
                for (int x = 0; x < bmp.Width - 500; x++)
                {
                    if (IsOnSmiley(bmp, x, y))
                    {
                        posX = x;
                        posY = y + 41; // top of smiley face down to top of board
                        x = bmp.Width;
                        y = bmp.Height;
                    }
                }
            }

            while (posX > 0)
            {
                if (bmp.GetPixel(posX - 1, posY) == colorW
                    && bmp.GetPixel(posX - 2, posY) == colorW
                    && bmp.GetPixel(posX - 3, posY) == colorW)
                {
                    posX = posX + 3;
                    break;
                }

                posX--;
            }

            if (posX == 0)
                throw new Exception("No board found :'(");

            _boardPosition = new Point(posX, posY);
        }

        private static Color color0 = Color.FromArgb(255, 255, 0);
        private static Color color1 = Color.FromArgb(44, 44, 44);

        private bool IsOnSmiley(Bitmap bmp, int x, int y)
        {
            return (bmp.GetPixel(x, y - 2) == color1
                        && bmp.GetPixel(x, y) == color0);
        }

        private static Color border = Color.FromArgb(156, 156, 156);
        private void SetBoardDimensions(Bitmap bmp)
        {
            Width = 0;
            Height = 0;
            int x = _boardPosition.X;
            int y = _boardPosition.Y;

            while (!(bmp.GetPixel(x + 4, _boardPosition.Y + 4) == Colors.BackgroundColor
                && bmp.GetPixel(x + _blockSize + 10, _boardPosition.Y + 4) == Colors.White))
            {
                Width++;
                x += _blockSize;
            }

            while (!(bmp.GetPixel(_boardPosition.X + 4, y + 4) == Colors.White
                && bmp.GetPixel(_boardPosition.X + 4, y + _blockSize + 12) == Colors.White))
            {
                Height++;
                y += _blockSize;
            }

            // cause it counts 1 extra
            Height--;
        }


        private void EnsureMatrixCurrent()
        {
            if (matrixUpToDate)
                return;

            // todo - can optimise the screen grab by checking just the required pixels first
            //if(!IsBoardPositionCorrect)
            // grab board again?

            using (Bitmap bitmap = new Bitmap(_screenWidth, _screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(_screenWidth, _screenHeight));
                }
                int offset = _blockSize / 2;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        var centrePixel = bitmap.GetPixel(_boardPosition.X + (x * _blockSize) + offset, _boardPosition.Y + (y * _blockSize) + offset);
                        var topPixel = bitmap.GetPixel(_boardPosition.X + (x * _blockSize) + 4, _boardPosition.Y + (y * _blockSize));
                        _matrix[x, y] = ParsePixel(centrePixel);
                        if (_matrix[x, y] == BombValues.Safe)
                        {
                            if (topPixel == Colors.White)
                                _matrix[x, y] = BombValues.Unknown;
                        }
                    }
                }
                matrixUpToDate = true;
            }
        }

        private static int ParsePixel(Color pixel)
        {
            if (pixel == Colors.BackgroundColor)
            {
                return BombValues.Safe;
            }
            else if (pixel == Colors.One)
            {
                return 1;
            }
            else if (pixel == Colors.Two)
            {
                return 2;
            }
            else if (pixel == Colors.Three)
            {
                return 3;
            }
            else if (pixel == Colors.Four)
            {
                return 4;
            }
            else if (pixel == Colors.Five)
            {
                return 5;
            }
            else if (pixel == Colors.Six)
            {
                return 6;
            }
            else if (pixel == Colors.Flag)
            {
                return BombValues.Bomb;
            }
            else if (pixel == Colors.Bomb)
            {
                return BombValues.Bomb;
            }

            throw new InvalidOperationException("Unknown tile");
        }
    }
}
