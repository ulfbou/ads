# Architecture foundation

Production dependencies are fixed: Domain has none; Application depends on Domain; Infrastructure depends on Application and Domain; Host.Mcp depends on Application and Domain; App is the sole executable and composes all production projects.

The Robust Prototype uses `net10.0`, C# 14, nullable reference types, deterministic compilation, warnings as errors, central package management, and locked NuGet restore. Generated state under `.dx/` is non-authoritative.
