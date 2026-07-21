from pathlib import Path
import json

repo = Path(r"c:\Games\SPT\dev\Fox-Weapon-Sounds-Reforged")
data = json.loads((repo / "mapping" / "prefab_remaps_filtered.json").read_text(encoding="utf-8"))
remaps = data["remaps"]

proj_dir = repo / "FoxWeaponSoundsReforged"
proj_dir.mkdir(exist_ok=True)

csproj = """<Project Sdk=\"Microsoft.NET.Sdk.Web\">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>FoxWeaponSoundsReforged</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <OutputType>Library</OutputType>
    <Version>0.1.0</Version>
    <AssemblyName>Fox-Weapon-Sounds-Reforged</AssemblyName>
    <OutputPath>bin\\$(Configuration)\\$(AssemblyName)\\</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=\"SPTarkov.Common\" Version=\"4.0.1\" />
    <PackageReference Include=\"SPTarkov.DI\" Version=\"4.0.1\" />
    <PackageReference Include=\"SPTarkov.Server.Core\" Version=\"4.0.1\" />
  </ItemGroup>

</Project>
"""
(proj_dir / "FoxWeaponSoundsReforged.csproj").write_text(csproj, encoding="utf-8")

entries = ",\n".join(f'        ["{r["tpl"]}"] = "{r["path"]}"' for r in remaps)

loader = f"""using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace FoxWeaponSoundsReforged;

public record ModMetadata : AbstractModMetadata
{{
    public override string ModGuid {{ get; init; }} = "com.raylee.fox-weapon-sounds-reforged";
    public override string Name {{ get; init; }} = "Fox-Weapon-Sounds-Reforged";
    public override string Author {{ get; init; }} = "Raylee";
    public override List<string>? Contributors {{ get; init; }} = ["Fox", "Krinkova"];
    public override SemanticVersioning.Version Version {{ get; init; }} = new("0.1.0");
    public override SemanticVersioning.Range SptVersion {{ get; init; }} = new("~4.0.0");
    public override List<string>? Incompatibilities {{ get; init; }}
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies {{ get; init; }}
    public override string? Url {{ get; init; }} = "https://github.com/Rayllienstery/Fox-Weapon-Sounds-Reforged";
    public override bool? IsBundleMod {{ get; init; }} = true;
    public override string? License {{ get; init; }} = "NCSA";
}}

/// <summary>
/// Ports Fox Sound Mod Prefab.path remaps so the client loads Fox-overridden weapon
/// containers (which depend on *fox audio banks + *sounds packs) via bundles.json.
/// TT / SKS / VAL / VSS skipped (known asset breakage on newer clients).
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 100)]
public class FoxWeaponSoundsLoader(
    ISptLogger<FoxWeaponSoundsLoader> logger,
    DatabaseService databaseService) : IOnLoad
{{
    static readonly Dictionary<string, string> PrefabRemaps = new()
    {{
{entries}
    }};

    public Task OnLoad()
    {{
        var items = databaseService.GetItems();
        if (items is null)
        {{
            logger.Error("[Fox-Weapon-Sounds-Reforged] items DB missing");
            return Task.CompletedTask;
        }}

        var applied = 0;
        var missing = 0;
        foreach (var (tpl, path) in PrefabRemaps)
        {{
            if (!items.TryGetValue(tpl, out var item) || item?.Properties?.Prefab is null)
            {{
                missing++;
                continue;
            }}

            item.Properties.Prefab.Path = path;
            applied++;
        }}

        logger.Info(
            $"[Fox-Weapon-Sounds-Reforged] v0.1.0 Prefab remaps applied={{applied}}, missingTpl={{missing}} (bundle mod)");
        return Task.CompletedTask;
    }}
}}
"""
(proj_dir / "FoxWeaponSoundsLoader.cs").write_text(loader, encoding="utf-8")
print("wrote loader remaps", len(remaps))
