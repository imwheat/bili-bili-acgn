using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>
/// 根据 <see cref="CardDefinition"/> 生成继承 <c>CardBaseModel</c> 的 C# 源码。
/// </summary>
public static class CardCodeGenerator
{
    private static readonly CultureInfo ZhCn = new("zh-CN");

    /// <summary>生成完整 .cs 文本。</summary>
    public static string GenerateSource(CardDefinition def, DateTime? createdAt = null, string? author = "wheat")
    {
        if (string.IsNullOrWhiteSpace(def.ClassName))
            throw new InvalidOperationException("className 不能为空。");

        var className = SanitizeClassName(def.ClassName.Trim());
        var ns = string.IsNullOrWhiteSpace(def.Namespace)
            ? "BiliBiliACGN.BiliBiliACGNCode.Cards"
            : def.Namespace.Trim();
        var pool = string.IsNullOrWhiteSpace(def.PoolTypeName) ? "ColorlessCardPool" : def.PoolTypeName.Trim();

        var t = createdAt ?? DateTime.Now;
        var timeLine = t.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) + " " +
                       t.ToString("dddd", ZhCn);
        var descSource = string.IsNullOrWhiteSpace(def.Description)
            ? def.Notes
            : def.Description;
        var desc = string.IsNullOrWhiteSpace(descSource)
            ? "卡牌类（由卡牌编辑器生成）"
            : descSource.Trim().Replace("\r\n", " ").Replace("\n", " ");
        if (desc.Length > 80)
            desc = desc[..80] + "…";

