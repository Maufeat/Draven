﻿using Draven.ServerModels;
using Draven.Structures;
using Messages;
using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;
using System;

namespace Draven.Messages.LeaguesServiceProxy
{
    class GetAllMyLeagues : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            e.ReturnRequired = true;
            e.Data = new SummonerLeaguesDTO()
            {
                SummonerLeagues = new ArrayCollection()
                {
                    new LeagueListDTO()
                    {
                        Queue = "RANKED_SOLO_5x5",
                        Name = "Sahin The Master",
                        Tier = "CHALLENGER",
                        RequestorsRank = "null",
                        Entries = new ArrayCollection()
                        {
                            new LeagueItemDTO()
                            {
                                PreviousDayLeaguePosition = 1,
                                SeasonEndTier = "CHALLENGER",
                                SeasonEndRank = "I",
                                HotStreak = true,
                                LeagueName = "Sahin The Master",
                                MiniSeries = null,
                                Tier = "CHALLENGER",
                                FreshBlood = true,
                                LastPlayed = 0,
                                TimeUntilInactivityStatusChanges = 0,
                                InactivityStatus = "OK",
                                PlayerOrTeamId = "1",
                                LeaguePoints = 9999,
                                DemotionWarning = 0,
                                Inactive = false,
                                SeasonEndApexPosition = 1,
                                Rank = "I",
                                Veteran = true,
                                QueueType = "RANKED_SOLO_5x5",
                                Losses = 0,
                                TimeUntilDecay = -1,
                                DisplayDecayWarning = false,
                                PlayerOrteamName = "Maufeat",
                                Wins = 999
                            }
                        },
                        NextApexUpdate = 7430971,
                        MaxLeagueSize = 200,
                        RequestorsName = null
                    }
                }
            };
            return e;
        }
    }
}
