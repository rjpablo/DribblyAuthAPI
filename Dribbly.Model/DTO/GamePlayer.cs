﻿using Dribbly.Model.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dribbly.Service.DTO
{
    public class GamePlayer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int JerseyNo { get; set; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Fouls { get; set; }
        public int Assists { get; set; }
        public long TeamId { get; set; }

        public GamePlayer(TeamMembershipModel player)
        {
            Id = player.Member.IdentityUserId;
            Name = player.Member.Name;
            JerseyNo = player.JerseyNo;
            TeamId = player.TeamId;
        }
    }
}