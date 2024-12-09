using System;
using System.Threading;

public static class PersonManager
{
    public static void InitializePersonRequests(Building building, int personCount)
    {
        for (int i = 0; i < personCount; i++)
        {
            int personId = i + 1;
            int startFloor = new Random().Next(1, 6);
            int targetFloor = new Random().Next(6, 10);

            new Thread(() =>
            {
                Thread.Sleep(personId * 1000);
                var person = new Person(startFloor, targetFloor);
                person.RequestElevatorUp(building);
            }).Start();
        }
    }
}