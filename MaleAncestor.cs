public class MaleAncestor : Ancestor
{
    public MaleAncestor(string name, bool isLiving, string gender, string pid) : base (name, isLiving, gender, pid)
    {
        
    }

    public override void PrintAncestor(int number)
    {
        Console.WriteLine($"{number}. Brother {_ancestorName} ({_pid})");
    }

    public override string PrintName()
    {
        return $"Brother {_ancestorName}";
    }
}