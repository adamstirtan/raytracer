using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;

using RayTracer.Core;
using RayTracer.Core.Extensions;
using RayTracer.Core.Scenes;

namespace Raytracer.Windows;

public partial class FormApp : Form
{
    private readonly Engine _engine;
    private readonly Timer _timer;

    private float _cameraAngle;

    public FormApp()
    {
        InitializeComponent();

        Scene scene = new SphereScene();
        RenderOptions options = ReadOptions();

        _engine = new(scene, options);

        _engine.RenderCompleted += RenderCompleted;

        _timer = new Timer
        {
            Interval = 1000
        };

        _timer.Tick += Timer_Tick;

        Render();
    }

    private void RenderCompleted(object sender, TimeSpan elapsed)
    {
        TextboxElapsed.Text = $"{(int)elapsed.TotalMilliseconds}";
    }

    private void Render()
    {
        // Update engine camera position from UI controls so runtime movement takes effect
        _engine.SetCameraPosition(new Vector3(
            (float)NumericCameraX.Value,
            (float)NumericCameraY.Value,
            (float)NumericCameraZ.Value));

        using var render = _engine.Render();
        using var image = ImageSharpExtensions.ToBitmap(render);

        pboxRender.Image = (Image)image.Clone();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _cameraAngle += 0.1f; // Adjust the angle increment as needed
        float radius = 2.0f; // Distance from the origin
        float x = radius * MathF.Cos(_cameraAngle);
        float y = radius * MathF.Sin(_cameraAngle);
        float z = radius * MathF.Cos(_cameraAngle);

        NumericCameraX.Value = (decimal)x;
        NumericCameraY.Value = (decimal)y;
        NumericCameraZ.Value = (decimal)z;

        Render();
    }

    private RenderOptions ReadOptions()
    {
        return new RenderOptions
        {
            Width = pboxRender.Width,
            Height = pboxRender.Height,
            CameraPosition = new(
                float.Parse(NumericCameraX.Value.ToString()),
                float.Parse(NumericCameraY.Value.ToString()),
                float.Parse(NumericCameraZ.Value.ToString())),
            TraceDepth = (int)NumericTraceDepth.Value,
            DisableReflections = CheckBoxDisableReflections.Checked,
            DisableDiffuse = CheckBoxDisableDiffuse.Checked,
            DisableSpeculation = CheckBoxDisableSpeculation.Checked
        };
    }

    private void CheckBoxDisableReflections_CheckedChanged(object sender, EventArgs e)
    {
        NumericTraceDepth.Enabled = !CheckBoxDisableReflections.Checked;

        Render();
    }

    private void NumericTraceDepth_ValueChanged(object sender, EventArgs e)
    {
        Render();
    }

    private void ButtonCameraLeft_Click(object sender, EventArgs e)
    {
        NumericCameraX.Value -= 0.25m;

        Render();
    }

    private void ButtonCameraRight_Click(object sender, EventArgs e)
    {
        NumericCameraX.Value += 0.25m;

        Render();
    }

    private void ButtonCameraForward_Click(object sender, EventArgs e)
    {
        NumericCameraZ.Value += 0.25m;

        Render();
    }

    private void ButtonCameraBackward_Click(object sender, EventArgs e)
    {
        NumericCameraZ.Value -= 0.25m;

        Render();
    }

    private void NumericCameraX_ValueChanged(object sender, EventArgs e)
    {
        Render();
    }

    private void NumericCameraY_ValueChanged(object sender, EventArgs e)
    {
        Render();
    }

    private void NumericCameraZ_ValueChanged(object sender, EventArgs e)
    {
        Render();
    }

    private void CheckBoxDisableDiffuse_CheckedChanged(object sender, EventArgs e)
    {
        Render();
    }

    private void CheckBoxDisableSpeculation_CheckedChanged(object sender, EventArgs e)
    {
        Render();
    }

    private void CheckBoxMoveAutomatically_CheckedChanged(object sender, EventArgs e)
    {
        bool @checked = CheckBoxMoveAutomatically.Checked;

        if (@checked)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }
    }
}