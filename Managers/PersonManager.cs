using System;
using System.Threading;

public static class PersonManager
{
    private static readonly Random _random = new Random();

    public static void InitializePersonRequests(Building building, int personCount)
    {
        int cumulativeDelay = 0; 

        for (int i = 0; i < personCount; i++)
        {
            int individualDelay = _random.Next(500, 1500); 
            cumulativeDelay += individualDelay; 
            int delayForThisPerson = cumulativeDelay;

            new Thread(() =>
            {
                Thread.Sleep(delayForThisPerson);
                int startFloor = _random.Next(1, building.TotalFloors + 1);
                int targetFloor;

                do
                {
                    targetFloor = _random.Next(1, building.TotalFloors + 1);
                } while (targetFloor == startFloor);

                var person = new Person(startFloor, targetFloor);

                if (targetFloor > startFloor)
                {
                    person.RequestElevatorUp(building);
                }
                else
                {
                    person.RequestElevatorDown(building);
                }
            }).Start();
        }
    }
}
