using System;

class Program
{
    static void Main()
    {
        var building = new Building(totalFloors: 9, elevatorCount: 2);
        var display = new ElevatorDisplay(building);
        var controller = new SimulationController(building, display);

        PersonManager.InitializePersonRequests(building, personCount: 20);
        controller.Run();
    }
}
