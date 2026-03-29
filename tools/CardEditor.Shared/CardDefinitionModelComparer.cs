using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>
/// 深拷贝与「仅影响脚本 / 仅影响本地化 / 仅备注」切片比较（用于保存与生成策略）。
/// </summary>
public static class CardDefinitionModelComparer
{
    public static CardDefinition DeepClone(CardDefinition m)
    {
        return new CardDefinition
        {
            SchemaVersion = m.SchemaVersion,
            ClassName = m.ClassName,
            Title = m.Title,
            Description = m.Description,
            Namespace = m.Namespace,
            EnergyCost = m.EnergyCost,
            CardType = m.CardType,
            Rarity = m.Rarity,
            TargetType = m.TargetType,
            ShowInCardLibrary = m.ShowInCardLibrary,
            PoolTypeName = m.PoolTypeName,
            DynamicVars = m.DynamicVars.Select(v => new DynamicVarEntry
            {
                Kind = v.Kind,
                BaseValue = v.BaseValue,
                UpgradeValue = v.UpgradeValue,
                ValueProp = v.ValueProp
            }).ToList(),
            CardPlayActions = m.CardPlayActions.Select(a => new CardPlayAction
            {
                ActionType = a.ActionType,
                ValueBinding = a.ValueBinding,
                Value = a.Value,
                Notes = a.Notes
            }).ToList(),
            Notes = m.Notes
        };
    }

    /// <summary>影响生成 C# 的字段（不含卡牌名称、描述、顶层备注）。</summary>
    public static bool CodeSliceEquals(CardDefinition a, CardDefinition b)
    {
        if (a.SchemaVersion != b.SchemaVersion) return false;
        if (!string.Equals(a.ClassName, b.ClassName, StringComparison.Ordinal)) return false;
        if (!string.Equals(a.Namespace, b.Namespace, StringComparison.Ordinal)) return false;
        if (a.EnergyCost != b.EnergyCost) return false;
        if (!string.Equals(a.CardType, b.CardType, StringComparison.Ordinal)) return false;
        if (!string.Equals(a.Rarity, b.Rarity, StringComparison.Ordinal)) return false;
        if (!string.Equals(a.TargetType, b.TargetType, StringComparison.Ordinal)) return false;
        if (a.ShowInCardLibrary != b.ShowInCardLibrary) return false;
        if (!string.Equals(a.PoolTypeName, b.PoolTypeName, StringComparison.Ordinal)) return false;
        if (!DynamicVarsSequenceEquals(a.DynamicVars, b.DynamicVars)) return false;
        return CardPlayActionsSequenceEquals(a.CardPlayActions, b.CardPlayActions);
    }

    /// <summary>卡牌名称 + 卡牌描述（与 cards.json 对应）。</summary>
    public static bool LocalizationSliceEquals(CardDefinition a, CardDefinition b) =>
        string.Equals(NormTitle(a.Title), NormTitle(b.Title), StringComparison.Ordinal) &&
        string.Equals(NormDesc(a.Description), NormDesc(b.Description), StringComparison.Ordinal);

    /// <summary>仅顶层备注变化：脚本切片与本地化切片均与快照一致，但备注不同。</summary>
    public static bool OnlyTopLevelNotesChanged(CardDefinition current, CardDefinition snapshot) =>
        LocalizationSliceEquals(current, snapshot) &&
        CodeSliceEquals(current, snapshot) &&
        !string.Equals(NormNotes(current.Notes), NormNotes(snapshot.Notes), StringComparison.Ordinal);

    private static bool DynamicVarsSequenceEquals(IReadOnlyList<DynamicVarEntry> a, IReadOnlyList<DynamicVarEntry> b)
    {
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
        {
            if (!string.Equals(a[i].Kind, b[i].Kind, StringComparison.Ordinal)) return false;
            if (a[i].BaseValue != b[i].BaseValue) return false;
            if (a[i].UpgradeValue != b[i].UpgradeValue) return false;
            if (a[i].ValueProp != b[i].ValueProp) return false;
        }
        return true;
    }

    private static bool CardPlayActionsSequenceEquals(IReadOnlyList<CardPlayAction> a, IReadOnlyList<CardPlayAction> b)
    {
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
        {
            if (!string.Equals(a[i].ActionType, b[i].ActionType, StringComparison.Ordinal)) return false;
            if (!string.Equals(
                    string.IsNullOrWhiteSpace(a[i].ValueBinding) ? "literal" : a[i].ValueBinding.Trim(),
                    string.IsNullOrWhiteSpace(b[i].ValueBinding) ? "literal" : b[i].ValueBinding.Trim(),
                    StringComparison.Ordinal))
                return false;
            if (a[i].Value != b[i].Value) return false;
            if (!string.Equals(a[i].Notes ?? "", b[i].Notes ?? "", StringComparison.Ordinal)) return false;
        }
        return true;
    }

    private static string NormTitle(string? s) => s?.Trim() ?? "";

    private static string NormDesc(string? s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim();

    private static string NormNotes(string? s) => string.IsNullOrWhiteSpace(s) ? "" : s.TrimEnd();
}
