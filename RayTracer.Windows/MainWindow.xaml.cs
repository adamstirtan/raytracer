using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RayTracer.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Populate scene list
            SceneCombo.Items.Add("SphereScene");
            SceneCombo.Items.Add("TorusScene");
            SceneCombo.Items.Add("BoxScene");
            SceneCombo.SelectedIndex = 0;

            StatusTextBlock.Text = "Ready";
        }

        private async void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderButton.IsEnabled = false;
                StatusTextBlock.Text = "Rendering...";

                // Parse options
                int width = int.TryParse(WidthTextBox.Text, out var w) ? w : 400;
                int height = int.TryParse(HeightTextBox.Text, out var h) ? h : 300;
                int samples = int.TryParse(SamplesTextBox.Text, out var s) ? s : 1;
                int traceDepth = int.TryParse(TraceDepthTextBox.Text, out var d) ? d : 3;

                bool enableReflections = EnableReflectionsCheckBox.IsChecked == true;
                bool enableDiffuse = EnableDiffuseCheckBox.IsChecked == true;
                bool enableSpecular = EnableSpecularCheckBox.IsChecked == true;

                // Build RenderOptions
                var options = new RayTracer.Core.RenderOptions
                {
                    Width = width,
                    Height = height,
                    SamplesPerPixel = samples,
                    TraceDepth = traceDepth,
                    DisableReflections = !enableReflections,
                    DisableDiffuse = !enableDiffuse,
                    DisableSpeculation = !enableSpecular,
                    CameraPosition = new System.Numerics.Vector3(0, 1.5f, -6f)
                };

                // Create scene
                RayTracer.Core.Scenes.Scene scene = CreateScene(SceneCombo.SelectedItem as string ?? "SphereScene");

                var engine = new RayTracer.Core.Engine(scene, options);

                engine.RenderStarted += (s, ev) => this.DispatcherQueue.TryEnqueue(() => StatusTextBlock.Text = "Render started");
                engine.RenderCompleted += (s, time) => this.DispatcherQueue.TryEnqueue(() => StatusTextBlock.Text = $"Completed in {time.TotalSeconds:F2}s");

                // Run render on background thread to avoid UI blocking
                var image = await System.Threading.Tasks.Task.Run(() => engine.Render());

                // Convert ImageSharp image to BitmapImage via MemoryStream
                using var ms = new System.IO.MemoryStream();
                image.Save(ms, new PngEncoder());
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                var bitmapImage = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();
                await bitmapImage.SetSourceAsync(ms.AsRandomAccessStream());
                RenderImage.Source = bitmapImage;
            }
            catch (System.Exception ex)
            {
                StatusTextBlock.Text = "Error: " + ex.Message;
            }
            finally
            {
                RenderButton.IsEnabled = true;
            }
        }

        private RayTracer.Core.Scenes.Scene CreateScene(string? name)
        {
            return name switch
            {
                "SphereScene" => new RayTracer.Core.Scenes.SphereScene(),
                "TorusScene" => new RayTracer.Core.Scenes.TorusScene(),
                "BoxScene" => new RayTracer.Core.Scenes.BoxScene(),
                _ => new RayTracer.Core.Scenes.SphereScene(),
            };
        }
    }
}
