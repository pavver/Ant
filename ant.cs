namespace ant
{
    public class Ant
    {
        private int _direction = 1;
        public int X;
        public int Y;

        public Ant(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool GetDirection(int seed, byte color)
        {
            return (seed >> color) % 2 == 1;
        }

        public void Move(int seed, byte color)
        {
            _direction = GetDirection(seed, color) ? _direction + 1 : _direction - 1;
            if (_direction == -1) _direction = 3;
            if (_direction == 4) _direction = 0;

            if (_direction == 0) Y--;
            if (_direction == 1) X--;
            if (_direction == 2) Y++;
            if (_direction == 3) X++;
        }
    }
}