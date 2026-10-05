using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Tests;

[TestClass]
public class ObjLoaderTests
{
    [TestMethod]
    public void NegativeVertexAndNormalIndices_AreResolvedRelativeToTheirLists()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "v 0 0 0\nv 1 0 0\nv 0 1 0\nvn 0 0 1\nf -3//-1 -2//-1 -1//-1\n");
            Mesh mesh = Mesh.FromObj(path, new Material(Vector3.One));
            Assert.AreEqual((0, 1, 2), mesh.Triangles[0]);
            Assert.AreEqual(Vector3.UnitZ, mesh.VertexNormals[0]);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void DecimalCoordinates_AreIndependentOfCurrentCulture()
    {
        string path = Path.GetTempFileName();
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            File.WriteAllText(path, "v 0.5 0 0\nv 1.5 0 0\nv 0.5 1 0\nf 1 2 3\n");
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Mesh expected = Mesh.FromObj(path, new Material(Vector3.One));
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Mesh actual = Mesh.FromObj(path, new Material(Vector3.One));
            CollectionAssert.AreEqual(expected.Vertices, actual.Vertices);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
            File.Delete(path);
        }
    }
}
