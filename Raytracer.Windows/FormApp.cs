using System;
using System.Windows.Forms;

using RayTracer.Core;
using RayTracer.Core.Extensions;
using RayTracer.Core.Scenes;

namespace Raytracer.Windows
{
    public partial class FormApp : Form
    {
        public FormApp()
        {
            InitializeComponent();
        }

        private void buttonRender_Click(object sender, EventArgs e)
        {
            var engine = new Engine();

            engine.LoadScene(new SphereScene());

            var render = engine.Render(pboxRender.Width, pboxRender.Height);
            var image = ImageSharpExtensions.ToBitmap(render);

            pboxRender.Image = image;
        }
    }
}