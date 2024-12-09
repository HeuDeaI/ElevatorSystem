using System;
using System.Linq;

public class ElevatorDisplay
{
    private readonly Building _building;

    public ElevatorDisplay(Building building)
    {
        _building = building;
    }

    public void Render()
    {
        Console.Clear();
        for (int i = _building.TotalFloors; i >= 1; i--)
        {
            string waitingPeople = GetWaitingPeople(i);
            string elevatorDisplay = GetElevatorAtFloor(i);

            Console.WriteLine($"{i}: {waitingPeople,-15} {elevatorDisplay}");
        }

        Console.WriteLine(new string('-', 30));
    }

    private string GetWaitingPeople(int floor)
    {
        var upQueue = _building.UpwardQueues;
        var downQueue = _building.DownwardQueues;

        string up = upQueue.TryGetValue(floor, out var upPeople) ? string.Join(" ", upPeople.Select(p => $"#{p.Id}↑")) : "";
        string down = downQueue.TryGetValue(floor, out var downPeople) ? string.Join(" ", downPeople.Select(p => $"#{p.Id}↓")) : "";

        return $"{up} {down}".Trim();
    }

    private string GetElevatorAtFloor(int floor)
    {
        return string.Join(" ", _building.Elevators
            .Where(e => e.CurrentFloor == floor)
            .Select(e => $"E{e.Id}[{string.Join(" ", e.Passengers.Select(p => $"#{p.Id}"))}]"));
    }
}
