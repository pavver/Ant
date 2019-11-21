using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ant.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FontFamily = "Arial";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}