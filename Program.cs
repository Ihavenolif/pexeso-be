using System;
using Fleck;

class Program
{
    public static void Main(String[] args)
    {

        NetworkManager networkManager = new NetworkManager("ws://0.0.0.0:6969");

        networkManager.CreateLobby("IAO Draft 2025");

        for (int i = 0; i < 10; i++)
        {
            networkManager.CreateLobby($"test {i}");
        }

        Console.ReadLine();
    }
}