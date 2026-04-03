//****************** 代码文件申明 ***********************
//* 文件：NewConceptLovePower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 新概念恋爱
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewConceptLovePower : PowerBaseModel
{
    protected override string customIconPath => "new_concept_love";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
}