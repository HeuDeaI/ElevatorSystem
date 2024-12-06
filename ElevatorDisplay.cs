using System;
using System.Collections.Generic;
using System.Linq;

public class ElevatorDisplay
{
    private readonly int _totalFloors;
    private readonly Building _building;

    public ElevatorDisplay(int totalFloors, Building building)
    {
        _totalFloors = totalFloors;
        _building = building;
    }

    public void Render()
    {
        Console.Clear();
        var elevators = _building.Elevators;
        var waitingUp = _building.QueueToUp;
        var waitingDown = _building.QueueToDown;

        for (int i = _totalFloors; i >= 1; i--)
        {
            string waitingPeople = GetWaitingPeopleDisplay(i, waitingUp, waitingDown);
            string elevatorSymbol = GetElevatorDisplayAtFloor(i, elevators);
            Console.WriteLine($"{i}: {waitingPeople,-15} {elevatorSymbol}");
        }

        Console.WriteLine(new string('-', 30));
    }

    private string GetWaitingPeopleDisplay(int floor, Dictionary<int, Queue<Person>> queueUp, Dictionary<int, Queue<Person>> queueDown)
    {
        var up = queueUp.ContainsKey(floor) ? string.Join(" ", queueUp[floor].Select(p => $"#{p.Id}↑")) : "";
        var down = queueDown.ContainsKey(floor) ? string.Join(" ", queueDown[floor].Select(p => $"#{p.Id}↓")) : "";
        return $"{up} {down}".Trim();
    }

    private string GetElevatorDisplayAtFloor(int floor, List<Elevator> elevators)
    {
        return string.Join(" ", elevators
            .Where(e => e.CurrentFloor == floor)
            .Select(e => $"E{e.Id}[{string.Join(" ", e.Passengers.Select(p => $"#{p.Id}"))}]"));
    }
}
