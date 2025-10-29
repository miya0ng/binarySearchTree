
public class GameTask
{
    public string Name { get; }
    public GameTask(string name) => Name = name;
    public override string ToString() => Name;
}