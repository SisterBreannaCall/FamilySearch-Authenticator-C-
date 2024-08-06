using MemoriesResource;
using Newtonsoft.Json;

public class MemoriesManager
{
    private int _memoryIndex = 0;
    private int _memoryChosen = 0;
    private int _personMemoriesIndex = 0;
    private List<string> _textMemoryTitle = new List<string>();
    private List<string> _textMemoryLocations = new List<string>();
    private static MemoriesManager _instance = new MemoriesManager();
    private List<MemoriesJson> _memoriesJson = new List<MemoriesJson>();

    private MemoriesManager()
    {

    }

    public static MemoriesManager Instance
    {
        get { return _instance; }
    }

    public string GetMemoryCount()
    {
        return _memoryIndex.ToString();
    }

    public void SetMemorySet(string response)
    {
        _memoriesJson.Add(JsonConvert.DeserializeObject<MemoriesJson>(response));
        _memoryIndex += _memoriesJson[_personMemoriesIndex].sourceDescriptions.Count;
        _personMemoriesIndex += 1;
    }

    public void ParseMemoryData()
    {
        for (int i = 0; i < _memoriesJson.Count; i++)
        {
            for (int j = 0; j < _memoriesJson[i].sourceDescriptions.Count; j++)
            {
                if (_memoriesJson[i].sourceDescriptions[j].mediaType == "text/plain")
                {
                    if (_memoriesJson[i].sourceDescriptions[j].titles != null)
                    {
                        _textMemoryLocations.Add(_memoriesJson[i].sourceDescriptions[j].about);
                        _textMemoryTitle.Add(_memoriesJson[i].sourceDescriptions[j].titles[0].value);
                    }
                    else if (_memoriesJson[i].sourceDescriptions[j].titles == null)
                    {
                        _textMemoryLocations.Add(_memoriesJson[i].sourceDescriptions[j].about);
                        _textMemoryTitle.Add("");
                    }
                }
            }
        }

        SelectMemory();
    }

    public void SelectMemory()
    {
        if (_textMemoryLocations.Count > 0)
        {
            Console.WriteLine();
            for (int i = 0; i < _textMemoryLocations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_textMemoryTitle[i]}");
            }
            Console.Write("Select a choice from the menu: ");
            string usereInputString = Console.ReadLine();
            _memoryChosen = int.Parse(usereInputString);
        }
        if (_textMemoryLocations.Count == 0)
        {
            _memoryChosen = 5000;
        }
    }

    public void ClearMemoryData()
    {
        _memoriesJson.Clear();
        _personMemoriesIndex = 0;
        _memoryIndex = 0;
        _textMemoryLocations.Clear();
        _textMemoryTitle.Clear();
        _memoryChosen = 0;
    }

    public void SetZero()
    {
        _memoryChosen = 0;
    }

    public string PrintMemoryName()
    {
        if (_memoryChosen == 0)
        {
            return "none currently";
        }
        else if (_memoryChosen == 5000)
        {
            return "No text memories found. Please choose a different ancestor.";
        }
        else
        {
            return $"{_textMemoryTitle[_memoryChosen - 1]}";
        }
    }

    public string GetLocation()
    {
        if (_memoryChosen != 0)
        {
            if (_memoryChosen != 5000)
            {
                return _textMemoryLocations[_memoryChosen - 1];
            }
        }

        return "";
    }
}