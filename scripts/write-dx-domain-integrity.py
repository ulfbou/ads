#!/usr/bin/env python3
import hashlib
import json
import subprocess
from pathlib import Path

root = Path(__file__).resolve().parent.parent
submodule = root / "external" / "dx-domain"
output = root / ".dx" / "ads" / "qualification" / "dx-domain-integrity.json"
projects = [
    "src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj",
    "src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj",
    "src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj",
    "src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj",
]

def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()

def git(*args: str) -> str:
    return subprocess.check_output(["git", "-C", str(submodule), *args], text=True).strip()

assets = []
for path in sorted(submodule.glob("src/*/obj/project.assets.json")):
    document = json.loads(path.read_text(encoding="utf-8-sig"))
    packages = []
    for identity, details in sorted(document.get("libraries", {}).items()):
        if details.get("type") == "package":
            packages.append({
                "identity": identity,
                "sha512": details.get("sha512"),
                "path": details.get("path"),
            })
    assets.append({"path": path.relative_to(submodule).as_posix(), "packages": packages})

if not assets:
    raise SystemExit("No restored Dx.Domain project.assets.json files were found.")
missing_hashes = [p["identity"] for a in assets for p in a["packages"] if not p["sha512"]]
if missing_hashes:
    raise SystemExit("Missing NuGet content hashes: " + ", ".join(missing_hashes))

record = {
    "schemaVersion": "1.0",
    "repository": "https://github.com/ulfbou/Dx.Domain.git",
    "path": "external/dx-domain",
    "commit": git("rev-parse", "HEAD"),
    "tree": git("rev-parse", "HEAD^{tree}"),
    "license": {"path": "LICENSE", "sha256": digest(submodule / "LICENSE")},
    "projects": [{"path": p, "sha256": digest(submodule / p)} for p in projects],
    "resolvedPackages": assets,
}
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(json.dumps(record, indent=2, sort_keys=True) + "\n", encoding="utf-8", newline="\n")
print(output.relative_to(root).as_posix())
