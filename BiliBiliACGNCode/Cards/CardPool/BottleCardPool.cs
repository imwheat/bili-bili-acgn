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
            ModelDb.Card<ChairTuber>(),
            ModelDb.Card<MacroDomain>(),
            ModelDb.Card<NeverDie>(),
            ModelDb.Card<OffFieldPlay>(),
            ModelDb.Card<ReadingRadio>(),
            ModelDb.Card<UsGreenCard>(),
            ModelDb.Card<WitPeak>(),
            ModelDb.Card<WildBuffaloStrike>(),
            ModelDb.Card<ZhuangTang>(),
            ModelDb.Card<CoinOperation>(),
            ModelDb.Card<EmergencyOxygen>(),
            ModelDb.Card<OnlyTailwindGames>(),
            ModelDb.Card<DidntPad>(),
            ModelDb.Card<OxCall>(),
            ModelDb.Card<AlreadyReported>(),
            ModelDb.Card<StreamEndManhwa>(),
            ModelDb.Card<HeadFlashGlare>(),
            ModelDb.Card<ElbowStrike>(),
            ModelDb.Card<OutsiderMe>(),
            ModelDb.Card<NewYearGalaDeathSong>(),
            ModelDb.Card<SacrificeCyberParents>(),
            ModelDb.Card<ReallyUnsubscribed>(),
            ModelDb.Card<JusticeBao>(),
            ModelDb.Card<MolotovCocktail>(),
            ModelDb.Card<ImOut>(),
            ModelDb.Card<MyConfession>(),
            ModelDb.Card<InterpretYourDream>(),
            ModelDb.Card<DontSayChallenge>(),
            ModelDb.Card<BullTexasGuillotine>(),
            ModelDb.Card<BullDemonForm>(),
            ModelDb.Card<CorrectTeam>(),
            ModelDb.Card<SecondRateCommentator>(),
            ModelDb.Card<AnimeSword>(),
            ModelDb.Card<BlueTamago>(),
            ModelDb.Card<STQGather>(),
            ModelDb.Card<SacredSlash>(),
            ModelDb.Card<BiggestWarCriminal>(),
            ModelDb.Card<SmNiuGe>(),
            ModelDb.Card<NoRightToKnightMe>(),
            ModelDb.Card<ShowbizEffect>(),
            ModelDb.Card<PoisonMilkMedusa>(),
            ModelDb.Card<NewConceptLove>(),
            ModelDb.Card<MouSouZei>(),
            ModelDb.Card<KaiYun>(),
            ModelDb.Card<Roar>(),
            ModelDb.Card<OnlyTwoYearsOlder>(),
            ModelDb.Card<SherrysWhisper>(),
            ModelDb.Card<HonestPickUp>(),
            ModelDb.Card<WhereSecondFloor>(),
            ModelDb.Card<NotAfraidOfMyLies>(),
            ModelDb.Card<InstantPoisonMilk>(),
            ModelDb.Card<HappyWaterBuffalo>(),
            ModelDb.Card<BurnZero>(),
            ModelDb.Card<WhiteDoveWing>(),
            ModelDb.Card<YouCaughtItToo>(),
            ModelDb.Card<BullGeneral>(),
            ModelDb.Card<HailOfBlades>(),
            ModelDb.Card<RefutationalPersonality>(),
            ModelDb.Card<StartEnjoying>(),
            ModelDb.Card<BullAngel>(),
            ModelDb.Card<RedBullCard>(),
            ModelDb.Card<MikeZiHao>(),
            ModelDb.Card<MaiMaiProtect>(),
            ModelDb.Card<SoyBraisedF1ckle>(),
            ModelDb.Card<AhaXiuMaker>(),
            ModelDb.Card<TwoHundredLoveComics>(),
            ModelDb.Card<CloseDoorReleaseSa>(),
            ModelDb.Card<NeedAKill>(),
            ModelDb.Card<BlowWithThemAll>(),
            ModelDb.Card<USForcesJapan>(),
            ModelDb.Card<AmericanQuickDraw>(),
            ModelDb.Card<ChampagnePop>(),
            ModelDb.Card<CalmDownCalmDown>(),
            ModelDb.Card<Meditation>(),
            ModelDb.Card<InfiniteBullness>(),
        ];
    }
}
