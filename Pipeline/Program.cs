using System;
using System.Collections.Generic;
using System.Threading;

namespace Pipeline
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		    var source = new CancellationTokenSource();

            var pipeline = Pipeline<string, int, MyStatus>
                .New()
                .WithStage(ProvideStrings)
                .WithStage(CountCharacters)
                .Finally(MultiplyByTwo);

            pipeline.Run(source.Token);

		    Console.ReadLine();
		}

	    public class MyStatus
	    {
            public string Blah { get; set; }
	    }

	    public static IEnumerable<string> ProvideStrings()
	    {
	        var rnd = new Random();

	        for (int i = 0; i < 1000000; i++)
	        {
	            var guid = Guid.NewGuid().ToString();
	            var str = guid.Substring(rnd.Next(guid.Length));
                Console.WriteLine("String: {0}", str);

	            yield return str;
	        }
	    }

        public static int CountCharacters(string s)
        {
            var l = s.Length;

            Console.WriteLine("Length: {0}", l);

            return l;
        }

        public static int MultiplyByTwo(int i)
        {
            Console.WriteLine("Multiplied: {0}", i*2);

            return i * 2;
        }
	}
}
