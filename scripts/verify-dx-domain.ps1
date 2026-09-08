$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..'))
$path = 'external/dx-domain'
$expectedUrl = 'https://github.com/ulfbou/Dx.Domain.git'
if (-not (Test-Path '.gitmodules')) { throw '.gitmodules is missing.' }
if ((git config -f .gitmodules --get submodule.external/dx-domain.path) -ne $path) { throw 'Dx.Domain submodule path mismatch.' }
if ((git config -f .gitmodules --get submodule.external/dx-domain.url) -ne $expectedUrl) { throw 'Dx.Domain submodule URL mismatch.' }
$entry = git ls-files --stage -- $path
if ($entry -notmatch '^160000 ([0-9a-f]{40}) 0\t') { throw 'Dx.Domain gitlink is missing.' }
$expectedCommit = $Matches[1]
if (-not (Test-Path "$path/.git")) { throw 'Dx.Domain submodule is not initialized.' }
if ((git -C $path rev-parse HEAD) -ne $expectedCommit) { throw 'Dx.Domain commit does not match the gitlink.' }
if (git -C $path status --porcelain) { throw 'Dx.Domain submodule is dirty.' }
@(
  'src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj',
  'src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj',
  'src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj',
  'src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj',
  'LICENSE'
) | ForEach-Object { if (-not (Test-Path "$path/$_")) { throw "Missing admitted Dx.Domain path: $_" } }
