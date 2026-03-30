//****************** 代码文件申明 ***********************
//* 文件：CustomKeyWords
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:00 星期四
//* 描述：自定义词条
//*******************************************************
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

public static class CustomKeyWords
{
    [CustomEnum("YYSY")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static readonly CardKeyword YYSY;
}