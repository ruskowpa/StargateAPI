using StargateAPI.Business.Enums;

namespace StargateAPI.Business.Helpers
{
    public static class EnumHelper
    {
        public static class DutyTitles
        {
            public const string Commander = "Commander";
            public const string Captain = "Captain";
            public const string Major = "Major";
            public const string Lieutenant = "Lieutenant";
            public const string Retired = "RETIRED";
        }

        public static class Ranks
        {
            public const string SecondLieutenant = "2LT";
            public const string FirstLieutenant = "1LT";
            public const string Captain = "CPT";
            public const string Major = "MAJ";
            public const string LieutenantColonel = "LTC";
            public const string Colonel = "COL";
        }

        public static DutyTitleEnum GetDutyTitleEnum(string dutyTitle)
        {
            return dutyTitle switch
            {
                DutyTitles.Commander => DutyTitleEnum.Commander,
                DutyTitles.Captain => DutyTitleEnum.Captain,
                DutyTitles.Major => DutyTitleEnum.Major,
                DutyTitles.Lieutenant => DutyTitleEnum.Lieutenant,
                DutyTitles.Retired => DutyTitleEnum.Retired,
                _ => throw new ArgumentException($"Unknown duty title: {dutyTitle}")
            };
        }

        public static RankEnum GetRankEnum(string rank)
        {
            return rank switch
            {
                Ranks.SecondLieutenant => RankEnum.SecondLieutenant,
                Ranks.FirstLieutenant => RankEnum.FirstLieutenant,
                Ranks.Captain => RankEnum.Captain,
                Ranks.Major => RankEnum.Major,
                Ranks.LieutenantColonel => RankEnum.LieutenantColonel,
                Ranks.Colonel => RankEnum.Colonel,
                _ => throw new ArgumentException($"Unknown rank: {rank}")
            };
        }

        public static bool IsRetired(string dutyTitle)
        {
            return dutyTitle == DutyTitles.Retired;
        }
    }
}
