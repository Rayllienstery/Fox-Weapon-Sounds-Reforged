using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace FoxWeaponSoundsReforged;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.raylee.fox-weapon-sounds-reforged";
    public override string Name { get; init; } = "Fox-Weapon-Sounds-Reforged";
    public override string Author { get; init; } = "Raylee";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("0.1.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://github.com/Rayllienstery/Fox-Weapon-Sounds-Reforged";
    public override bool? IsBundleMod { get; init; } = true;
    public override string? License { get; init; } = "NCSA";
}

/// <summary>
/// Ports Fox Sound Mod Prefab.path remaps so the client loads Fox-overridden weapon
/// containers (which depend on *fox audio banks + *sounds packs) via bundles.json.
/// TT / SKS / VAL / VSS skipped (known asset breakage on newer clients).
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 100)]
public class FoxWeaponSoundsLoader(
    ISptLogger<FoxWeaponSoundsLoader> logger,
    DatabaseService databaseService) : IOnLoad
{
    static readonly Dictionary<string, string> PrefabRemaps = new()
    {
        ["5447a9cd4bdc2dbd208b4567"] = "assets/content/weapons/m4a1/weapon_colt_m4a1_556x45_container.bundle",
        ["5448bd6b4bdc2dfc2f8b4569"] = "assets/content/weapons/pm/weapon_izhmeh_pm_9x18pm_container.bundle",
        ["54491c4f4bdc2db1078b4568"] = "assets/content/weapons/mr133/weapon_izhmeh_mr133_12g_container.bundle",
        ["5644bd2b4bdc2d3b4c8b4572"] = "assets/content/weapons/ak74/weapon_izhmash_ak74n_545x39_container.bundle",
        ["56d59856d2720bd8418b456a"] = "assets/content/weapons/p226r/weapon_sig_p226r_9x19_container.bundle",
        ["56dee2bdd2720bc8328b4567"] = "assets/content/weapons/mr153/weapon_izhmeh_mr153_12g_container.bundle",
        ["56e0598dd2720bb5668b45a6"] = "assets/content/weapons/pb/weapon_tochmash_pb_9x18pm_container.bundle",
        ["576165642459773c7a400233"] = "assets/content/weapons/saiga12/weapon_izhmash_saiga12k_10_12g_container.bundle",
        ["576a581d2459771e7b1bc4f1"] = "assets/content/weapons/mp443/weapon_izhmeh_mp443_9x19_container.bundle",
        ["579204f224597773d619e051"] = "assets/content/weapons/pm/weapon_izhmeh_pm_threaded_9x18pm_container.bundle",
        ["57d14d2524597714373db789"] = "assets/content/weapons/pp-91/weapon_zmz_pp-91_9x18pm_container.bundle",
        ["57dc2fa62459775949412633"] = "assets/content/weapons/aks74u/weapon_izhmash_aks74u_545x39_container.bundle",
        ["57f3c6bd24597738e730fa2f"] = "assets/content/weapons/pp-91/weapon_zmz_pp-91-01_9x18pm_container.bundle",
        ["57f4c844245977379d5c14d1"] = "assets/content/weapons/pp-91/weapon_zmz_pp-9_9x18pmm_container.bundle",
        ["583990e32459771419544dd2"] = "assets/content/weapons/aks74u/weapon_izhmash_aks74un_545x39_container.bundle",
        ["5839a40f24597726f856b511"] = "assets/content/weapons/aks74u/weapon_izhmash_aks74ub_545x39_container.bundle",
        ["58948c8e86f77409493f7266"] = "assets/content/weapons/mpx/weapon_sig_mpx_9x19_container.bundle",
        ["5926bb2186f7744b1c6c6e60"] = "assets/content/weapons/mp5/weapon_hk_mp5_navy3_9x19_container.bundle",
        ["59984ab886f7743e98271174"] = "assets/content/weapons/pp-19-01/weapon_izhmash_pp-19-01_9x19_container.bundle",
        ["59d6088586f774275f37482f"] = "assets/content/weapons/akm/weapon_izhmash_akm_762x39_container.bundle",
        ["59e6152586f77473dc057aa1"] = "assets/content/weapons/akm/weapon_molot_vepr_km_vpo_136_762x39_container.bundle",
        ["59e6687d86f77411d949b251"] = "assets/content/weapons/akm/weapon_molot_akm_vpo_209_366tkm_container.bundle",
        ["59f98b4986f7746f546d2cef"] = "assets/content/weapons/sr1mp/weapon_tochmash_sr1mp_9x21_container.bundle",
        ["59ff346386f77477562ff5e2"] = "assets/content/weapons/akm/weapon_izhmash_akms_762x39_container.bundle",
        ["5a17f98cfcdbcb0980087290"] = "assets/content/weapons/aps/weapon_molot_aps_9x18pm_container.bundle",
        ["5a7828548dc32e5a9c28b516"] = "assets/content/weapons/m870/weapon_remington_model_870_12g_container.bundle",
        ["5a7ae0c351dfba0017554310"] = "assets/content/weapons/glock17/weapon_glock_glock_17_gen3_9x19_container.bundle",
        ["5aafa857e5b5b00018480968"] = "assets/content/weapons/m1a/weapon_springfield_m1a_762x51_container.bundle",
        ["5ab8e9fcd8ce870019439434"] = "assets/content/weapons/aks74/weapon_izhmash_aks74n_545x39_container.bundle",
        ["5abcbc27d8ce8700182eceeb"] = "assets/content/weapons/akm/weapon_izhmash_akmsn_762x39_container.bundle",
        ["5abccb7dd8ce87001773e277"] = "assets/content/weapons/aps/weapon_toz_apb_9x18pm_container.bundle",
        ["5ac4cd105acfc40016339859"] = "assets/content/weapons/ak74m/weapon_izhmash_ak74m_545x39_container.bundle",
        ["5ac66cb05acfc40198510a10"] = "assets/content/weapons/ak100/weapon_izhmash_ak101_556x45_container.bundle",
        ["5ac66d015acfc400180ae6e4"] = "assets/content/weapons/ak100/weapon_izhmash_ak102_556x45_container.bundle",
        ["5ac66d2e5acfc43b321d4b53"] = "assets/content/weapons/ak100/weapon_izhmash_ak103_762x39_container.bundle",
        ["5ac66d9b5acfc4001633997a"] = "assets/content/weapons/ak100/weapon_izhmash_ak105_545x39_container.bundle",
        ["5b0bbe4e5acfc40dc528a72d"] = "assets/content/weapons/sa58/weapon_dsa_sa58_762x51_container.bundle",
        ["5b1fa9b25acfc40018633c01"] = "assets/content/weapons/glock18c/weapon_glock_glock_18c_gen3_9x19_container.bundle",
        ["5ba26383d4351e00334c93d9"] = "assets/content/weapons/mp7/weapon_hk_mp7a1_46x30_container.bundle",
        ["5bb2475ed4351e00853264e3"] = "assets/content/weapons/hk416/weapon_hk_416a5_556x45_container.bundle",
        ["5bd70322209c4d00d7167b8f"] = "assets/content/weapons/mp7/weapon_hk_mp7a2_46x30_container.bundle",
        ["5beed0f50db834001c062b12"] = "assets/content/weapons/rpk16/weapon_izhmash_rpk16_545x39_container.bundle",
        ["5bf3e03b0db834001d2c4a9c"] = "assets/content/weapons/ak74/weapon_izhmash_ak74_545x39_container.bundle",
        ["5bf3e0490db83400196199af"] = "assets/content/weapons/ak100/weapon_izhmash_ak101_556x45_container.bundle",
        ["5bfea6e90db834001b7347f3"] = "assets/content/weapons/m700/weapon_remington_model_700_762x51_container.bundle",
        ["5c07c60e0db834002330051f"] = "assets/content/weapons/adar_2-15/weapon_adar_2-15_556x45_container.bundle",
        ["5c46fbd72e2216398b5a8c9c"] = "assets/content/weapons/svd/weapon_izhmash_svd_s_762x54_container.bundle",
        ["5c488a752e221602b412af63"] = "assets/content/weapons/mdr/weapon_dt_mdr_556x45_container.bundle",
        ["5c501a4d2e221602b412b540"] = "assets/content/weapons/vpo-101/weapon_molot_vepr_hunter_vpo-101_762x51_container.bundle",
        ["5cadc190ae921500103bb3b6"] = "assets/content/weapons/m9a3/weapon_beretta_m9a3_9x19_container.bundle",
        ["5cc82d76e24e8d00134b4b83"] = "assets/content/weapons/p90/weapon_fn_p90_57x28_container.bundle",
        ["5d2f0d8048f0356c925bc3b0"] = "assets/content/weapons/mp5/weapon_hk_mp5_kurtz_9x19_container.bundle",
        ["5d3eb3b0a4b93615055e84d2"] = "assets/content/weapons/fiveseven/weapon_fn_five_seven_57x28_container.bundle",
        ["5d43021ca4b9362eab4b5e25"] = "assets/content/weapons/tx15/weapon_lone_star_tx15_designated_marksman_556x45_container.bundle",
        ["5d67abc1a4b93614ec50137f"] = "assets/content/weapons/fiveseven/weapon_fn_five_seven_57x28_fde_container.bundle",
        ["5dcbd56fdbd3d91b3e5468d5"] = "assets/content/weapons/mdr/weapon_dt_mdr_762x51_container.bundle",
        ["5de7bd7bfd6b4e6e2276dc25"] = "assets/content/weapons/mp9/weapon_bt_mp9n_9x19_container.bundle",
        ["5e00903ae9dc277128008b87"] = "assets/content/weapons/mp9/weapon_bt_mp9_9x19_container.bundle",
        ["5e81c3cbac2bb513793cdc75"] = "assets/content/weapons/m1911a1/weapon_colt_m1911a1_1143x23_container.bundle",
        ["5e870397991fd70db46995c8"] = "assets/content/weapons/m590/weapon_mossberg_590a1_12g_container.bundle",
        ["5f36a0e5fbf956000b716b65"] = "assets/content/weapons/m1911a1/weapon_colt_m45a1_1143x23_container.bundle",
        ["5fb64bc92b1b027b1f50bcf2"] = "assets/content/weapons/vector/weapon_tdi_kriss_vector_gen_2_1143x23_container.bundle",
        ["5fbcc1d9016cce60e8341ab3"] = "assets/content/weapons/mcx/weapon_sig_mcx_gen1_762x35_container.bundle",
        ["5fc3e272f8b6a877a729eac5"] = "assets/content/weapons/ump/weapon_hk_ump_1143x23_container.bundle",
        ["5fc3f2d5900b1d5091531e57"] = "assets/content/weapons/vector/weapon_tdi_kriss_vector_gen_2_9x19_container.bundle",
        ["602a9740da11d6478d5a06dc"] = "assets/content/weapons/pl15/weapon_izhmash_pl15_9x19_container.bundle",
        ["60339954d62c9b14ed777c06"] = "assets/content/weapons/stm9/weapon_stmarms_stm_9_9x19_container.bundle",
        ["606587252535c57a13424cfd"] = "assets/content/weapons/mk47/weapon_cmmg_mk47_762x39_container.bundle",
        ["606dae0ab0e443224b421bb7"] = "assets/content/weapons/mp155/weapon_kalashnikov_mp155_12g_container.bundle",
        ["6165ac306ef05c2ce828ef74"] = "assets/content/weapons/scar/weapon_fn_mk17_762x51_fde_container.bundle",
        ["6176aca650224f204c1da3fb"] = "assets/content/weapons/g28/weapon_hk_g28_762x51_container.bundle",
        ["6183afd850224f204c1da514"] = "assets/content/weapons/scar/weapon_fn_mk17_762x51_container.bundle",
        ["6184055050224f204c1da540"] = "assets/content/weapons/scar/weapon_fn_mk16_556x45_container.bundle",
        ["618428466ef05c2ce828f218"] = "assets/content/weapons/scar/weapon_fn_mk16_556x45_fde_container.bundle",
        ["6193a720f8ee7e52e42109ed"] = "assets/content/weapons/usp/weapon_hk_usp_45_container.bundle"
    };

    public Task OnLoad()
    {
        var items = databaseService.GetItems();
        if (items is null)
        {
            logger.Error("[Fox-Weapon-Sounds-Reforged] items DB missing");
            return Task.CompletedTask;
        }

        var applied = 0;
        var missing = 0;
        foreach (var (tpl, path) in PrefabRemaps)
        {
            if (!items.TryGetValue(tpl, out var item) || item?.Properties?.Prefab is null)
            {
                missing++;
                continue;
            }

            item.Properties.Prefab.Path = path;
            applied++;
        }

        logger.Info(
            $"[Fox-Weapon-Sounds-Reforged] v0.1.0 Prefab remaps applied={applied}, missingTpl={missing} (bundle mod)");
        return Task.CompletedTask;
    }
}
