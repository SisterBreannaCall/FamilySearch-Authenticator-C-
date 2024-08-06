using Newtonsoft.Json;
using PersonAncestryResource;

public class AncestryManager
{
    private int _ancestorChosen;
    private List<Ancestor> _ancestors = new List<Ancestor>();
    private static AncestryManager _instance = new AncestryManager();

    private AncestryManager()
    {

    }

    public static AncestryManager Instance
    {
        get { return _instance; }
    }

    public void SetAncestors(string response)
    {
        PersonAncestryJson personAncestry = JsonConvert.DeserializeObject<PersonAncestryJson>(response);

        for (int i = 0; i < personAncestry.persons.Count; i++)
        {
            if (personAncestry.persons[i].display.gender == "Male")
            {
                MaleAncestor maleAncestor = new MaleAncestor(
                    personAncestry.persons[i].display.name,
                    personAncestry.persons[i].living,
                    personAncestry.persons[i].display.gender,
                    personAncestry.persons[i].id
                );

                _ancestors.Add(maleAncestor);    
            }

            if (personAncestry.persons[i].display.gender == "Female")
            {
                FemaleAncestor femaleAncestor = new FemaleAncestor(
                    personAncestry.persons[i].display.name,
                    personAncestry.persons[i].living,
                    personAncestry.persons[i].display.gender,
                    personAncestry.persons[i].id
                );

                _ancestors.Add(femaleAncestor);
            }
        }

        SelectAncestor();
    }

    public void SelectAncestor()
    {
        bool terminateLoop = false;

        do
        {
            Console.WriteLine();
            for (int i = 0; i < _ancestors.Count; i++)
            {
                _ancestors[i].PrintAncestor(i + 1);
            }
            Console.WriteLine();
            Console.Write("Select a choice from the menu: ");
            string userInputString = Console.ReadLine();
            int userInput = int.Parse(userInputString);

            if (_ancestors[userInput - 1].GetLiving())
            {
                Console.WriteLine();
                Console.WriteLine("Please choose a deceased ancestor.");
            }
            else if (!_ancestors[userInput - 1].GetLiving())
            {
                _ancestorChosen = userInput - 1;
                MemoriesManager.Instance.SetZero();
                terminateLoop = true;
            }
        }
        while (!terminateLoop);
    }

    public string PrintAncestor()
    {
        return $"{_ancestors[_ancestorChosen].PrintName()}";
    }

    public string GetSelectedAncestorPID()
    {
        return $"{_ancestors[_ancestorChosen].GetPid()}";
    }
}