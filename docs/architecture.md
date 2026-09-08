# Architecture foundation

Production dependencies are fixed: Domain has none; Application depends on Domain; Infrastructure depends on Application and Domain; Host.Mcp depends on Application and Domain; App is the sole executable and composes all production projects.

The Robust Prototype uses `net10.0`, C# 14, nullable reference types, deterministic compilation, warnings as errors, central package management, and locked NuGet restore. Generated state under `.dx/` is non-authoritative.

## Domain boundary

The Domain project owns transport-neutral actors, runtime roles, operating modes, ledger record semantics and payloads, lifecycle identities, evidence references, reflection findings, operation and transaction identities, terminal outcomes, mutation certainty, persistence completion, runtime dispositions, ledger states, and diagnostics.

Domain construction rejects malformed identities through deterministic typed results. Domain public APIs expose only BCL and Domain-owned types. The Domain project has no production project references, external package references, JSON, MCP, filesystem, environment, persistence, or runtime-composition concerns.

The ledger recognizes observation, decision, outcome, reflection, proposal, knowledge-candidate, review, publication, and correction records. The Robust Prototype write boundary is limited to observation, decision, outcome, reflection, and proposal. Proposals are explicitly non-authoritative. Reflection classification keeps causal, contributing, and correlation findings distinct.

## Qualified S0 accountability substrate

WP17-02 integrates one exact-commit `Dx.Domain` Git submodule at `external/dx-domain`. ADS admits only `Dx.Domain.Kernel`, `Dx.Domain.Primitives`, `Dx.Domain.Annotations`, and their associated build-time analyzers. ADS retains exclusive authority over ledger semantics, lifecycle, roles, modes, diagnostics, recovery, and product outcomes. Repository validation verifies the canonical URL, exact gitlink commit, clean initialized state, admitted project paths, license hash, project hashes, and restored NuGet SHA-512 content hashes.
