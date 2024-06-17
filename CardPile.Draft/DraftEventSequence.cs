using System.Collections;

namespace CardPile.Draft;

public class DraftEventSequence : IEnumerable<DraftEvent>, IEnumerable
{   
    public DraftEventSequence()
    { }

    public int Length => events.Count;

    public IEnumerator<DraftEvent> GetEnumerator()
    {
        return events.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return events.GetEnumerator();
    }

    public void Add(DraftEvent draftEvent)
    {
        events.Add(draftEvent);
    }

    public DraftEventSequence SubSequence(int last)
    {
        return SubSequence(0, last);
    }

    public DraftEventSequence SubSequence(int first, int last)
    {
        if (first < 0)
        {
            first = 0;
        }
        if (events.Count < first)
        {
            first = events.Count;
        }
        if (last < 0)
        {
            last = 0;
        }
        if (events.Count < last)
        {
            last = events.Count;
        }

        DraftEventSequence result = new DraftEventSequence();
        for (int i = first; i < last; i++)
        {
            result.Add(events[i]);
        }
        return result;
    }

    private List<DraftEvent> events = new List<DraftEvent>();
}
