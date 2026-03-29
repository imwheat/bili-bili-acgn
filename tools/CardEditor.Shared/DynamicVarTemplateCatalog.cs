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
        new() { Name = "Damage", Description = "伤害", ExampleText = "造成{Damage:diff()}点伤害。" },
        new() { Name = "Block", Description = "格挡", ExampleText = "获得{Block:diff()}点格挡。" },
        new() { Name = "Cards", Description = "卡牌数量", ExampleText = "抽{Cards:diff()}张牌。" },
        new() { Name = "Energy", Description = "能量（动态值）", ExampleText = "获得{Energy:energyIcons()}。" },
        new() { Name = "energyPrefix", Description = "能量（固定数值）", ExampleText = "获得{energyPrefix:energyIcons(1)}。" },
        new() { Name = "Repeat", Description = "重复次数", ExampleText = "造成{Damage:diff()}点伤害{Repeat:diff()}次。" },
        new() { Name = "Heal", Description = "治疗", ExampleText = "回复{Heal:diff()}点生命。" },
        new() { Name = "HpLoss", Description = "失去生命", ExampleText = "失去{HpLoss:Diff()}点生命。" },
        new() { Name = "MaxHp", Description = "最大生命", ExampleText = "获得{MaxHp:diff()}点最大生命。" },
        new() { Name = "Gold", Description = "金币", ExampleText = "获得{Gold:diff()}金币。" },
        new() { Name = "Summon", Description = "召唤", ExampleText = "召唤{Summon:diff()}。" },
        new() { Name = "Forge", Description = "铸造", ExampleText = "铸造{Forge:diff()}。" },
        new() { Name = "Stars", Description = "辉星", ExampleText = "获得{Stars:starIcons()}。" },
        new() { Name = "StrengthPower", Description = "力量", ExampleText = "获得{StrengthPower:diff()}点力量。" },
        new() { Name = "DexterityPower", Description = "敏捷", ExampleText = "获得{DexterityPower:diff()}点敏捷。" },
        new() { Name = "WeakPower", Description = "虚弱", ExampleText = "给予{WeakPower:diff()}层虚弱。" },
        new() { Name = "VulnerablePower", Description = "易伤", ExampleText = "给予{VulnerablePower:diff()}层易伤。" },
        new() { Name = "PoisonPower", Description = "中毒", ExampleText = "给予{PoisonPower:diff()}层中毒。" },
        new() { Name = "DoomPower", Description = "灾厄", ExampleText = "给予{DoomPower:diff()}层灾厄。" },
        new() { Name = "CalculatedDamage", Description = "计算出的伤害量", ExampleText = "（造成{CalculatedDamage:diff()}点伤害）" },
        new() { Name = "CalculatedBlock", Description = "计算出的格挡值", ExampleText = "（获得{CalculatedBlock:diff()}点格挡）" }
    ];
}
