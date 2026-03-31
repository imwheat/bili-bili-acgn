using System.Linq;
using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

public sealed class BlockCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Block";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var blockEntry = context.CanonicalVars?.FirstOrDefault(x =>
            x.Kind.Trim().Equals("Block", StringComparison.OrdinalIgnoreCase));
        var vp = CardPlayActionEmitSyntax.FormatValuePropForEmit(blockEntry?.ValueProp ?? ValueProp.None, "block");
        var inner = $"{indent}await CreatureCmd.GainBlock(base.Owner.Creature, {v}, {vp}, null);";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}
