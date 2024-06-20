using System.Net.Quic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;

namespace CardPile.Draft
{
    public class DraftState
    {
        public static DraftState GenerateFromSequence(DraftEventSequence sequence)
        {
            DraftState result = new();
            foreach(var e in sequence)
            {
                result.ProcessEvent(e);
            }
            return result;
        }

        public void ProcessEvent(DraftEvent e)
        {
            if (e is DraftEnterEvent startEvent)
            {
            }
            if (e is DraftChoiceEvent choiceEvent)
            {
                lastPack = choiceEvent.PackNumber;
                lastPick = choiceEvent.PickNumber;
                CurrentPack = choiceEvent.CardsInPack;
                AddSeenPack(choiceEvent.PackNumber, choiceEvent.PickNumber, choiceEvent.CardsInPack);
            }
            else if (e is DraftPickEvent pickEvent)
            {
                if (pickEvent.PackNumber == lastPack && pickEvent.PickNumber == lastPick)
                {
                    CurrentPack.Clear();
                }
                AddPick(pickEvent.PackNumber, pickEvent.PickNumber, pickEvent.CardPicked);
                AddSeenPack(pickEvent.PackNumber, pickEvent.PickNumber, pickEvent.CardsInPack);
            }
        }

        public List<int> CurrentPack
        {
            get
            {
                return currentPack;
            }
            private set
            {
                currentPack = new List<int>(value);
            }
        }

        public List<List<int>> Picks 
        {
            get
            {
                return picks;
            }
        }

        public int LastPack
        {
            get
            {
                return lastPack;
            }
        }

        public int LastPick
        {
            get
            {
                return lastPick;
            }
        }

        public List<int> GetCurrentDeck()
        {
            return GetDeck(lastPack, lastPick);
        }

        public List<int> GetDeck(int pack, int pick)
        {
            var packIndex = pack - 1;
            var pickIndex = pick - 1;

            if (picks.Count <= packIndex)
            {
                packIndex = picks.Count - 1;
                if (packIndex >= 0)
                {
                    pickIndex = picks[packIndex].Count - 1;
                }
            }
            if (packIndex < 0)
            {
                return [];
            }
            if (picks[packIndex].Count <= pickIndex)
            {
                pickIndex = picks[packIndex].Count - 1;
            }

            var result = new List<int>();
            for(int currentPackIndex = 0; currentPackIndex < packIndex; currentPackIndex++)
            {
                result.AddRange(picks[currentPackIndex]);
            }
            for(int currentPickIndex = 0; currentPickIndex <= pickIndex; currentPickIndex++)
            {
                result.Add(picks[packIndex][currentPickIndex]);
            }
            return result;
        }

        public List<int> GetCurrentMissingCards()
        {
            return GetMissingCards(lastPack, lastPick);
        }

        public List<int> GetMissingCards(int pack, int pick)
        {
            if(CurrentPack.Count == 0)
            {
                // No cards missing if there is no current pack
                return []; 
            }

            var packIndex = pack - 1;
            var pickIndex = pick - 1;

            if (packsSeen.Count <= packIndex || packsSeen[packIndex].Count <= pickIndex )
            {
                return [];
            }

            var previousPickIndex = pickIndex - PACK_LOOK_BACK;
            if(previousPickIndex < 0 )
            {
                return [];
            }

            var result = new List<int>(packsSeen[packIndex][previousPickIndex]);
            result.Remove(picks[packIndex][previousPickIndex]);
            foreach(var card in packsSeen[packIndex][pickIndex])
            {
                result.Remove(card);
            }
            return result;
        }

        public int? GetCurrentPackPreviousPick()
        {
            return GetPreviousPick(lastPack, lastPick);
        }

        public int? GetPreviousPick(int pack, int pick)
        {
            if (CurrentPack.Count == 0)
            {
                // No cards previuous pick if there is no current pack
                return null;
            }

            var packIndex = pack - 1;
            var pickIndex = pick - 1;

            if (packsSeen.Count <= packIndex || packsSeen[packIndex].Count <= pickIndex)
            {
                return null;
            }

            var previousPickIndex = pickIndex - PACK_LOOK_BACK;
            if (previousPickIndex < 0)
            {
                return null;
            }

            return picks[packIndex][previousPickIndex];
        }

        public List<int> GetCurrentUpcomingCards()
        {
            return GetUpcomingCards(lastPack, lastPick);
        }

        public List<int> GetUpcomingCards(int pack, int pick)
        {
            var packIndex = pack - 1;
            var pickIndex = pick - 1;

            if (packsSeen.Count <= packIndex || packsSeen[packIndex].Count <= pickIndex)
            {
                return [];
            }

            var upcomingPickIndex = pickIndex - UPCOMING_PACK_LOOK_BACK;
            if (upcomingPickIndex < 0)
            {
                return [];
            }

            if(packsSeen[packIndex][upcomingPickIndex].Count <= 8)
            {
                return [];
            }

            var result = new List<int>(packsSeen[packIndex][upcomingPickIndex]);
            result.Remove(picks[packIndex][upcomingPickIndex]);
            return result;
        }

        private void AddPick(int packNumber, int pickNumber, int cardPicked)
        {
            var packIndex = packNumber - 1;
            var pickIndex = pickNumber - 1;

            while (picks.Count <= packIndex)
            {
                picks.Add([]);
            }
            while (picks[packIndex].Count <= pickIndex)
            {
                picks[packIndex].Add(-1);
            }

            picks[packIndex][pickIndex] = cardPicked;
        }

        private void AddSeenPack(int packNumber, int pickNumber, List<int> pack)
        {
            var packIndex = packNumber - 1;
            var pickIndex = pickNumber - 1;
            while (packsSeen.Count <= packIndex)
            {
                packsSeen.Add([]);
            }
            while (packsSeen[packIndex].Count <= pickIndex)
            {
                packsSeen[packIndex].Add([]);
            }

            packsSeen[packIndex][pickIndex] = new List<int>(pack);
        }

        private const int PACK_LOOK_BACK = 8;
        private const int UPCOMING_PACK_LOOK_BACK = 7;

        private int lastPack = 1;

        private int lastPick = 1;

        private List<int> currentPack = [];

        private List<List<int>> picks = [];

        private List<List<List<int>>> packsSeen = [];
    }
}
