using Draven.ServerModels;
using Draven.Structures;
using Messages;
using RtmpSharp.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draven.Messages.SummonerService
{
    class GetAllSummonerDataByAccount : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            object[] body = e.Body as object[];
            SummonerClient summonerSender = sender as SummonerClient;
            int creds = Convert.ToInt32(body[0]);
            Console.WriteLine("Tries to get ALlSummonerDataByAccount from ID: " + creds);

            AllSummonerData allSD = new AllSummonerData()
            {
                SpellBook = new SpellBookDTO()
                {
                    BookPagesJson = null,
                    BookPages = null,
                    DateString = "Tue Dec 02 03:23:04 UTC 2014",
                    SummonerId = summonerSender._sumId
                },
                SummonerDefaultSpells = new SummonerDefaultSpells()
                {
                    SummonerDefaultSpellsJson = null,
                    SummonerDefaultSpellMap = null,
                    SummonerId = summonerSender._sumId
                },
                SummonerTalentsAndPoints = new SummonerTalentsAndPoints()
                {
                    TalentPoints = 30,
                    ModifyDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    CreateDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    SummonerId = summonerSender._sumId
                },
                Summoner = new Summoner()
                {
                    InternalName = summonerSender._session.Summary.InternalName,
                    PreviousSeasonHighestTier = "CHALLENGER",
                    AcctId = summonerSender._session.Summary.AccountId,
                    HelpFlag = false,
                    SumId = summonerSender._sumId,
                    ProfileIconId = Convert.ToInt32(summonerSender._sumIcon),
                    DisplayEloQuestionaire = false,
                    LastGameDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    PreviousSeasonHighestTeamReward = 0,
                    RevisionDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    AdvancedTutorialFlag = false,
                    RevisionId = 0,
                    Name = summonerSender._session.Summary.SummonerName,
                    NameChangeFlag = false,
                    TutorialFlag = false
                },
                MasteryBook = new MasteryBookDTO()
                {
                    BookPagesJson = null,
                    BookPages = null,
                    DateString = "Tue Dec 02 03:23:04 UTC 2014",
                    SummonerId = summonerSender._sumId
                },
                SummonerLevelAndPoints = new SummonerLevelAndPoints()
                {
                    InfPoints = 69974,
                    ExpPoints = 44,
                    SummonerLevel = summonerSender._sumLvl,
                    SummonerId = summonerSender._sumId
                },
                SummonerLevel = new SummonerLevel()
                {
                    ExpTierMod = 1,
                    GrantRp = 0,
                    ExpForLoss = 0,
                    SummonerTier = 1,
                    InfTierMod = 1,
                    ExpToNextLevel = 1250,
                    ExpForWin = 0,
                    Level = summonerSender._sumLvl
                }
            };

            e.ReturnRequired = true;
            e.Data = allSD;
            return e;
        }
    }
}
