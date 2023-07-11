using System.Collections.Generic;

public class HexCellPriorityQueue
{
    public int Count { get; private set; }
    private List<HexCell> list = new List<HexCell>();
    private int minimum = int.MaxValue;

    public void Enqueue(HexCell cell)
    {
        Count++;
        int priority = cell.SearchPriority;
        if (priority < minimum)
        {
            minimum = priority;
        }
        while (priority >= list.Count)
        {
            list.Add(null);
        }
        cell.nextWithSamePriority = list[priority];
        list[priority] = cell;
    }

    public HexCell Dequeue()
    {
        Count--;
        for (; minimum < list.Count; minimum++)
        {
            HexCell cell = list[minimum];
            if (cell != null)
            {
                list[minimum] = cell.nextWithSamePriority;
                return cell;
            }
        }
        return null;
    }

    public void Change(HexCell cell, int oldPriority)
    {
        HexCell current = list[oldPriority];
        HexCell next = current.nextWithSamePriority;
        if (current == cell)
        {
            list[oldPriority] = next;
        }
        else
        {
            while (next != cell)
            {
                current = next;
                next = current.nextWithSamePriority;
            }
            current.nextWithSamePriority = cell.nextWithSamePriority;
        }
        Enqueue(cell);
        Count--;
    }

    public void Clear()
    {
        list.Clear();
        Count = 0;
        minimum = int.MaxValue;
    }
}
