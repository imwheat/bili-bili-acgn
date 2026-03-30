using System.Text;
using CardEditor.Shared.Models;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 伤害打出效果。可通过 <see cref="CardPlayActionEmitContext.TargetType"/>（<see cref="MegaCrit.Sts2.Core.Entities.Cards.TargetType"/>）在 <see cref="GenerateCode"/> 内分支；
/// 重复次数请在本方法内按需编写或调用 <see cref="CardPlayActionEmitSyntax.WrapWithRepeatLoop"/>，不在此强制套一层。
/// </summary>
public sealed class DamageCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Damage";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var targetType = context.TargetType;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var repeatCount = CardPlayActionEmitSyntax.RepeatCountExpression(action);
        return targetType switch
        {
            TargetType.AllEnemies => BuildAllEnemyDamage(indent, v, repeatCount),
            TargetType.RandomEnemy => BuildRandomEnemyDamage(indent, v, repeatCount),
            // 默认：需单目标的攻击（与 TestCard 等一致）；其它 TargetType 请在下方添加 case 自行编排。
            _ => BuildDefaultSingleTargetDamage(indent, v, repeatCount)
        };
    }




    /// <summary>依赖 <c>cardPlay.Target</c> 的单次伤害；重复次数请在调用处外包或于 <see cref="GenerateCode"/> 内自定义循环。</summary>
    private static string BuildDefaultSingleTargetDamage(string indent, string valueExpr, string repeatCount)
    {
        var inner = new StringBuilder();
        inner.AppendLine($"{indent}if (cardPlay.Target != null)");
        inner.AppendLine($"{indent}{{");
        inner.AppendLine($"{indent}    await DamageCmd.Attack({valueExpr})");
        if(repeatCount != "1"){
            inner.AppendLine($"{indent}    .WithHitCount({repeatCount})");
        }
        inner.AppendLine($"{indent}        .FromCard(this)");
        inner.AppendLine($"{indent}        .Targeting(cardPlay.Target)");
        inner.AppendLine($"{indent}        .Execute(choiceContext);");
        inner.Append($"{indent}}}");
        return inner.ToString();
    }
    /// <summary>
    /// 攻击全体敌人代码生成
    /// </summary>
    /// <param name="indent"></param>
    /// <param name="valueExpr"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    private static string BuildAllEnemyDamage(string indent, string valueExpr, string repeatCount)
    {
        var inner = new StringBuilder();
        inner.AppendLine($"{indent}if (cardPlay.Target != null)");
        inner.AppendLine($"{indent}{{");
        inner.AppendLine($"{indent}    await DamageCmd.Attack({valueExpr})");
        if(repeatCount != "1"){
            inner.AppendLine($"{indent}    .WithHitCount({repeatCount})");
        }
        inner.AppendLine($"{indent}        .FromCard(this)");
        inner.AppendLine($"{indent}        .TargetingAllOpponents(base.CombatState)");
        inner.AppendLine($"{indent}        .Execute(choiceContext);");
        inner.Append($"{indent}}}");
        return inner.ToString();
    }
    /// <summary>
    /// 攻击随机敌人代码生成
    /// </summary>
    /// <param name="indent"></param>
    /// <param name="valueExpr"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    private static string BuildRandomEnemyDamage(string indent, string valueExpr, string repeatCount)
    {
        var inner = new StringBuilder();
        inner.AppendLine($"{indent}if (cardPlay.Target != null)");
        inner.AppendLine($"{indent}{{");
        inner.AppendLine($"{indent}    await DamageCmd.Attack({valueExpr})");
        if(repeatCount != "1"){
            inner.AppendLine($"{indent}    .WithHitCount({repeatCount})");
        }
        inner.AppendLine($"{indent}        .FromCard(this)");
        inner.AppendLine($"{indent}        .TargetingRandomOpponents(base.CombatState, true)");
        inner.AppendLine($"{indent}        .Execute(choiceContext);");
        inner.Append($"{indent}}}");
        return inner.ToString();
    }
}
