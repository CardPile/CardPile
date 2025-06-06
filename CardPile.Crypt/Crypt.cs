﻿using NLog;
using Tomlet;

namespace CardPile.Crypt;

public class Crypt
{
    public Crypt()
    {}

    public List<Skeleton> Skeletons { get; init; } = [];

    public void UpdateSkeletons(List<int> cardIds)
    {
        foreach (var skelton in Skeletons)
        {
            skelton.Update(cardIds);
        }
    }

    public void LoadSkeletons(string skeletonDirectory)
    {
        if (!Directory.Exists(skeletonDirectory))
        {
            return;
        }

        Skeletons.Clear();

        var filePaths = Directory.GetFiles(skeletonDirectory, "*.toml", SearchOption.AllDirectories);
        foreach (var filePath in filePaths)
        {
            try
            {
                logger.Info("Loading skeleton from {filePath}", filePath);

                var document = TomlParser.ParseFile(filePath);
                var skeleton = Skeleton.TryLoad(document);
                if (skeleton == null)
                {
                    logger.Error("Error parsing skeleton file {filePath}", filePath);
                    continue;
                }

                if(!skeleton.CanBeSatisfied())
                {
                    logger.Warn("Skeleton {name} at {filePath} cannot be satisfied.", skeleton.Name, filePath);
                }

                Skeletons.Add(skeleton);
            }
            catch (Exception ex)
            {
                logger.Error("Error parsing skeleton {filePath}. Exception: {exception}", filePath, ex);
            }
        }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
