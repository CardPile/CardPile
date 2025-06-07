using System.Numerics;

namespace CardPile.CardData.Importance;

public static class ImportanceCalculators
{
    static public Func<T, ImportanceLevel> Critical<T>()
    {
        return Constant<T>(ImportanceLevel.Critical);
    }

    static public Func<T, ImportanceLevel> High<T>()
    {
        return Constant<T>(ImportanceLevel.High);
    }

    static public Func<T, ImportanceLevel> Regular<T>()
    {
        return Constant<T>(ImportanceLevel.Regular);
    }

    static public Func<T, ImportanceLevel> Low<T>()
    {
        return Constant<T>(ImportanceLevel.Low);
    }

    static public Func<T, ImportanceLevel> Constant<T>(ImportanceLevel level)
    {
        return (T) => level;
    }

    static public Func<T, ImportanceLevel> AboveOffset<T>(T criticalOffset, T highOffset, Func<T> referenceValueSelector) where T : INumber<T>
    {
        return (T value) =>
        {
            var thresholdValue = referenceValueSelector();
            if (value >= thresholdValue + criticalOffset)
            {
                return ImportanceLevel.Critical;
            }
            if (value >= thresholdValue + highOffset)
            {
                return ImportanceLevel.High;
            }
            if (value >= thresholdValue)
            {
                return ImportanceLevel.Regular;
            }
            return ImportanceLevel.Low;
        };
    }

    static public Func<T, ImportanceLevel> BelowOffset<T>(T criticalOffset, T highOffset, Func<T> referenceValueSelector) where T : INumber<T>
    {
        return (T value) =>
        {
            var thresholdValue = referenceValueSelector();
            if (value <= thresholdValue - criticalOffset)
            {
                return ImportanceLevel.Critical;
            }
            if (value <= thresholdValue - highOffset)
            {
                return ImportanceLevel.High;
            }
            if (value <= thresholdValue)
            {
                return ImportanceLevel.Regular;
            }
            return ImportanceLevel.Low;
        };
    }

    static public Func<T, ImportanceLevel> AboveThreshold<T>(T criticalThreshold, T highThreshold, T regularThreshold, Func<T, T> valueMap) where T : INumber<T>
    {
        return (T value) =>
        {
            var mappedValue = valueMap(value);
            if (mappedValue >= criticalThreshold)
            {
                return ImportanceLevel.Critical;
            }
            if (mappedValue >= highThreshold)
            {
                return ImportanceLevel.High;
            }
            if (mappedValue >= regularThreshold)
            {
                return ImportanceLevel.Regular;
            }
            return ImportanceLevel.Low;
        };
    }

    static public Func<T, ImportanceLevel> AboveThreshold<T>(T criticalThreshold, T highThreshold, T regularThreshold) where T : INumber<T>
    {
        return AboveThreshold<T>(criticalThreshold, highThreshold, regularThreshold, x => x);
    }
}