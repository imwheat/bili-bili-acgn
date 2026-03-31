using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CardEditor.Shared.CardPlayActionEmit;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>
/// 根据 <see cref="CardDefinition"/> 生成继承 <c>CardBaseModel</c> 的 C# 源码。
/// 通过 <c>#region</c> 分为：Card properties、CardPlayAction、OnUpgrade；更新已有 .cs 时按 region 分别替换或插入缺失块。
/// </summary>
public static class CardCodeGenerator
{
    private static readonly CultureInfo ZhCn = new("zh-CN");

    public const string RegionCardProperties = "卡牌属性配置";
    public const string RegionCardPlayAction = "卡牌打出效果";
    public const string RegionOnUpgrade = "升级效果";

    private const string IndentClass = "    ";
    private const string IndentMethod = "        ";

    /// <summary>生成完整 .cs 文本（含三个 <c>#region</c>）。<paramref name="author"/> 为空或空白时使用系统用户名。</summary>
    public static string GenerateSource(CardDefinition def, DateTime? createdAt = null, string? author = null)
    {
        if (string.IsNullOrWhiteSpace(def.ClassName))
            throw new InvalidOperationException("className 不能为空。");

        var resolvedAuthor = ResolveAuthor(author);
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

        var sb = new StringBuilder(8192);
        sb.AppendLine("//****************** 代码文件申明 ***********************");
        sb.AppendLine($"//* 文件：{className}");
        sb.AppendLine($"//* 作者：{resolvedAuthor}");
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
        sb.AppendLine("using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;");
        sb.AppendLine("using MegaCrit.Sts2.Core.ValueProps;");
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");
        sb.AppendLine();
        sb.AppendLine($"[Pool(typeof({pool}))]");
        sb.AppendLine($"public sealed class {className} : CardBaseModel");
        sb.AppendLine("{");
        sb.AppendLine($"{IndentClass}#region {RegionCardProperties}");
        sb.Append(BuildCardPropertiesInner(def, className));
        sb.AppendLine($"{IndentClass}#endregion");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 出牌效果。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)");
        sb.AppendLine("    {");
        sb.AppendLine($"{IndentMethod}#region {RegionCardPlayAction}");
        sb.Append(BuildCardPlayActionInner(def));
        sb.AppendLine($"{IndentMethod}#endregion");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 升级效果。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected override void OnUpgrade()");
        sb.AppendLine("    {");
        sb.AppendLine($"{IndentMethod}#region {RegionOnUpgrade}");
        sb.Append(BuildOnUpgradeInner(def));
        sb.AppendLine($"{IndentMethod}#endregion");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// 写入 .cs：已存在文件时对每个命名 region 单独处理——有则只替换 region 内正文，无则在约定锚点插入整段 <c>#region … #endregion</c>；不存在文件时写入完整生成内容。
    /// </summary>
    public static string WriteGeneratedFile(CardDefinition def, string outputDirectory, DateTime? createdAt = null, string? author = null)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory))
            throw new ArgumentException("输出目录不能为空。", nameof(outputDirectory));

        var dir = Path.GetFullPath(outputDirectory.Trim());
        Directory.CreateDirectory(dir);

        var className = SanitizeClassName(def.ClassName.Trim());
        var path = Path.Combine(dir, $"{className}.cs");
        var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var resolvedAuthor = ResolveAuthor(author);

        if (File.Exists(path))
        {
            var existing = File.ReadAllText(path, utf8WithBom);
            var merged = MergeRegionsIntoExisting(existing, def, className);
            File.WriteAllText(path, merged, utf8WithBom);
            return path;
        }

        File.WriteAllText(path, GenerateSource(def, createdAt, resolvedAuthor), utf8WithBom);
        return path;
    }

    private static string ResolveAuthor(string? author) =>
        string.IsNullOrWhiteSpace(author) ? Environment.UserName : author.Trim();

    /// <summary>对已存在源码逐个 region 更新：存在的 region 只替换内部；不存在的在锚点插入整段 region（插入按索引从大到小执行）。</summary>
    private static string MergeRegionsIntoExisting(string existing, CardDefinition def, string className)
    {
        var result = existing;
        var cardInner = BuildCardPropertiesInner(def, className);
        var playInner = BuildCardPlayActionInner(def);
        var upgradeInner = BuildOnUpgradeInner(def);

        if (RegionExists(result, RegionCardProperties, IndentClass))
            TryReplaceRegionContent(result, RegionCardProperties, IndentClass, cardInner, out result);
        if (RegionExists(result, RegionCardPlayAction, IndentMethod))
            TryReplaceRegionContent(result, RegionCardPlayAction, IndentMethod, playInner, out result);
        if (RegionExists(result, RegionOnUpgrade, IndentMethod))
            TryReplaceRegionContent(result, RegionOnUpgrade, IndentMethod, upgradeInner, out result);

        var inserts = new List<(int Index, string Text)>();
        if (!RegionExists(result, RegionCardProperties, IndentClass) &&
            TryFindInsertIndexAfterClassOpenBrace(result, out var idxClass))
            inserts.Add((idxClass, FormatRegionBlock(RegionCardProperties, IndentClass, cardInner)));
        if (!RegionExists(result, RegionCardPlayAction, IndentMethod) &&
            TryFindInsertIndexAfterOnPlayOpenBrace(result, out var idxPlay))
            inserts.Add((idxPlay, FormatRegionBlock(RegionCardPlayAction, IndentMethod, playInner)));
        if (!RegionExists(result, RegionOnUpgrade, IndentMethod) &&
            TryFindInsertIndexAfterOnUpgradeOpenBrace(result, out var idxUp))
            inserts.Add((idxUp, FormatRegionBlock(RegionOnUpgrade, IndentMethod, upgradeInner)));

        inserts.Sort((a, b) => b.Index.CompareTo(a.Index));
        foreach (var (index, text) in inserts)
            result = result.Insert(index, text);

        return result;
    }

    private static bool RegionExists(string source, string regionName, string indent) =>
        source.IndexOf($"{indent}#region {regionName}", StringComparison.Ordinal) >= 0;

    private static string FormatRegionBlock(string regionName, string indent, string inner) =>
        $"{indent}#region {regionName}{Environment.NewLine}{inner}{indent}#endregion{Environment.NewLine}";

    private static bool TryFindInsertIndexAfterClassOpenBrace(string source, out int insertIndex)
    {
        insertIndex = -1;
        var m = Regex.Match(source, @"CardBaseModel\s*\r?\n\s*\{", RegexOptions.Multiline);
        if (!m.Success)
            m = Regex.Match(source, @"CardBaseModel\s*\{");
        if (!m.Success)
            return false;
        insertIndex = AdvancePastLineBreakAfterBrace(source, m.Index + m.Length);
        return insertIndex >= 0;
    }

    private static bool TryFindInsertIndexAfterOnPlayOpenBrace(string source, out int insertIndex)
    {
        insertIndex = -1;
        var patterns = new[]
        {
            @"protected\s+override\s+async\s+Task\s+OnPlay\s*\([^)]*\)\s*\r?\n\s*\{",
            @"protected\s+override\s+async\s+Task\s+OnPlay\s*\([^)]*\)\s*\{"
        };
        foreach (var p in patterns)
        {
            var m = Regex.Match(source, p, RegexOptions.Singleline);
            if (!m.Success)
                continue;
            insertIndex = AdvancePastLineBreakAfterBrace(source, m.Index + m.Length);
            return insertIndex >= 0;
        }
        return false;
    }

    private static bool TryFindInsertIndexAfterOnUpgradeOpenBrace(string source, out int insertIndex)
    {
        insertIndex = -1;
        var patterns = new[]
        {
            @"protected\s+override\s+void\s+OnUpgrade\s*\(\s*\)\s*\r?\n\s*\{",
            @"protected\s+override\s+void\s+OnUpgrade\s*\(\s*\)\s*\{"
        };
        foreach (var p in patterns)
        {
            var m = Regex.Match(source, p, RegexOptions.Singleline);
            if (!m.Success)
                continue;
            insertIndex = AdvancePastLineBreakAfterBrace(source, m.Index + m.Length);
            return insertIndex >= 0;
        }
        return false;
    }

    /// <summary><paramref name="pos"/> 为紧接在类/方法起始 <c>{</c> 之后的下标；返回下一行起点（方法体/类体首行插入位置）。</summary>
    private static int AdvancePastLineBreakAfterBrace(string source, int pos)
    {
        if (pos > source.Length)
            return -1;
        if (pos < source.Length && source[pos] == '\r')
            pos++;
        if (pos < source.Length && source[pos] == '\n')
            pos++;
        return pos;
    }

    /// <summary>在 <paramref name="source"/> 中定位 <c>#region {regionName}</c> 与同级 <c>#endregion</c>，将两者之间替换为 <paramref name="newInner"/>（不含 region 行）。</summary>
    private static bool TryReplaceRegionContent(string source, string regionName, string indentBeforeRegion, string newInner, out string result)
    {
        result = source;
        var startTag = $"{indentBeforeRegion}#region {regionName}";
        var startIdx = source.IndexOf(startTag, StringComparison.Ordinal);
        if (startIdx < 0)
            return false;

        var line1End = source.IndexOfAny(new[] { '\r', '\n' }, startIdx);
        if (line1End < 0)
            return false;
        var contentStart = line1End;
        if (source[contentStart] == '\r' && contentStart + 1 < source.Length && source[contentStart + 1] == '\n')
            contentStart += 2;
        else if (source[contentStart] == '\n')
            contentStart += 1;

        var endTag = $"{indentBeforeRegion}#endregion";
        var endIdx = source.IndexOf(endTag, contentStart, StringComparison.Ordinal);
        if (endIdx < 0)
            return false;

        result = source[..contentStart] + newInner + source[endIdx..];
        return true;
    }

    private static string BuildCardPropertiesInner(CardDefinition def, string className)
    {
        var sb = new StringBuilder();
        const string TAB = "    ";
        sb.AppendLine($"{TAB}private const int energyCost = {def.EnergyCost};");
        sb.AppendLine($"{TAB}private const CardType type = CardType.{MapCardType(def.CardType)};");
        sb.AppendLine($"{TAB}private const CardRarity rarity = CardRarity.{MapRarity(def.Rarity)};");
        sb.AppendLine($"{TAB}private const TargetType targetType = TargetType.{MapTargetType(def.TargetType)};");
        sb.AppendLine($"{TAB}private const bool shouldShowInCardLibrary = {def.ShowInCardLibrary.ToString().ToLowerInvariant()};");
        sb.AppendLine();
        sb.AppendLine($"{TAB}/// <summary>");
        sb.AppendLine($"{TAB}/// 牌面动态变量配置。");
        sb.AppendLine($"{TAB}/// </summary>");
        sb.AppendLine($"{TAB}protected override IEnumerable<DynamicVar> CanonicalVars =>");
        sb.AppendLine($"{TAB}[");
        var vars = def.DynamicVars.Count > 0
            ? def.DynamicVars
            : new List<DynamicVarEntry> { new() { Kind = "Damage", BaseValue = 1m, UpgradeValue = 0m } };
        for (var i = 0; i < vars.Count; i++)
        {
            var comma = i < vars.Count - 1 ? "," : "";
            sb.AppendLine($"{TAB}{TAB}{EmitCanonicalVar(vars[i])}{comma}");
        }
        sb.AppendLine($"{TAB}];");
        sb.AppendLine();
        sb.AppendLine($"{TAB}public {className}() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) {{ }}");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string BuildCardPlayActionInner(CardDefinition def)
    {
        var actions = def.CardPlayActions ?? [];
        if (actions.Count == 0)
            return $"{IndentMethod}await Task.CompletedTask;{Environment.NewLine}";

        var ctx = CardPlayActionEmitContext.FromDefinition(def);
        var sb = new StringBuilder();
        for (var i = 0; i < actions.Count; i++)
        {
            var a = actions[i];
            if (!CardPlayActionCodeEmitterRegistry.TryGet(a.ActionType, out var emitter))
            {
                sb.AppendLine($"{IndentMethod}// TODO: 未知 actionType \"{EscapeCSharpString(a.ActionType)}\"");
                continue;
            }
            var code = emitter.Emit(a, ctx);
            if (i > 0 && sb.Length > 0)
                sb.AppendLine();
            sb.Append(code);
            if (!code.EndsWith('\n'))
                sb.AppendLine();
        }
        if (sb.Length == 0)
            return $"{IndentMethod}await Task.CompletedTask;{Environment.NewLine}";

        return sb.ToString();
    }

    private static string BuildOnUpgradeInner(CardDefinition def)
    {
        var vars = def.DynamicVars.Count > 0
            ? def.DynamicVars
            : new List<DynamicVarEntry> { new() { Kind = "Damage", BaseValue = 1m, UpgradeValue = 0m } };
        var sb = new StringBuilder();
        var any = false;
        foreach (var v in vars)
        {
            if (v.UpgradeValue == 0m)
                continue;
            var u = FormatDecimal(v.UpgradeValue);
            var key = EscapeCSharpString(v.Kind.Trim());
            sb.AppendLine($"{IndentMethod}base.DynamicVars[\"{key}\"].UpgradeValueBy({u}m);");
            any = true;
        }
        if (!any)
            sb.AppendLine($"{IndentMethod}// 无升级数值（upgradeValue 均为 0）");
        sb.AppendLine();
        return sb.ToString();
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

    private static string MapTargetType(string s) =>
        CardTargetTypeMapping.ParseFromDefinition(s).ToString();

    private static string EmitCanonicalVar(DynamicVarEntry v)
    {
        var kind = v.Kind.Trim();
        var kl = kind.ToLowerInvariant();
        var bv = FormatDecimal(v.BaseValue);
        return kl switch
        {
            "damage" => $"new DamageVar({bv}m, {CardPlayActionEmitSyntax.FormatValuePropForEmit(v.ValueProp, "damage")})",
            "block" => $"new BlockVar({bv}m, {CardPlayActionEmitSyntax.FormatValuePropForEmit(v.ValueProp, "block")})",
            "cards" => $"new CardsVar({(int)v.BaseValue})",
            "repeat" => $"new DynamicVar(\"Repeat\", {bv}m)",
            "power" => $"new DynamicVar(\"Power\", {bv}m)",
            _ => $"new DynamicVar(\"{EscapeCSharpString(kind)}\", {bv}m)"
        };
    }

    private static string FormatDecimal(decimal d) =>
        d.ToString("0.############", CultureInfo.InvariantCulture);

    private static string EscapeCSharpString(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
