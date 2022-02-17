using System;
using System.Collections.Generic;

namespace TalkiPlay.Shared
{
    public interface IChildItemReward
    {
        int ItemId { get; }
        string Status { get; }
        int FailureCount { get; }

        bool IsSuccess();
    }

    public interface IChildItemRewards
    {
        int DurationInSeconds { get; }
        IList<ChildItemRewardDto> Items { get; }
    }


    public interface IChildItemProgress
    {
        int Id { get; }
        double Star { get; }
    }

    public interface IChildPackProgress
    {
        int Id { get; }
        string Name { get; }
        double TotalMinutesPlayed { get; }
        double Progress { get; }
    }
}
