using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer
{
    public static class Utilities
    {
        public static string GetRandomPhrase(IList<Phrase> phraseCollection)
        {
            if (phraseCollection.All(x => x.Used))
            {
                for (int i = 0; i < phraseCollection.Count; i++)
                {
                    phraseCollection[i].Used = false;
                }
            }

            Random rnd = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray()));
            Phrase[] unusedPhrases = phraseCollection.Where(x => !x.Used).ToArray();
            Phrase selection = unusedPhrases[rnd.Next(0, unusedPhrases.Length - 1)];

            phraseCollection.First(x => x == selection).Used = true;

            return selection.Text;
        }
    }
}
