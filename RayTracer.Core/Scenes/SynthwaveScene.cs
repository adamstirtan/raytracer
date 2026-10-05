using System;
using System.IO;
using System.Numerics;
using RayTracer.Core.Materials;
using RayTracer.Core.Math;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>A luminous grid valley beneath a pink sunset and purple star field.</summary>
public class SynthwaveScene : Scene
{
    private readonly Valley valley;
    private readonly Sky sky;
    private readonly GridGround? ground;
    public SynthwaveScene(bool distantMountains = false)
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "Textures", "Synthwave");
        Texture Load(string name) => new(Path.Combine(directory, name + ".png"));
        valley = new Valley(Load("grid"), distantMountains);
        sky = new Sky(Load("sky"));
        if (distantMountains)
        {
            sky.D = 26000;
            ground = new GridGround(Load("grid"));
            AddObject(ground);
        }
        AddObject(valley);
        var sunset = new Sunset(Load("sun"));
        if (distantMountains) { sunset.Center *= 10; sunset.Radius *= 10; }
        AddObject(sunset);
        AddObject(sky);
        Camera.Target = new Vector3(0, 9, 100);
    }

    private sealed class GridGround(Texture texture) : Primitives.Plane(Vector3.UnitY, 0,
        new Material(Vector3.One) { Emission = 1 }, texture)
    {
        public float Travel { get; set; }
        public override Vector2 GetUV(Vector3 position) => new(position.X / 4, (position.Z + Travel) / 6);
    }

    public void SetAnimationProgress(float progress)
    {
        if (!float.IsFinite(progress) || progress < 0 || progress > 1)
            throw new ArgumentOutOfRangeException(nameof(progress));
        float phase = progress == 1 ? 0 : progress;
        if (ground == null) valley.SetPhase(phase);
        else ground.Travel = phase * 240;
        sky.Phase = phase;
    }

    private sealed class Sunset(Texture texture) : Disk(new Vector3(0, 340, 2000), -Vector3.UnitZ, 300,
        new Material(Vector3.One) { Emission = 1 }, texture)
    {
        public override Vector2 GetUV(Vector3 position)
            => new((position.X - Center.X) / (Radius * 2) + .5f,
                .5f - (position.Y - Center.Y) / (Radius * 2));
    }

    private sealed class Sky(Texture texture) : Primitives.Plane(-Vector3.UnitZ, 2600,
        new Material(Vector3.One) { Emission = 1 }, texture)
    {
        public float Phase { get; set; }
        public override Vector2 GetUV(Vector3 position)
            => new((position.X / (D / 2600) + 2100) / 4200 + .025f * MathF.Sin(Phase * MathF.Tau),
                System.Math.Clamp((1300 - position.Y / (D / 2600)) / 1400, .001f, .999f));
    }

    // Walk only the grid cells crossed by a ray instead of searching every terrain triangle.
    private sealed class Valley : Primitive
    {
        private const int Columns = 80, Rows = 44;
        private readonly float MinX, MinZ, StepX, StepZ;
        private readonly float scale;
        private readonly Vector3[,] vertices = new Vector3[Columns + 1, Rows + 1];

        private float travel;
        public Valley(Texture texture, bool distantMountains) : base(new Material(Vector3.One) { Emission = 1 }, texture)
        {
            scale = distantMountains ? 10 : 1;
            MinX = -160 * scale; MinZ = distantMountains ? 500 : -24;
            StepX = 4 * scale; StepZ = 6 * scale;
            SetPhase(0);
        }

        public void SetPhase(float phase)
        {
            travel = phase * 240;
            for (int x = 0; x <= Columns; x++)
                for (int z = 0; z <= Rows; z++)
                {
                    float px = MinX + x * StepX, pz = MinZ + z * StepZ;
                    float localX = px / scale, localZ = pz / scale;
                    float shoulder = MathF.Max(0, MathF.Abs(localX) - (scale > 1 ? 20 : 10));
                    float angle = (localZ + travel) / 240 * MathF.Tau;
                    float ridge = 1 + .32f * MathF.Sin(angle * 6 + localX * .12f)
                        + .24f * MathF.Sin(angle * 12 - localX * .23f);
                    float height = MathF.Min(shoulder * .55f, 27) * ridge
                        + MathF.Min(shoulder * .16f, 5) * (.5f + .5f * MathF.Sin(angle * 17 + localX * 1.73f));
                    vertices[x, z] = new Vector3(px, scale > 1 && z == 0 ? 0 : height * scale, pz);
                }
        }

        public override PrimitiveType GetPrimitiveType() => PrimitiveType.Mesh;
        public override Vector2 GetUV(Vector3 position)
            => new((position.X - MinX) * 2 / StepX, (position.Z - MinZ + travel) * 2 / StepZ);

        public override IntersectionResult Intersect(Ray ray, float maxDistance)
        {
            float start = 0, end = maxDistance;
            bool Clip(float origin, float direction, float min, float max)
            {
                if (direction == 0) return origin >= min && origin <= max;
                float a = (min - origin) / direction, b = (max - origin) / direction;
                if (a > b) (a, b) = (b, a);
                start = MathF.Max(start, a); end = MathF.Min(end, b);
                return start <= end;
            }
            if (!Clip(ray.Origin.X, ray.Direction.X, MinX, MinX + Columns * StepX)
                || !Clip(ray.Origin.Z, ray.Direction.Z, MinZ, MinZ + Rows * StepZ))
                return new(RayIntersection.Miss, maxDistance);
            float t = MathF.Max(start, 0) + .0001f;
            Vector3 p = ray.Origin + ray.Direction * t;
            int x = System.Math.Clamp((int)MathF.Floor((p.X - MinX) / StepX), 0, Columns - 1);
            int z = System.Math.Clamp((int)MathF.Floor((p.Z - MinZ) / StepZ), 0, Rows - 1);
            int dx = ray.Direction.X >= 0 ? 1 : -1, dz = ray.Direction.Z >= 0 ? 1 : -1;
            float nextX = ray.Direction.X == 0 ? float.PositiveInfinity
                : (MinX + (x + (dx > 0 ? 1 : 0)) * StepX - ray.Origin.X) / ray.Direction.X;
            float nextZ = ray.Direction.Z == 0 ? float.PositiveInfinity
                : (MinZ + (z + (dz > 0 ? 1 : 0)) * StepZ - ray.Origin.Z) / ray.Direction.Z;
            float deltaX = StepX / MathF.Abs(ray.Direction.X), deltaZ = StepZ / MathF.Abs(ray.Direction.Z);
            while (x >= 0 && x < Columns && z >= 0 && z < Rows && t <= end)
            {
                Vector3 a = vertices[x,z], b = vertices[x+1,z], c = vertices[x,z+1], d = vertices[x+1,z+1];
                var first = HitTriangle(ray, a, c, b, maxDistance);
                var second = HitTriangle(ray, b, c, d, first.Distance);
                if (second.RayIntersection == RayIntersection.Hit) return second;
                if (first.RayIntersection == RayIntersection.Hit) return first;
                if (float.IsPositiveInfinity(nextX) && float.IsPositiveInfinity(nextZ)) break;
                if (nextX < nextZ) { t = nextX; nextX += deltaX; x += dx; }
                else { t = nextZ; nextZ += deltaZ; z += dz; }
            }
            return new(RayIntersection.Miss, maxDistance);
        }

        private static IntersectionResult HitTriangle(Ray ray, Vector3 a, Vector3 b, Vector3 c, float limit)
        {
            Vector3 e1 = b-a, e2 = c-a, cross = Vector3.Cross(ray.Direction, e2);
            float determinant = Vector3.Dot(e1, cross);
            if (MathF.Abs(determinant) < 1e-8f) return new(RayIntersection.Miss, limit);
            Vector3 offset = ray.Origin-a;
            float u = Vector3.Dot(offset, cross) / determinant;
            Vector3 q = Vector3.Cross(offset, e1);
            float v = Vector3.Dot(ray.Direction, q) / determinant;
            float t = Vector3.Dot(e2, q) / determinant;
            if (u < 0 || v < 0 || u+v > 1 || t <= 1e-6f || t >= limit)
                return new(RayIntersection.Miss, limit);
            return new(RayIntersection.Hit, t, Vector3.Normalize(Vector3.Cross(e1,e2)));
        }

        public override RayIntersection Intersects(Ray ray, ref float distance)
        {
            var hit = Intersect(ray, distance);
            distance = hit.Distance;
            return hit.RayIntersection;
        }

        public override Vector3 GetNormal(Vector3 position)
        {
            int x = System.Math.Clamp((int)MathF.Floor((position.X-MinX)/StepX),0,Columns-1);
            int z = System.Math.Clamp((int)MathF.Floor((position.Z-MinZ)/StepZ),0,Rows-1);
            float u = (position.X-MinX)/StepX-x, v = (position.Z-MinZ)/StepZ-z;
            Vector3 a=vertices[x,z], b=vertices[x+1,z], c=vertices[x,z+1], d=vertices[x+1,z+1];
            return u+v <= 1 ? Vector3.Normalize(Vector3.Cross(c-a,b-a))
                : Vector3.Normalize(Vector3.Cross(c-b,d-b));
        }
    }
}
