using UnitySpy.ProcessFacade;
using UnitySpy.Offsets;
using UnitySpy;
using System.Diagnostics;
using NLog;
using CardPile.Draft;

namespace CardPile.Parser;

public class MemoryWatcher
{
    public event EventHandler<DraftChoiceEvent>? DraftChoiceEvent;

    public MemoryWatcher()
    { }

    public bool Paused { get; set; }

    public void Poll()
    {
        var polledState = PollDraftState();
        if (polledState == null)
        {
            return;
        }

        var draftChoiceEvent = DraftChoiceEvent;
        if (draftChoiceEvent != null)
        {
            draftChoiceEvent(this, new DraftChoiceEvent(polledState.DraftId, polledState.PackNumber, polledState.PickNumber, polledState.CardsInPack));
        }
    }

    private class DraftState
    {
        internal DraftState(Guid draftId, int packNumber, int pickNumber, List<int> cardsInPack)
        {
            DraftId = draftId;
            PackNumber = packNumber;
            PickNumber = pickNumber;
            CardsInPack = cardsInPack;
        }

        internal Guid DraftId;
        internal int PackNumber;
        internal int PickNumber;
        internal List<int> CardsInPack;
    }

    private DraftState? PollDraftState()
    {
        try
        {
            var assemblyImage = GetMtgaAssemblyImage();
            if (assemblyImage == null)
            {
                return default;
            }

            var wrapperControllerType = assemblyImage.GetTypeDefinition(NeedleTypeName);
            if (wrapperControllerType == null)
            {
                logger.Warn("Could not find [{TypeName}] type", NeedleTypeName);
                return default;
            }

            var instanceFieldValue = wrapperControllerType.GetStaticValue<IManagedObjectInstance>(NeedleTypeFieldName);
            if (instanceFieldValue == null)
            {
                logger.Warn("Could not find a static field [{FieldName}] in the [{TypeName}] type", NeedleTypeFieldName, wrapperControllerType.FullName);
                return default;
            }

            var currentNavContentFieldValue = instanceFieldValue.TryToGetPath(CurrentNavContentPath);
            if (currentNavContentFieldValue == null)
            {
                return default;
            }

            if (currentNavContentFieldValue.TypeDefinition.Name != CurrentNavContentTypeNameNeedle)
            {
                return default;
            }

            if (!GetDraftInfo(currentNavContentFieldValue, out Guid draftId, out int packNumber, out int pickNumber))
            {
                return default;
            }

            var cardsInPack = GetCardsInPack(currentNavContentFieldValue);
            if (cardsInPack == null)
            {
                return default;
            }

            return new DraftState(draftId, packNumber, pickNumber, cardsInPack);
        }
        catch (Exception e)
        {
            logger.Error(e);
            throw;
        }
    }

    private static List<int>? GetCardsInPack(IManagedObjectInstance currentNavContentFieldValue)
    {
        var itemListFieldValue = currentNavContentFieldValue.TryToGetPath(DraftItemListPath);
        if (itemListFieldValue == null)
        {
            return default;
        }

        var items = itemListFieldValue.TryToGetValue<object[]>("_items");
        if (items == null)
        {
            return default;
        }

        var itemsCount = itemListFieldValue.TryToGetValue<int>("_size");
        logger.Info("Found {ItemCount} items", itemsCount);

        List<int> cardChoices = [];
        for (int i = 0; i < itemsCount; i++)
        {
            // These items are in opposite order
            var item = items[i];
            if (item == null)
            {
                logger.Warn("Item {ItemIndex} is null", i);
                return default;
            }

            var itemInstance = item as IManagedObjectInstance;
            if (itemInstance == null)
            {
                logger.Warn("Item {ItemIndex} of draft items is of type {TypeName} but IManagedObjectInstance was expected", i, item.GetType().FullName);
                return default;
            }

            var quantity = itemInstance.TryToGetValue<int>(CardCollectionItemQuantityNeedle);

            var recordFieldValue = itemInstance.TryToGetPath(RecordPath);
            if (recordFieldValue == null)
            {
                logger.Warn("Cannot get the record field of the draft item {ItemIndex}", i);
                return default;
            }

            var grpId = recordFieldValue.TryToGetValue<uint>("GrpId");
            for(int q = 0; q < quantity; ++q)
            {
                cardChoices.Add((int)grpId);
            }
        }

        cardChoices.Reverse();

        return cardChoices;
    }

