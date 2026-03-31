using System;

public class BulletDataContainer
{
    private BulletObjectData[] buffer;
    private int capacity;
    private int activeCount;

    public BulletDataContainer(BulletObjectData[] bullets, int capacity)
    {
        buffer = bullets;
        this.capacity = capacity;
        this.activeCount = capacity;
    }

    public Span<BulletObjectData> GetActiveBullets() => buffer.AsSpan(0, activeCount);

    public void Remove(int index)
    {
        if (index < 0 || index >= activeCount) return;

        buffer[index] = buffer[activeCount - 1];
        activeCount--;
    }

    public void Add(BulletObjectData data)
    {
        if (activeCount < capacity)
        {
            buffer[activeCount] = data;
            activeCount++;
        }
    }

    public int Capacity => capacity;
    public int Count => activeCount;
}
