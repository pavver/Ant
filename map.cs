namespace ant
{
    public class Map
    {
        private readonly byte _countColors;
        public readonly byte[,] _map;

        public Map(int size, byte countColors)
        {
            _map = new byte[size, size];
            _countColors = countColors;
        }

        public byte Step(int x, int y)
        {
            _map[x, y]++;
            if (_map[x, y] > _countColors)
                _map[x, y] = 1;

            return _map[x, y];
        }
    }
}