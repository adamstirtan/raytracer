using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;

namespace RayTracer.Core.Tests;

[TestClass]
public class ChromaticSceneTests
{
    [TestMethod]
    public void Garden_Contains360SeparatedReflectiveSpheresAboveFloor()
    {
        var spheres = new ChromaticScene().OfType<Sphere>().Where(s=>s is not Light).ToArray();
        Assert.AreEqual(360,spheres.Length);
        for(int i=0;i<spheres.Length;i++)
        {
            Assert.IsTrue(spheres[i].Center.Y >= spheres[i].Radius);
            Assert.IsTrue(spheres[i].Material.Reflection >= .48f);
            for(int j=i+1;j<spheres.Length;j++)
                Assert.IsTrue(Vector3.Distance(spheres[i].Center,spheres[j].Center)
                    >= spheres[i].Radius+spheres[j].Radius+.119f,$"Spheres {i} and {j} overlap.");
        }
    }
}
