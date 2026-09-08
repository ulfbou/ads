#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$root"
path='external/dx-domain'
expected_url='https://github.com/ulfbou/Dx.Domain.git'

test -f .gitmodules
test "$(git config -f .gitmodules --get submodule.external/dx-domain.path)" = "$path"
test "$(git config -f .gitmodules --get submodule.external/dx-domain.url)" = "$expected_url"
expected_commit="$(git ls-files --stage -- "$path" | awk '$1 == "160000" { print $2 }')"
test -n "$expected_commit"
test -e "$path/.git"
actual_commit="$(git -C "$path" rev-parse HEAD)"
test "$actual_commit" = "$expected_commit"
test -z "$(git -C "$path" status --porcelain)"

for project in \
  src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj \
  src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj \
  src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj \
  src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj
do
  test -f "$path/$project"
done

test -f "$path/LICENSE"
if test -f "$path/.gitmodules"; then
  nested_count="$(git -C "$path" config -f .gitmodules --get-regexp '^submodule\..*\.path$' 2>/dev/null | wc -l | tr -d ' ')"
  test "$nested_count" = '0'
fi
