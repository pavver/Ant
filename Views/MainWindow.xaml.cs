using System.Linq;
using ant.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace ant.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FontFamily = "Arial";
            DataContext = _viewModel;
        }

        private IControl _img;
        private MainWindowViewModel _viewModel;
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _img = ((Grid)Content).Children.First();

            // Delegate is called from bg thread, use synchronous call to avoid concurrency issues within Avalonia.
            _viewModel = new MainWindowViewModel(() =>
                Dispatcher.UIThread.InvokeAsync(() => _img.InvalidateVisual()).Wait());
        }
    }
}