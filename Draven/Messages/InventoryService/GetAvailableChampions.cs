using Draven.Structures;
using Messages;
using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Draven.Messages.InventoryService
{
    class GetAvailableChampions : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            ArrayCollection champions = new ArrayCollection();

            /*foreach (Champions champ in PoroServer._data.Champions)
            {
                var champDTO = new ChampionDTO
                {
                    Owned = true,
                    ChampionID = champ.id,
                    Active = true,
                    BotEnabled = true,
                    RankedPlayEnabled = true
                };

                champDTO.ChampionSkins = new ArrayCollection();

                IEnumerable<ChampionSkinDTO> champSkinData = PoroServer._data.ChampionSkins.Where(x => x.championId == champ.id).Select(skins => new ChampionSkinDTO
                {
                    ChampionID = champ.id,
                    SkinID = skins.id,
                    StillObtainable = true,
                    Owned = true
                });

                foreach (ChampionSkinDTO champion in champSkinData)
                {
                    champDTO.ChampionSkins.Add(champion);
                }

                champions.Add(champDTO);
            }*/

            e.ReturnRequired = true;
            e.Data = null;

            return e;
        }
    }
}
