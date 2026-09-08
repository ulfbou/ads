$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..'))
if ((dotnet --version) -ne '10.0.400') { throw 'Expected .NET SDK 10.0.400.' }
& ./scripts/verify-dx-domain.ps1
if ($LASTEXITCODE) { throw 'Dx.Domain submodule verification failed.' }
dotnet restore AccountableDecisionSystem.slnx --locked-mode
if ($LASTEXITCODE) { throw 'Locked restore failed.' }
dotnet build AccountableDecisionSystem.slnx -c Release --no-restore -p:ContinuousIntegrationBuild=true -p:Deterministic=true
if ($LASTEXITCODE) { throw 'Release build failed.' }
dotnet test AccountableDecisionSystem.slnx -c Release --no-build --no-restore
if ($LASTEXITCODE) { throw 'Tests failed.' }
python scripts/write-dx-domain-integrity.py
if ($LASTEXITCODE) { throw 'Dx.Domain integrity evidence failed.' }
