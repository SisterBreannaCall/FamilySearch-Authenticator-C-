public class FemaleAncestor : Ancestor
{
    public FemaleAncestor(string name, bool isLiving, string gender, string pid) : base (name, isLiving, gender, pid)
    {
        
    }

    public override void PrintAncestor(int number)
    {
        Console.WriteLine($"{number}. Sister {_ancestorName} ({_pid})");
    }

    public override string PrintName()
    {
        return $"Sister {_ancestorName}";
    }
}