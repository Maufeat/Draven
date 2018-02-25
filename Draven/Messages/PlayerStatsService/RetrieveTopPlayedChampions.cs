using Draven.ServerModels;
using Draven.Structures;
using Messages;
using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;
using System;

namespace Draven.Messages.PlayerStatsService
{
    class RetrieveTopPlayedChampions : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            object[] body = e.Body as object[];
            SummonerClient summonerSender = sender as SummonerClient;
            int accId = Convert.ToInt32(body[0]);
            string unknown = Convert.ToString(body[1]);

            ArrayCollection rData = new ArrayCollection()
            {
                new ChampionStatInfo()
                {
                    TotalGammesPlayed = 9999,
                    AccountId = Convert.ToInt32(summonerSender._session.Summary.AccountId),
                    Stats = new ArrayCollection()
                    {

                    },
                    ChampionId = 0
                },
                new ChampionStatInfo()
                {
                    TotalGammesPlayed = 9999,
                    AccountId = Convert.ToInt32(summonerSender._session.Summary.AccountId),
                    Stats = new ArrayCollection()
                    {

                    },
                    ChampionId = 25
                },
                new ChampionStatInfo()
                {
                    TotalGammesPlayed = 9999,
                    AccountId = Convert.ToInt32(summonerSender._session.Summary.AccountId),
                    Stats = new ArrayCollection()
                    {

                    },
                    ChampionId = 267
                },
                new ChampionStatInfo()
                {
                    TotalGammesPlayed = 9999,
                    AccountId = Convert.ToInt32(summonerSender._session.Summary.AccountId),
                    Stats = new ArrayCollection()
                    {

                    },
                    ChampionId = 12
                }
            };

            e.ReturnRequired = true;
            e.Data = rData;

            return e;
        }
    }
}
