using System.Collections.Generic;

public class HexCellPriorityQueue
{
    public int Count { get; private set; }
    private List<HexCell> _list = new List<HexCell>();
    private int _minimum = int.MaxValue;

    public void Enqueue(HexCell cell)
    {
        Count++;
        int priority = cell.SearchPriority;
        if (priority < _minimum)
        {
            _minimum = priority;
        }
        while (priority >= _list.Count)
        {
            _list.Add(null);
        }
        cell.NextWithSamePriority = _list[priority];
        _list[priority] = cell;
    }

    public HexCell Dequeue()
    {
        Count--;
        for (; _minimum < _list.Count; _minimum++)
        {
            HexCell cell = _list[_minimum];
            if (cell != null)
            {
                _list[_minimum] = cell.NextWithSamePriority;
                return cell;
            }
        }
        return null;
    }

    public void Change(HexCell cell, int oldPriority)
    {
        HexCell current = _list[oldPriority];
        HexCell next = current.NextWithSamePriority;
        if (current == cell)
        {
            _list[oldPriority] = next;
        }
        else
        {
            while (next != cell)
            {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;
        }
        Enqueue(cell);
        Count--;
    }

    public void Clear()
    {
        _list.Clear();
        Count = 0;
        _minimum = int.MaxValue;
    }
}
