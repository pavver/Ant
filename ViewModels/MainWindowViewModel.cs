﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace ant.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private byte _countColors = 4;

        private string _iterationText = "Итераций 0 из ";

        private int _seed = 12;

        private ulong _step;

        public string[] CelColors =
        {
            "#FFFFFFFF",
            "#FFEFEBE9",
            "#FFD7CCC8",
            "#FFBCAAA4",
            "#FFA1887F",
            "#FF8D6E63",
            "#FF6D4C41",
            "#FF5D4037",
            "#FF4E342E",
            "#FF3E2723",
            "#FFE64A19",
            "#FFD84315",
            "#FFBF360C"
        };

        private Color[] Colors;

        public ObservableCollection<string> Directions { get; set; } = new ObservableCollection<string>();

        private uint _iterationsPerFrame = 1000;

        public uint IterationsPerFrame
        {
            get => _iterationsPerFrame;
            set => this.RaiseAndSetIfChanged(ref _iterationsPerFrame, value);
        }

        private readonly Action _invalidate;

        public MainWindowViewModel(Action invalidate)
        {
            _invalidate = invalidate;
            var b = new BrushConverter();
            ButtonColors = CelColors.Select(d => b.ConvertFromString(d) as SolidColorBrush).ToArray();
            Colors = CelColors.Select(d => Color.Parse(d)).ToArray();
            for (int i = 0; i < 12; i++)
            {
                Directions.Add("");
            }

            UpdateDirections();
        }

        public SolidColorBrush[] ButtonColors { get; set; }

        public ulong Step
        {
            get => _step;
            set
            {
                this.RaiseAndSetIfChanged(ref _step, value);
                IterationText = $"Итераций {Step} из {MaxSteps}";
            }
        }

        public ulong MaxSteps { get; set; } = int.MaxValue / 4;

        private bool _work;

        public bool Work
        {
            get => _work;
            set
            {
                this.RaiseAndSetIfChanged(ref _work, value);
                if (value) StartWork();
            }
        }

        public string IterationText
        {
            get => _iterationText;
            set => this.RaiseAndSetIfChanged(ref _iterationText, value);
        }

        public int Seed
        {
            get => _seed;
            set
            {
                this.RaiseAndSetIfChanged(ref _seed, value);
                UpdateDirections();
            }
        }

        private int _mapSize = 512;

        public int MapSize
        {
            get => _mapSize;
            set => this.RaiseAndSetIfChanged(ref _mapSize, value);
        }

        public byte CountColors
        {
            get => _countColors;
            set
            {
                this.RaiseAndSetIfChanged(ref _countColors, value);
                UpdateDirections();
            }
        }

        private WriteableBitmap _bitmap;

        public WriteableBitmap Bitmap
        {
            get => _bitmap;
            set => this.RaiseAndSetIfChanged(ref _bitmap, value);
        }

        public void ChangeDirection(string parameter)
        {
            int i = int.Parse(parameter) -1;
            if (i > CountColors - 1) return;
            int t = 1 << i;
            Seed = _seed ^ t;
            
        }

        public unsafe void PutPixel(double x, double y, ILockedFramebuffer buf, Color color)
        {
            var pixel = color.B + ((uint) color.G << 8) + ((uint) color.R << 16) + ((uint) color.A << 24);

            var ptr = (uint*) buf.Address;
            ptr += (uint) (Bitmap.PixelSize.Width * y + x);

            *ptr = pixel;
        }

        private void StartWork()
        {
            Task.Run(() =>
            {
                var antStart = _mapSize / 2;

                var map = new Map(_mapSize, CountColors );
                var ant = new Ant(antStart, antStart);
                Step = 0;

                Bitmap = new WriteableBitmap(new PixelSize(_mapSize, _mapSize), new Vector(96, 96),
                    PixelFormat.Bgra8888);


                for (; Work && _step < MaxSteps; _step++)
                {
                    var color = map.Step(ant.X, ant.Y);
                    ant.Move(Seed, color);

                    if ((_step % IterationsPerFrame) == 0)
                    {
                        UpdateBitmap(map._map);
                        Step = _step;
                    }

                    if (ant.X == 0 || ant.Y == 0 || ant.X == _mapSize || ant.Y == _mapSize)
                        break;
                }

                Work = false;
                Step = _step;
                UpdateBitmap(map._map);
            });
        }

        private void UpdateBitmap(byte[,] map)
        {
            using var buf = Bitmap.Lock();

            for (var x = 0; x < _mapSize; x++)
            for (var y = 0; y < _mapSize; y++)
                PutPixel(x, y, buf, Colors[map[x, y]]);

            Task.Run(() => _invalidate());
        }

        private void UpdateDirections()
        {

            for (byte i = 0; i < 12; i++)
            {
                var ret = CountColors <= i ? "" : Ant.GetDirection(Seed, i) ? "→" : "←";
                if (!string.Equals(ret, Directions[i], StringComparison.Ordinal)) Directions[i] = ret;
            }
        }

    }
}