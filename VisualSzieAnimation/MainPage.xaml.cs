using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace VisualSzieAnimation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var rootVisual = Root.ContainerVisual();
            var compositor = rootVisual.Compositor;

            var visual = compositor.CreateSpriteVisual();
            visual.Brush = compositor.CreateColorBrush(Colors.LightSeaGreen);
            visual.Size = new Vector2(200.0f, 200.0f);
            visual.Offset = new Vector3(24.0f, 24.0f, 0.0f);

            visual.EnableImplicitAnimation(VisualPropertyType.All);

            rootVisual.Children.InsertAtTop(visual);

            await Task.Delay(3000);
            visual.Opacity = 0.2f;

            await Task.Delay(3000);
            visual.Offset = new Vector3(120.0f, 120.0f, 0.0f);

            await Task.Delay(3000);
            visual.Size = new Vector2(400.0f, 400.0f);
        }
    }
}
