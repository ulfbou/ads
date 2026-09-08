#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$root"
[ "$(dotnet --version)" = '10.0.400' ]
./scripts/verify-dx-domain.sh
dotnet restore AccountableDecisionSystem.slnx --locked-mode
dotnet build AccountableDecisionSystem.slnx -c Release --no-restore -p:ContinuousIntegrationBuild=true -p:Deterministic=true
dotnet test AccountableDecisionSystem.slnx -c Release --no-build --no-restore
python3 scripts/write-dx-domain-integrity.py
