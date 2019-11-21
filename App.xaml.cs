using Avalonia;
using Avalonia.Markup.Xaml;

namespace ant
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}