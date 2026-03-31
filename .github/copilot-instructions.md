# Copilot instructions for the RayTracer repository

This file gives quick, actionable guidance for Copilot sessions working on this repo.

## Quick commands
- Restore: `dotnet restore`
- Build entire solution: `dotnet build ./RayTracer.sln -c Release`
- Build a single project: `dotnet build ./RayTracer.Core/RayTracer.Core.csproj -c Release`
- Run all tests: `dotnet test ./RayTracer.Core.Tests/RayTracer.Core.Tests.csproj -c Release`
- Run a single test (exact):
  `dotnet test ./RayTracer.Core.Tests/RayTracer.Core.Tests.csproj --filter "FullyQualifiedName=RayTracer.Core.Tests.MyTests.TestMethod"`
- Run a single test (contains):
  `dotnet test ./RayTracer.Core.Tests/RayTracer.Core.Tests.csproj --filter "Name~TestMethod"`
- Run tests with coverage (coverlet):
  `dotnet test ./RayTracer.Core.Tests/RayTracer.Core.Tests.csproj --collect:"XPlat Code Coverage"`
- Publish client (CI example from .github/workflows/build.yml):
  `dotnet restore Raytracer.Client/RayTracer.Client.csproj && dotnet build Raytracer.Client/RayTracer.Client.csproj --no-restore -c Release && dotnet publish Raytracer.Client/RayTracer.Client.csproj --no-build -c Release -o ./published`
- Run Windows app (Windows only): `dotnet run --project ./Raytracer.Windows`

## High-level architecture
- Solution: `RayTracer.sln` ties the projects together.
- RayTracer.Core (net10.0): core rendering library. References SixLabors.ImageSharp and System.Drawing.Common. Textures/static assets are stored under `Textures/` and are copied to output (see csproj).
- RayTracer.Core.Tests (net10.0): MSTest test project that references RayTracer.Core. Uses `coverlet.collector` for coverage collection.
- Raytracer.Windows (net10.0-windows7.0): Windows Forms front-end (WinExe) that references RayTracer.Core.
- CI/workflow: `.github/workflows/build.yml` sets `NET_VERSION=10.0.100` and runs restore/build/publish for a client project (Raytracer.Client) and deploys to Azure WebApp `raytracer-as`.

## Key conventions
- Target frameworks: libraries target `net10.0`; the desktop UI targets `net10.0-windows7.0` (Windows-only).
- Static assets (textures) are declared in csproj with `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` to ensure they're available at runtime.
- Tests: MSTest is used. Use `dotnet test --filter` to run a single test locally (examples above).
- Coverage: `coverlet.collector` is present in the test project; use `--collect:"XPlat Code Coverage"` to produce coverage data.
- Package versions and dependencies are pinned in the csproj files; update package versions in the project file when making changes.
- Project/test naming: tests follow the `<ProjectName>.Tests` pattern and reference the core project directly.
- CI keeps `NET_VERSION` in workflow variables; if changing TFMs, update CI to match.

## Where to start
- Core rendering logic: `RayTracer.Core`
- Tests and verification: `RayTracer.Core.Tests`
- Desktop UI experiments: `Raytracer.Windows` (Windows)
- CI/publishing: `.github/workflows/build.yml`

If you'd like, Copilot can also:
- Add small CI helpers or developer launch scripts
- Configure MCP servers (Playwright) for client UI testing — ask below
