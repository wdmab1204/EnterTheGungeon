using System.Collections.Generic;
using System.Linq;

public class TagContainer
{
    private Dictionary<int, string> idToTag;
    private Dictionary<string, int> tagToId;

    public TagContainer(Dictionary<int, string> container)
    {
        idToTag = container;
        tagToId = container.ToDictionary(pair => pair.Value, pair => pair.Key);
    }

    public string GetTag(int id) => idToTag[id];
    public int GetID(string tag) => tagToId[tag];
}
