public abstract class Ancestor
{
    private bool _isLiving;
    private string _gender;
    private protected string _pid;
    private protected string _ancestorName;

    public Ancestor (string name, bool isLiving, string gender, string pid)
    {
        _ancestorName = name;
        _isLiving = isLiving;
        _gender = gender;
        _pid = pid;
    }

    public abstract void PrintAncestor(int number);
    public abstract string PrintName();

    public bool GetLiving()
    {
        return _isLiving;
    }

    public string GetPid()
    {
        return _pid;
    }
}