﻿using Dribbly.Model.Entities;
using Dribbly.Model.Fouls;
using Dribbly.Model.Games;

namespace Dribbly.Model.Play
{
    public class UpsertShotResultModel
    {
        public UpsertFoulResultModel FoulResult { get; set; }
        public GamePlayerModel TakenBy { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public GameTeamModel Team1 { get; set; }
        public GameTeamModel Team2 { get; set; }
        public BlockResultModel BlockResult { get; set; }
        public AssistResultModel AssistResult { get; set; }
        public ReboundResultModel ReboundResult { get; set; }
    }
}
