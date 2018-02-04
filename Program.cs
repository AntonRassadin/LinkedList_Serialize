using System;
using System.IO;

namespace SaberTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ListRand listRand = new ListRand();
            ListRandOperation.Add(listRand, "Tom");
            ListRandOperation.Add(listRand, "Mike");
            ListRandOperation.Add(listRand, "Anna");
            ListRandOperation.Add(listRand, "Kate");
            ListRandOperation.Add(listRand, "Frank");
            ListRandOperation.Add(listRand, "John");
            ListRandOperation.Add(listRand, "Habib");

            ListNode current = listRand.Head;
            while (current != null)
            {
                Console.WriteLine($"Data: {current.Data} Next: {current.Next?.Data} Prev: {current.Prev?.Data} Rand: {current.Rand?.Data}");
                current = current.Next;
            }
            Console.WriteLine();

            listRand.Serialize(new FileStream(@"text.txt", FileMode.Create));

            ListRand deserList = new ListRand();
            deserList.Deserialize(File.OpenRead(@"text.txt"));

            current = deserList.Head;
            while (current != null)
            {
                Console.WriteLine($"Data: {current.Data} Next: {current.Next?.Data} Prev: {current.Prev?.Data} Rand: {current.Rand?.Data}");
                current = current.Next;
            }
            Console.ReadLine();
        }

    }



}