        var sb = new StringBuilder(4096);
        sb.AppendLine("//****************** 代码文件申明 ***********************");
        sb.AppendLine($"//* 文件：{className}");
        sb.AppendLine($"//* 作者：{author}");
        sb.AppendLine($"//* 创建时间：{timeLine}");
        sb.AppendLine($"//* 描述：{desc}");
        sb.AppendLine("//*******************************************************");
        sb.AppendLine();
        sb.AppendLine("using BaseLib.Utils;");
        sb.AppendLine("using MegaCrit.Sts2.Core.Commands;");
        sb.AppendLine("using MegaCrit.Sts2.Core.Entities.Cards;");
        sb.AppendLine("using MegaCrit.Sts2.Core.GameActions.Multiplayer;");
        sb.AppendLine("using MegaCrit.Sts2.Core.Localization.DynamicVars;");
        sb.AppendLine("using MegaCrit.Sts2.Core.Models.CardPools;");
        sb.AppendLine("using MegaCrit.Sts2.Core.ValueProps;");
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");
        sb.AppendLine();
        sb.AppendLine($"[Pool(typeof({pool}))]");
        sb.AppendLine($"public sealed class {className} : BiliBiliACGN.BiliBiliACGNCode.Cards.CardBaseModel");
        sb.AppendLine("{");
        sb.AppendLine($"    private const int energyCost = {def.EnergyCost};");
        sb.AppendLine($"    private const CardType type = CardType.{MapCardType(def.CardType)};");
        sb.AppendLine($"    private const CardRarity rarity = CardRarity.{MapRarity(def.Rarity)};");
        sb.AppendLine($"    private const TargetType targetType = TargetType.{MapTargetType(def.TargetType)};");
        sb.AppendLine($"    private const bool shouldShowInCardLibrary = {def.ShowInCardLibrary.ToString().ToLowerInvariant()};");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 牌面动态变量配置。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected override IEnumerable<DynamicVar> CanonicalVars =>");
        sb.AppendLine("    [");
        var vars = def.DynamicVars.Count > 0
            ? def.DynamicVars
            : [new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, UpgradeValue = 0m }];
        for (var i = 0; i < vars.Count; i++)
        {
            var comma = i < vars.Count - 1 ? "," : "";
            sb.AppendLine($"        {EmitCanonicalVar(vars[i])}{comma}");
        }
        sb.AppendLine("    ];");
        sb.AppendLine();
        sb.AppendLine($"    public {className}() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) {{ }}");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 出牌效果。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)");
        sb.AppendLine("    {");
        sb.AppendLine(EmitOnPlayBody(vars));
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 升级效果。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected override void OnUpgrade()");
        sb.AppendLine("    {");
        sb.AppendLine(EmitOnUpgradeBody(vars));
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>写入文件并返回绝对路径。</summary>
    public static string WriteGeneratedFile(CardDefinition def, string outputDirectory, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory))
            throw new ArgumentException("输出目录不能为空。", nameof(outputDirectory));

        var dir = Path.GetFullPath(outputDirectory.Trim());
        Directory.CreateDirectory(dir);

        var className = SanitizeClassName(def.ClassName.Trim());
        var path = Path.Combine(dir, $"{className}.cs");
        var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        File.WriteAllText(path, GenerateSource(def, createdAt), utf8WithBom);
        return path;
    }

    public static string SanitizeClassName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "GeneratedCard";
        var s = Regex.Replace(raw.Trim(), @"[^\p{L}\p{N}_]", "");
        if (s.Length == 0)
            return "GeneratedCard";
        if (char.IsDigit(s[0]))
            s = "Card_" + s;
        return s;
    }

    private static string MapCardType(string s) => s.Trim() switch
    {
        "Attack" => "Attack",
        "Skill" => "Skill",
        "Power" => "Power",
        "Status" => "Status",
        "Curse" => "Curse",
        "Quest" => "Quest",
        _ => "Attack"
    };

    private static string MapRarity(string s) => s.Trim() switch
    {
        "Common" => "Common",
        "Uncommon" => "Uncommon",
        "Rare" => "Rare",
        "Special" => "Special",
        "Curse" => "Curse",
        "Quest" => "Quest",
        _ => "Common"
    };

    private static string MapTargetType(string s) => s.Trim() switch
    {
        "None" => "None",
        "Self" => "Self",
        "AnyEnemy" => "AnyEnemy",
        "AllEnemies" => "AllEnemies",
        "RandomEnemy" => "RandomEnemy",
        "AnyPlayer" => "AnyPlayer",
        "AnyAlly" => "AnyAlly",
        "AllAllies" => "AllAllies",
        "TargetedNoCreature" => "TargetedNoCreature",
        "Osty" => "Osty",
        "Player" => "AnyPlayer",
        _ => "AnyEnemy"
    };

    private static string EmitCanonicalVar(DynamicVarEntry v)
    {
        var kind = v.Kind.Trim();
        var kl = kind.ToLowerInvariant();
        var bv = FormatDecimal(v.BaseValue);
        return kl switch
        {
            "damage" => $"new DamageVar({bv}m, {FormatValuePropForEmit(v.ValueProp, "damage")})",
            "block" => $"new BlockVar({bv}m, {FormatValuePropForEmit(v.ValueProp, "block")})",
            "cards" => $"new CardsVar({(int)v.BaseValue})",
            "repeat" => $"new DynamicVar(\"Repeat\", {bv}m)",
            "power" => $"new DynamicVar(\"Power\", {bv}m)",
            _ => $"new DynamicVar(\"{EscapeCSharpString(kind)}\", {bv}m)"
        };
    }

    /// <summary>生成源码中的 <c>ValueProp</c> 表达式；<paramref name="kindLower"/> 为 damage/block 时在未设置标志时沿用原默认。</summary>
    private static string FormatValuePropForEmit(ValueProp p, string kindLower)
    {
        if (p == ValueProp.None)
        {
            return kindLower switch
            {
                "damage" => "ValueProp.Move",
                "block" => "ValueProp.Unpowered",
                _ => "ValueProp.None"
            };
        }
        var parts = new List<string>();
        if (p.HasFlag(ValueProp.Move)) parts.Add("ValueProp.Move");
        if (p.HasFlag(ValueProp.Unpowered)) parts.Add("ValueProp.Unpowered");
        if (p.HasFlag(ValueProp.Unblockable)) parts.Add("ValueProp.Unblockable");
        if (p.HasFlag(ValueProp.SkipHurtAnim)) parts.Add("ValueProp.SkipHurtAnim");
        return parts.Count == 0 ? "ValueProp.None" : string.Join(" | ", parts);
    }

    private static string FormatDecimal(decimal d) =>
        d.ToString("0.############", CultureInfo.InvariantCulture);

    private static string EscapeCSharpString(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    private static string EmitOnPlayBody(IReadOnlyList<DynamicVarEntry> vars)
    {
        var hasDamage = vars.Any(v => v.Kind.Trim().Equals("Damage", StringComparison.OrdinalIgnoreCase));
        var hasBlock = vars.Any(v => v.Kind.Trim().Equals("Block", StringComparison.OrdinalIgnoreCase));
        var hasCards = vars.Any(v => v.Kind.Trim().Equals("Cards", StringComparison.OrdinalIgnoreCase));
        var blockEntry = vars.FirstOrDefault(v => v.Kind.Trim().Equals("Block", StringComparison.OrdinalIgnoreCase));

        var sb = new StringBuilder();
        if (hasDamage)
        {
            sb.AppendLine("        if (cardPlay.Target != null)");
            sb.AppendLine("        {");
            sb.AppendLine("            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)");
            sb.AppendLine("                .FromCard(this)");
            sb.AppendLine("                .Targeting(cardPlay.Target)");
            sb.AppendLine("                .Execute(choiceContext);");
            sb.AppendLine("        }");
        }
        if (hasCards)
        {
            if (hasDamage)
                sb.AppendLine();
            sb.AppendLine("        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);");
        }
        if (hasBlock && !hasDamage && !hasCards)
        {
            var blockVp = FormatValuePropForEmit(blockEntry?.ValueProp ?? ValueProp.None, "block");
            sb.AppendLine($"        await CreatureCmd.GainBlock(base.Owner, base.DynamicVars.Block.BaseValue, {blockVp}, null);");
        }
        if (!hasDamage && !hasCards && !hasBlock)
        {
            sb.AppendLine("        await Task.CompletedTask;");
            sb.AppendLine("        // TODO: 根据 kind 编排 OnPlay（或补充 Damage / Block / Cards 变量）");
        }
        else if (hasBlock && (hasDamage || hasCards))
        {
            sb.AppendLine();
            sb.AppendLine("        // TODO: 如需格挡效果，请手动编排 CreatureCmd.GainBlock 与上方顺序");
        }

        if (sb.Length == 0)
            sb.AppendLine("        await Task.CompletedTask;");

        return sb.ToString().TrimEnd();
    }

    private static string EmitOnUpgradeBody(IReadOnlyList<DynamicVarEntry> vars)
    {
        var sb = new StringBuilder();
        var any = false;
        foreach (var v in vars)
        {
            if (v.UpgradeValue == 0m)
                continue;
            var u = FormatDecimal(v.UpgradeValue);
            var line = EmitUpgradeLine(v, u);
            if (line != null)
            {
                sb.AppendLine(line);
                any = true;
            }
        }
        if (!any)
            sb.AppendLine("        // 无升级数值（upgradeValue 均为 0）");
        return sb.ToString().TrimEnd();
    }

    private static string? EmitUpgradeLine(DynamicVarEntry v, string upgradeDecimal)
    {
        var kind = v.Kind.Trim();
        switch (kind.ToLowerInvariant())
        {
            case "damage":
                return $"        base.DynamicVars.Damage.UpgradeValueBy({upgradeDecimal}m);";
            case "block":
                return $"        base.DynamicVars.Block.UpgradeValueBy({upgradeDecimal}m);";
            case "cards":
                return $"        base.DynamicVars.Cards.UpgradeValueBy({upgradeDecimal}m);";
            case "repeat":
                return $"        base.DynamicVars[\"Repeat\"].UpgradeValueBy({upgradeDecimal}m);";
            case "power":
                return $"        base.DynamicVars[\"Power\"].UpgradeValueBy({upgradeDecimal}m);";
            default:
                var key = EscapeCSharpString(kind);
                return $"        base.DynamicVars[\"{key}\"].UpgradeValueBy({upgradeDecimal}m);";
        }
    }
}
