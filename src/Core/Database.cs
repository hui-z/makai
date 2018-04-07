using System.Collections.Generic;
using System.IO;
using HuiZ.Makai.Game;
using Newtonsoft.Json;

namespace HuiZ.Makai
{
    public class Database
    {
        public IDictionary<long, Card> Cards { get; } = new Dictionary<long, Card>();

        public Database()
        {
            InitializeCards();
        }

        private void InitializeCards()
        {
            var s = File.ReadAllText("db/cards.json");
            dynamic json = JsonConvert.DeserializeObject(s);
            var cards = json.master;
            foreach(var card in cards)
            {
                var c = new Card
                {
                    Id = card.id,
                    Name = card.name,
                    NickName = card.nickname,
                };
                Cards[c.Id] = c;
            }

        }
    }
}
