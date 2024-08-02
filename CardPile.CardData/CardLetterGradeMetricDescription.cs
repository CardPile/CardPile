using CardPile.CardData.Formatting;

namespace CardPile.CardData;

public class CardLetterGradeMetricDescription : ICardMetricDescription
{
    public CardLetterGradeMetricDescription(string name, bool isDefaultVisible, bool isDefault)
    {
        Name = name;
        IsDefaultVisible = isDefaultVisible;
        IsDefaultMetric = isDefault;
    }

    public string Name { get; init; }

    public bool IsDefaultVisible { get; init; }

    public bool IsDefaultMetric { get; init; }

    public IComparer<ICardMetric> Comparer { get => new CardLetterGradeMetricComparer(); }

    public ICardMetric NewMetric<E>(E? value)
    {
        if (value == null)
        {
            return new CardLetterGradeMetric(this, null!);
        }

        if (value is not string baseValue)
        {
            throw new ArgumentException("The letter grade metric value type must be a string");
        }
        return new CardLetterGradeMetric(this, baseValue);
    }

    private class CardLetterGradeMetricComparer : IComparer<ICardMetric>
    {
        private static readonly Dictionary<string, int> GRADE_TO_VALUE = new()
        {
            {"A+", 12 },
            {"A",  11 },
            {"A-", 10 },
            {"B+",  9 },
            {"B",   8 },
            {"B-",  7 },
            {"C+",  6 },
            {"C",   5 },
            {"C-",  4 },
            {"D+",  3 },
            {"D",   2 },
            {"D-",  1 },
            {"F",   0 },
        };

        private static readonly string[] PREFIXES_TO_IGNORE =
        [
            "SB ",
            "BA ",
            "SYN "
        ];

        public int Compare(ICardMetric? x, ICardMetric? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x is not CardLetterGradeMetric xMetric)
            {
                throw new ArgumentException("First comparer parameter is not a CardLetterGradeMetric");
            }

            if (y is not CardLetterGradeMetric yMetric)
            {
                throw new ArgumentException("Second comparer parameter is not a CardLetterGradeMetric");
            }

            if (!xMetric.HasValue && !yMetric.HasValue)
            {
                return 0;
            }

            if (!xMetric.HasValue)
            {
                return -1;
            }

            if (!yMetric.HasValue)
            {
                return 1;
            }

            var xValue = xMetric.Value;
            foreach(var prefix in PREFIXES_TO_IGNORE)
            {
                if(xValue.StartsWith(prefix))
                {
                    xValue = xValue.Substring(prefix.Length);
                }
            }

            var yValue = yMetric.Value;
            foreach (var prefix in PREFIXES_TO_IGNORE)
            {
                if (yValue.StartsWith(prefix))
                {
                    yValue = yValue.Substring(prefix.Length);
                }
            }

            var xIntValue = GRADE_TO_VALUE.GetValueOrDefault(xValue, -1);
            var yIntValue = GRADE_TO_VALUE.GetValueOrDefault(yValue, -1);
            return Comparer<int>.Default.Compare(xIntValue, yIntValue);
        }
    };
}


