using System;
using System.Collections.Generic;
using System.IO;

namespace SaberTest
{
    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }


    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            ListNode current = Head;

            string serializeString = $"<ListRand>{Environment.NewLine}";
            int id = 0;

            //для снижения алгоритмической сложности, id элементов сохраняем в словарь
            Dictionary<ListNode, int> nodeDict = new Dictionary<ListNode, int>();
            while (current != null)
            {
                nodeDict.Add(current, id);
                current = current.Next;
                id++;
            }


            //write data and index of each member in string
            current = Head;
            while (current != null)
            {
                serializeString += 
                    $" <ListNode>{Environment.NewLine}" +
                    $"  <Id>{nodeDict[current]}</Id>{Environment.NewLine}" +
                    $"  <Data>{current.Data}</Data>{Environment.NewLine}" +
                    $"  <RandId>{nodeDict[current.Rand]}</RandId>{Environment.NewLine}" +
                    $" </ListNode>{Environment.NewLine}";

                //старый алгоритм поиска индека элемента в поле Rand, сложность приближенная к квадратичной из за вложенности
                //заменил его созданием словаря с индексами узлов
                //Find index of a Rand node
                //int randId = 0;
                //ListNode rand = current.Rand;
                //while (rand.Prev != null)
                //{
                //    randId++;
                //    rand = rand.Prev;
                //}
                //serializeString += $"  <RandId>{randId}</RandId>{Environment.NewLine} </ListNode>{Environment.NewLine}";
                current = current.Next;
            }
            serializeString += "</ListRand>";

            using (s)
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(serializeString);
                s.Write(array, 0, array.Length);
            }
        }

        public void Deserialize(FileStream s)
        {
            string textFromFile = "";
            using (s)
            {
                byte[] array = new byte[s.Length];
                s.Read(array, 0, array.Length);
                textFromFile = System.Text.Encoding.Default.GetString(array);
            }

            List<int> randomIdList = new List<int>();
            List<string> dataList = new List<string>();

            if (textFromFile.Contains(Tags.ListRand.Open))
            {
                while (textFromFile.Contains(Tags.ListNode.Open))
                {
                    dataList.Add(StringParcer.GetData(textFromFile, Tags.Data));
                    randomIdList.Add(int.Parse(StringParcer.GetData(textFromFile, Tags.RandId)));
                    textFromFile = textFromFile.Substring(textFromFile.IndexOf(Tags.ListNode.Close) + Tags.ListNode.Close.Length);
                }
            }

            ListRandOperation.DeserialiseListRand(this, dataList, randomIdList);
        }

    }

    static class ListRandOperation
    {
        static Random random = new Random();
        //add new node with data
        public static void Add(ListRand listRand, string data)
        {
            ListNode node = new ListNode() { Data = data };
            //add data and links
            LinkedList(listRand, node);

            //randomize Rand field on all nods in ListRand
            AddRandomLinks(listRand);
        }

        public static void DeserialiseListRand(ListRand listRand, List<string> dataList, List<int> randomIdList)
        {
            foreach (string data in dataList)
            {
                //add data and links to LinkedList
                ListNode node = new ListNode() { Data = data };
                LinkedList(listRand, node);
            }

            //add Rand links 
            AddCurrentRand(listRand, randomIdList);
        }


        private static void LinkedList(ListRand listRand, ListNode node)
        {

            if (listRand.Head == null)
            {
                listRand.Head = node;
            }
            else
            {
                listRand.Tail.Next = node;
                node.Prev = listRand.Tail;
            }
            listRand.Tail = node;
            listRand.Count++;
        }

        private static void AddRandomLinks(ListRand listRand)
        {
            ListNode current = listRand.Head;
            while (current != null)
            {
                int randomIndex = random.Next(listRand.Count);
                ListNode randomNode = listRand.Head;
                for (int i = 0; i < randomIndex; i++)
                {
                    randomNode = randomNode.Next;
                }
                current.Rand = randomNode;
                current = current.Next;
            }
        }

        public static void AddCurrentRand(ListRand listRand, List<int> randomIdList)
        {
            //записываем в словарь id и node
            int index = 0;
            Dictionary<int, ListNode> nodeDict = new Dictionary<int, ListNode>();
            ListNode current = listRand.Head;

            while (current != null)
            {
                nodeDict.Add(index++, current);
                current = current.Next;
            }

            index = 0;
            current = listRand.Head;
            while (current != null)
            {
                current.Rand = nodeDict[randomIdList[index++]];

                //старый алгоритм поиска индека элемента в поле Rand, сложность приближенная к квадратичной из за вложенности
                //заменил его созданием словаря с индексами узлов
                ////search random node
                //ListNode randNode = listRand.Head;
                ////steep for node with id
                //for (int i = 0; i < randomIdList[index]; i++)
                //{
                //    randNode = randNode.Next;
                //}
                //current.Rand = randNode;
                //index++;

                current = current.Next;
            }
        }
    }

    public struct Tag
    {
        string open;
        string close;
        public string Open { get { return open; } }
        public string Close { get { return close; } }
        public Tag(string openTag, string closeTag)
        {
            open = openTag;
            close = closeTag;
        }
    }

    static class Tags
    {
        public static Tag ListRand { get; } = new Tag("<ListRand>", "</ListRand>");
        public static Tag ListNode { get; } = new Tag("<ListNode>", "</ListNode>");
        public static Tag Data { get; } = new Tag("<Data>", "</Data>");
        public static Tag Id { get; } = new Tag("<Id>", "</Id>");
        public static Tag RandId { get; } = new Tag("<RandId>", "</RandId>");
    }

    static class StringParcer
    {
        public static string GetData(string text, Tag tag)
        {
            string data = text.Substring(text.IndexOf(tag.Open) + tag.Open.Length,
                        text.IndexOf(tag.Close) - (text.IndexOf(tag.Open) + tag.Open.Length));
            return data;
        }

    }
}
