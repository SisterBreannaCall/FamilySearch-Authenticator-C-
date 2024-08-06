public class MenuManager {
    private bool _isLoggedIn = false;
    private bool _terminateLoop = false;
    private bool _isAncestorSelected = false;
    private static MenuManager _instance = new MenuManager();

    private MenuManager()
    {

    }

    public static MenuManager Instance
    {
        get { return _instance; }
    }

    public void Start()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("FamilySearch Text Memories to Speech");
            Console.WriteLine();
            
            if (_isLoggedIn)
            {
                Console.WriteLine("FamilySearch Status: logged in");
            }
            else if (!_isLoggedIn)
            {
                Console.WriteLine("FamilySearch Status: logged out");
            }
            
            if (_isAncestorSelected)
            {
                Console.WriteLine($"Ancestor Selected: {AncestryManager.Instance.PrintAncestor()}");
            }
            else if (!_isAncestorSelected)
            {
                Console.WriteLine("Ancestor Selected: none currently");
            }
            
            Console.WriteLine($"Text Memory: {MemoriesManager.Instance.PrintMemoryName()}");

            Console.WriteLine();
            Console.WriteLine("Menu Options:");
            Console.WriteLine("   1. Login to FamilySearch");
            Console.WriteLine("   2. Select an ancestor");
            Console.WriteLine("   3. Get text memories attached to ancestor");
            Console.WriteLine("   4. Read text memory attached to ancestor");
            Console.WriteLine("   5. Quit");
            Console.Write("Select a choice from the menu: ");
            string userInputString = Console.ReadLine();
            int userInput = int.Parse(userInputString);

            if (userInput == 1)
            {
                if (!_isLoggedIn)
                {
                    _isLoggedIn = true;
                    FamilySearchAuth.Instance.InitAuth();
                }
            }
            else if (userInput == 2)
            {
                if (_isLoggedIn)
                {
                    if (!_isAncestorSelected)
                    {
                        _isAncestorSelected = true;
                        FamilySearchAuth.Instance.GetCurrentUser();
                    }
                    else if (_isAncestorSelected)
                    {
                        AncestryManager.Instance.SelectAncestor();
                    }
                }
            }

            else if (userInput == 3)
            {
                if (_isLoggedIn)
                {
                    if (_isAncestorSelected)
                    {
                        MemoriesManager.Instance.ClearMemoryData();
                        FamilySearchAuth.Instance.GetMemories();
                    }
                }
            }
            else if (userInput == 4)
            {
                if (_isLoggedIn)
                {
                    if (_isAncestorSelected)
                    {
                        string memoryLocation = MemoriesManager.Instance.GetLocation();

                        if (memoryLocation != "")
                        {
                            FamilySearchAuth.Instance.GetMemory(memoryLocation);
                        }
                    }
                }
            }
            else if (userInput == 5)
            {
                _terminateLoop = true;
            }

        }
        while (!_terminateLoop);
    }
}
