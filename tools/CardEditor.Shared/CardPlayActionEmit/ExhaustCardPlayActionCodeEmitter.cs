using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

public sealed class ExhaustCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Exhaust";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var inner =
            $"{indent}var __exhaustCount = (int){v};\n" +
            $"{indent}if (__exhaustCount > 0)\n" +
            $"{indent}{{\n" +
            $"{indent}    var __cardsToExhaust = new global::System.Collections.Generic.List<CardModel>();\n" +
            $"{indent}    foreach (var __card in base.Owner.Hand.Cards)\n" +
            $"{indent}    {{\n" +
            $"{indent}        if (__cardsToExhaust.Count >= __exhaustCount)\n" +
            $"{indent}            break;\n" +
            $"{indent}        __cardsToExhaust.Add(__card);\n" +
            $"{indent}    }}\n" +
            $"{indent}    foreach (var __card in __cardsToExhaust)\n" +
            $"{indent}        await CardCmd.Exhaust(choiceContext, __card, false, false);\n" +
            $"{indent}}}";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}
