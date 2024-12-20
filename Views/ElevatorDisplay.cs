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
        Console.WriteLine("Elevator Simulation");
        Console.WriteLine(new string('=', 40));

        for (int i = _building.TotalFloors; i >= 1; i--)
        {
            string waitingPeople = GetWaitingPeople(i);
            string elevatorDisplay = GetElevatorAtFloor(i);

            Console.WriteLine($"{i,2}: [{waitingPeople,-20}] {elevatorDisplay}");
        }

        Console.WriteLine(new string('-', 40));
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
        var elevatorsAtFloor = _building.Elevators
            .Where(e => e.CurrentFloor == floor)
            .Select(e =>
            {
                var arriving = e.ArrivingPassengers.Select(p => $"#{p.Id}");
                var remaining = e.Passengers.Select(p => $"#{p.Id}");
                string elevatorDisplay = $"E{e.Id}" +
                                        (remaining.Any() ? $"[{string.Join(" ", remaining)}]" : "") +
                                        (arriving.Any() ? $" -> {string.Join(" ", arriving)}" : "");
                e.ArrivingPassengers.Clear();
                return elevatorDisplay;
            });

        return string.Join(" ", elevatorsAtFloor);
    }
}