    private static bool GetDraftInfo(IManagedObjectInstance currentNavContentFieldValue, out Guid draftId, out int packNumber, out int pickNumber)
    {
        draftId = Guid.Empty;
        packNumber = 1;
        pickNumber = 1;

        var draftPodFieldValue = currentNavContentFieldValue.TryToGetPath(DraftPodPath);
        if (draftPodFieldValue == null)
        {
            return false;
        }
      
        if(draftPodFieldValue.TypeDefinition.Name == BotDraftPodTypeNameNeedle)
        {
            // NOOP
            return false;
        }
        else if(draftPodFieldValue.TypeDefinition.Name == HumanDraftPodTypeNameNeedle)
        {
            var retreivedDraftId = draftPodFieldValue.TryToGetValue<string>("<DraftId>k__BackingField");
            if (retreivedDraftId == null)
            {
                logger.Warn("Error getting value of DraftId from {TypeName}", draftPodFieldValue.TypeDefinition.Name);
                return false;
            }

            var currentPickInfoFieldValue = draftPodFieldValue.TryToGetValue<IManagedObjectInstance>("_currentPickInfo");
            if(currentPickInfoFieldValue == null)
            {
                return false;
            }

            var retreivedPackNumber = currentPickInfoFieldValue.TryToGetValue<int>("SelfPack");
            var retreivedPickNumber = currentPickInfoFieldValue.TryToGetValue<int>("SelfPick");

            if (Guid.TryParse(retreivedDraftId, out Guid parsedDraftId))
            {
                draftId = parsedDraftId;
            }
            else
            {
                logger.Warn("Error parsing value of DraftId equal to {TypeName}", retreivedDraftId);
            }
            packNumber = retreivedPackNumber;
            pickNumber = retreivedPickNumber;

            return true;
        }
        else
        {
            logger.Warn("Unexpected type of the draft pod field {TypeName}", draftPodFieldValue.TypeDefinition.Name);
            return false;
        }
    }

    private static List<int>? GetPackCardsFromHumanDraft(IManagedObjectInstance currentPickInfoFieldValue)
    {
        var packCardsListFieldValue = currentPickInfoFieldValue.TryToGetPath("PackCards");
        if(packCardsListFieldValue == null)
        {
            return default;
        }

        var items = packCardsListFieldValue.TryToGetValue<int[]>("_items");
        if (items == null)
        {
            return default;
        }

        logger.Info("Found {ItemCount} items in PackCards", items.Length);

        return [.. items];
    }

    private IAssemblyImage? GetMtgaAssemblyImage()
    {
        Process? mtgaProcess = Process.GetProcessesByName("MTGA").FirstOrDefault();
        if (mtgaProcess == null)
        {
            return default;
        }

        UnityProcessFacade unityProcessFacade;
        if(OperatingSystem.IsWindows())
        {
            var processFacade = new ProcessFacadeWindows(mtgaProcess);
            var monoLibraryOffsets = MonoLibraryOffsets.GetOffsets(processFacade.GetMainModuleFileName());
            unityProcessFacade = new UnityProcessFacade(processFacade, monoLibraryOffsets);
        }
        else if(OperatingSystem.IsMacOS())
        {
            // MTGA for MacOS uses il2cpp - a different soltion is required
            return default;
            
            /*
            if(mtgaProcess.MainModule == null)
            {
                logger.Warn("Found MTGA process, but MainModule is null");
                return default;
            }

            var processFacade = new ProcessFacadeMacOSDirect(mtgaProcess);
            var monoLibraryOffsets = MonoLibraryOffsets.GetOffsets(mtgaProcess.MainModule.FileName);
            unityProcessFacade = new UnityProcessFacade(processFacade, monoLibraryOffsets);
            */
        }
        else
        {
            throw new NotSupportedException($"Unsupported operating system {Environment.OSVersion}");
        }

        return AssemblyImageFactory.Create(unityProcessFacade, "Core");
    }

    private const string NeedleTypeName = "WrapperController";
    private const string NeedleTypeFieldName = "<Instance>k__BackingField";
    private static readonly string[] CurrentNavContentPath = ["<SceneLoader>k__BackingField", "<CurrentNavContent>k__BackingField"];
    private const string CurrentNavContentTypeNameNeedle = "DraftContentController";
    private static readonly string[] DraftItemListPath = ["_draftPackHolder", "_packCollection", "_itemList"];
    private const string CardCollectionItemQuantityNeedle = "<Quantity>k__BackingField";
    private static readonly string[] RecordPath = ["<Card>k__BackingField", "_printing", "Record"];
    private static readonly string[] DraftPodPath = ["_limitedEvent", "<DraftPod>k__BackingField"];
    private const string BotDraftPodTypeNameNeedle = "BotDraftPod";
    private const string HumanDraftPodTypeNameNeedle = "HumanDraftPod";

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
