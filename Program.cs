class Program
{
    static void Main()
    {
        var building = Building.Instance;

        building.AddElevator(new Elevator(building, 5));
        building.AddElevator(new Elevator(building, 5));
        building.AddElevator(new Elevator(building, 5));

        var display = new ElevatorDisplay(10, building);

        new Thread(() =>
        {
            Thread.Sleep(100);
            new Person(1, 5).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(3, 7).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(7, 2).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(4, 1).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(6, 9).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(8, 3).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(2, 6).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(9, 4).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(5, 8).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(10, 1).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(2, 7).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(4, 9).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(7, 3).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(6, 1).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(8, 10).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(5, 2).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(3, 8).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(9, 6).CallElevatorForDown(building);

            Thread.Sleep(100);
            new Person(1, 4).CallElevatorToUp(building);

            Thread.Sleep(100);
            new Person(10, 7).CallElevatorToUp(building);
        }).Start();



        while (true)
        {
            display.Render();
            Thread.Sleep(50);
        }
    }
}
