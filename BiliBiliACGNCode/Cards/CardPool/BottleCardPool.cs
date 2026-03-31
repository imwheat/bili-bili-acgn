//****************** 代码文件申明 ***********************
//* 文件：BottleCardPool
//* 作者：wheat
//* 创建时间：2026/03/30 10:51:00 星期一
//* 描述：瓶子君152（牛子豪）卡池
//*******************************************************
using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;

public sealed class BottleCardPool : CustomCardPoolModel
{
    public override string Title => "Bottle152";

    public override string EnergyColorName => CustomEnergyIconPatches.GetEnergyColorName(Id);
    public override string? TextEnergyIconPath{
        get
        {
            var path = $"bottle152_energy_icon.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
    public override string? BigEnergyIconPath {
        get
        {
            var path = $"bottle152_energy_big.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
    public override Color ShaderColor => new Color("789ccd");
    public override Color DeckEntryCardColor => new Color("789ccd");
    public override Color EnergyOutlineColor => new Color("184788");

    public override bool IsColorless => false;
    public override bool IsShared => true;

    protected override CardModel[] GenerateAllCards()
    {
        // 所有[Pool(typeof(BottleCardPool))]的卡牌
        return [
            ModelDb.Card<AngryStrike>(),
            ModelDb.Card<BottleStrike>(),
            ModelDb.Card<BottleBlock>(),
            ModelDb.Card<BullStab>(),
            ModelDb.Card<BusinessExpansion>(),
            ModelDb.Card<CornSyndrome>(),
            ModelDb.Card<Creep>(),
            ModelDb.Card<Day0>(),
            ModelDb.Card<DeepThink>(),
            ModelDb.Card<HairGrowth>(),
            ModelDb.Card<IAmTheMaggot>(),
            ModelDb.Card<InfiniteBullness>(),
            ModelDb.Card<MacroDomain>(),
            ModelDb.Card<NeverDie>(),
            ModelDb.Card<OffFieldPlay>(),
            ModelDb.Card<ReadingRadio>(),
            ModelDb.Card<UsGreenCard>(),
            ModelDb.Card<WitPeak>(),
            ModelDb.Card<WildBuffaloStrike>(),
            ModelDb.Card<ZhuangTang>(),
        ];
    }
}
