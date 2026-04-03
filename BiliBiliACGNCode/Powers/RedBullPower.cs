//****************** 代码文件申明 ***********************
//* 文件：RedBullPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 红牛
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class RedBullPower : PowerBaseModel
{
    protected override string customIconPath => "redbull";

	private class Data
	{
		public int energySpent;

		public int triggerCount;
	}

	private const int _energyIncrement = 4;

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int DisplayAmount => 4 - GetInternalData<Data>().energySpent % 4;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(4)];

	public override bool IsInstanced => true;


	protected override object InitInternalData()
	{
		return new Data();
	}
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(amount > 0 && applier == base.Owner && power is AngerPower){
			Data data = GetInternalData<Data>();
			data.energySpent += (int)amount;
			int triggers = data.energySpent / 4 - data.triggerCount;
			if (triggers > 0)
			{
				Flash();
				await PlayerCmd.GainEnergy(base.Amount * triggers, base.Owner.Player);
				data.triggerCount += triggers;
			}
			InvokeDisplayAmountChanged();
        }
    }

}
