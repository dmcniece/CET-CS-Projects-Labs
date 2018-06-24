using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Lab1
{
    class Program
    {
        private static List<string> words = new List<string>();
        static void Main(string[] args)
        {
            bool run = true;
            char selection;
            while (run)
            {
                Console.WriteLine("Hello World!!! My First C# App");
                Console.WriteLine("Options");
                Console.WriteLine("----------");
                Console.WriteLine("1 - Import Words From File");
                Console.WriteLine("2 - Bubble Sort Words");
                Console.WriteLine("3 - LINQ/LAMBDA Sort Words");
                Console.WriteLine("4 - Count The Distinct Words");
                Console.WriteLine("5 - Take The First 10 Words");
                Console.WriteLine("6 - Get the number of words that start with 'j' and display the count");
                Console.WriteLine("7 - Get and display of words that end with 'd' and display the count");
                Console.WriteLine("8 - Get and display of words that are greater than 4 characters long, and display the count");
                Console.WriteLine("9 - Get and display of words that are less than 3 characters long and starts with 'a', and display the count");
                Console.WriteLine("x - Exit");
                Console.WriteLine();
                Console.Write("Make a Selection: ");
                /*can use either one*/
                //selection = Console.ReadKey().KeyChar;
                selection = (char)Console.Read();
                Console.ReadLine();
                //Console.WriteLine();
                switch (selection)
                {
                    case '1':
                        //Console.WriteLine("Import Words From File");
                        readFile();
                        break;
                    case '2':
                        //Console.WriteLine("Bubble Sort Words");
                        bubbleSort(words);
                        break;
                    case '3':
                        Console.WriteLine("LINQ/LAMBDA Sort Words");
                        lambdaSort(words);
                        break;
                    case '4':
                        //Console.WriteLine("Count The Distinct Words");
                        countDistinctWords(words);
                        break;
                    case '5':
                        //Console.WriteLine("Take the First 10 Words");
                        firstTenWords(words);
                        break;
                    case '6':
                        //Console.WriteLine("Get the number of words that start with 'j' and display the count");
                        startsWithJCount(words);
                        break;
                    case '7':
                        //Console.WriteLine("Get and display of words that end with 'd' and display the count");
                        endsWithDCount(words);
                        break;
                    case '8':
                        //Console.WriteLine("Get and display of words that are greater than 4 characters long, and display the count");
                        greaterThanFourChars(words);
                        break;
                    case '9':
                        //Console.WriteLine("Get and display of words that are less than 3 characters long and start with the letter 'a', and display the count");
                        lessThanThreeChars(words);
                        break;
                    case 'x':
                        Console.WriteLine("Exit");
                        run = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Selection");
                        break;
                }
            }
        }
        private static void readFile()
        {
            Console.Clear();
            StreamReader reader = new StreamReader("Words.txt");
            string word;
            int counter = 0;
            Console.WriteLine("Reading Words");
            while((word = reader.ReadLine()) != null)
            {
                words.Add(word);
                counter++;
            }
            Console.WriteLine("Reading Words Complete");
            Console.WriteLine("Number of words found: {0}", counter);
            Console.WriteLine();
            //foreach(var el in words)
            //{
            //    Console.WriteLine(el);
            //}
        }

        //private static List<string> bubbleSort(List<string> words)
        //{
        //    Console.Clear();
        //    Stopwatch watch = Stopwatch.StartNew();
        //    for(int i = 0; i < (words.Count - 1); i++)
        //    {
        //        if(string.Compare(words[i], words[i + 1]) < 0)
        //        {
        //            string tempString = words[i];
        //            words[i] = words[i + 1];
        //            words[i + 1] = tempString;
        //        }
        //    }
        //    watch.Stop();
        //    Console.WriteLine("Elapsed Time: {0}", watch.Elapsed);
        //    //TimeSpan ts = watch.Elapsed;
        //    //Console.WriteLine("Elapsed Time: {0}", ts.Milliseconds);
        //    Console.WriteLine();
        //    return words;
        //}

        private static List<string> bubbleSort(List<string> words)
        {
            Console.Clear();
            Stopwatch watch = Stopwatch.StartNew();
            for(int i = 0; i < (words.Count - 1); i++)
            {
                for(int j = i + 1; j < words.Count; j++)
                {
                    if (string.Compare(words[j], words[i]) < 0)
                    {
                        string tempString = words[j];
                        words[j] = words[i];
                        words[i] = tempString;
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("Elapsed Time: {0} ms", watch.ElapsedMilliseconds);
            Console.WriteLine();
            return words;
        }

        private static List<string> lambdaSort(List<string> words)
        {
            Console.Clear();
            Stopwatch watch = Stopwatch.StartNew();
            //var query = from x in words orderby x select x;
            var query =  words.OrderBy(str => str).ToList();
            words = query;
            watch.Stop();
            Console.WriteLine("Elapsed Time: {0} ms", watch.ElapsedMilliseconds);
            return words;
        }

        private static void countDistinctWords(List<string> words)
        {
            Console.Clear();
            int numDistinctWords = (from x in words select x).Distinct().Count();
            Console.WriteLine("The number of distinct words is: {0}", numDistinctWords);
            Console.WriteLine();
        }

        private static void firstTenWords(List<string> words)
        {
            Console.Clear();
            var take = words.Take(10).ToList();
            foreach (var word in take)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine();
        }

        private static void startsWithJCount(List<string> words)
        {
            Console.Clear();
            //int count = (from x in words where x.StartsWith("j")).Select().Count();
            var query = from x in words where x.StartsWith("j") select x;
            int count = 0;
            foreach(var word in query)
            {
                Console.WriteLine(word);
                count++;
            }
            Console.WriteLine("The number of words that start with 'j': {0}", count);
            Console.WriteLine();
        }

        private static void endsWithDCount(List<string> words)
        {
            Console.Clear();
            var query = from x in words where x.EndsWith("d") select x;
            int count = 0;
            foreach (var word in query)
            {
                Console.WriteLine(word);
                count++;
            }
            Console.WriteLine("The number of words that end with 'd': {0}", count);
            Console.WriteLine();
        }

        private static void greaterThanFourChars(List<string> words)
        {
            Console.Clear();
            var query = from x in words where x.Length > 4 select x;
            int count = 0;
            foreach (var word in query)
            {
                Console.WriteLine(word);
                count++;
            }
            Console.WriteLine("The number of words longer than 4 characters: {0}", count);
            Console.WriteLine();
        }

        private static void lessThanThreeChars(List<string> words)
        {
            Console.Clear();
            var query = from x in words where x.Length < 3 && x.StartsWith("a") select x;
            int count = 0;
            foreach (var word in query)
            {
                Console.WriteLine(word);
                count++;
            }
            Console.WriteLine("The number of words less than 3 characters and starts with 'a': {0}", count);
            Console.WriteLine();
        }
    }
}
