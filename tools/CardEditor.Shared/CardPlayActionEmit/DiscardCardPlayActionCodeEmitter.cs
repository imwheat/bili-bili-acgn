using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

public sealed class DiscardCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Discard";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var inner =
            $"{indent}var __discardCount = (int){v};\n" +
            $"{indent}if (__discardCount > 0)\n" +
            $"{indent}{{\n" +
            $"{indent}    var __cardsToDiscard = new global::System.Collections.Generic.List<CardModel>();\n" +
            $"{indent}    foreach (var __card in base.Owner.Hand.Cards)\n" +
            $"{indent}    {{\n" +
            $"{indent}        if (__cardsToDiscard.Count >= __discardCount)\n" +
            $"{indent}            break;\n" +
            $"{indent}        __cardsToDiscard.Add(__card);\n" +
            $"{indent}    }}\n" +
            $"{indent}    if (__cardsToDiscard.Count > 0)\n" +
            $"{indent}        await CardCmd.DiscardAndDraw(choiceContext, __cardsToDiscard, 0);\n" +
            $"{indent}}}";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}
