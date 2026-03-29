using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>内置动态变量 kind 模版（当 exe 旁无 <c>DynamicVarTemplate.json</c> 或文件为空时使用）。</summary>
public static class DynamicVarTemplateCatalog
{
    public static List<DynamicVarTemplate> CloneDefaultTemplates() =>
        DefaultTemplates
            .Select(t => new DynamicVarTemplate
            {
                Name = t.Name,
                Description = t.Description,
                ExampleText = t.ExampleText
            })
            .ToList();

    private static readonly IReadOnlyList<DynamicVarTemplate> DefaultTemplates =
    [
        new() { Name = "Damage", Description = "伤害", ExampleText = "{Damage}" },
        new() { Name = "Block", Description = "格挡", ExampleText = "{Block}" },
        new() { Name = "MagicNumber", Description = "通用魔法数", ExampleText = "{MagicNumber}" },
        new() { Name = "Cards", Description = "抽牌张数", ExampleText = "{Cards}" },
        new() { Name = "Energy", Description = "能量", ExampleText = "{Energy}" },
        new() { Name = "Hits", Description = "打击次数", ExampleText = "{Hits}" },
        new() { Name = "Times", Description = "重复次数", ExampleText = "{Times}" },
        new() { Name = "Amount", Description = "通用数量", ExampleText = "{Amount}" },
        new() { Name = "Power", Description = "能力层数/数值", ExampleText = "{Power}" },
        new() { Name = "Strength", Description = "力量", ExampleText = "{Strength}" },
        new() { Name = "Dexterity", Description = "敏捷", ExampleText = "{Dexterity}" },
        new() { Name = "Focus", Description = "集中", ExampleText = "{Focus}" },
        new() { Name = "Poison", Description = "中毒层数", ExampleText = "{Poison}" },
        new() { Name = "Vulnerable", Description = "易伤", ExampleText = "{Vulnerable}" },
        new() { Name = "Weak", Description = "虚弱", ExampleText = "{Weak}" },
        new() { Name = "Heal", Description = "治疗量", ExampleText = "{Heal}" },
        new() { Name = "Discard", Description = "弃牌张数", ExampleText = "{Discard}" },
        new() { Name = "Exhaust", Description = "消耗张数", ExampleText = "{Exhaust}" },
        new() { Name = "Draw", Description = "抽牌（别名）", ExampleText = "{Draw}" },
        new() { Name = "Gold", Description = "金币", ExampleText = "{Gold}" },
        new() { Name = "energyPrefix", Description = "费用前缀文案（若项目使用）", ExampleText = "{energyPrefix}" }
    ];
}
