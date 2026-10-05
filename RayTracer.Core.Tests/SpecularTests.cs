using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class SpecularTests
{
    [TestMethod]
    public void SlightlyLongNormal_DoesNotAmplifySharpHighlight()
    {
        var scene = new Scene();
        scene.AddObject(new ImperfectNormalPlane());
        scene.AddLight(new Light(Vector3.Zero, float.MinValue, new Material(new Vector3(.25f))));
        using var image = new Engine(scene, Options()).Render();
        // Unit alignment gives .2 * .25 = .05, independent of the normal's length.
        Assert.IsTrue(image[0,0].R >= 12 && image[0,0].R <= 14);
    }

    [TestMethod]
    public void LightBehindSurface_DoesNotProduceSpecularHighlight()
    {
        var scene = new Scene();
        scene.AddObject(new RayTracer.Core.Primitives.Plane(-Vector3.UnitZ, 5,
            new Material(Vector3.One,0,0,.8f){Shininess=0},null));
        scene.AddLight(new Light(new Vector3(0,0,6),float.MinValue,new Material(Vector3.One)));
        using var image = new Engine(scene, Options()).Render();
        Assert.AreEqual((byte)0,image[0,0].R);
    }

    private static RenderOptions Options() => new()
    {
        Width=1,Height=1,TraceDepth=1,DisableReflections=true,
        CameraPosition=Vector3.Zero,CameraTarget=Vector3.UnitZ
    };

    private sealed class ImperfectNormalPlane() : RayTracer.Core.Primitives.Plane(-Vector3.UnitZ,5,
        new Material(Vector3.One,0,0,.2f){Shininess=96},null)
    {
        public override Vector3 GetNormal(Vector3 position) => -Vector3.UnitZ * 1.01f;
    }
}
