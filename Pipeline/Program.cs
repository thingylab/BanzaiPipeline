using System;

namespace Pipeline
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            var pipeline = Pipeline<string, int, string>
                .New()
                .WithStage(CountCharacters)
                .Finally(MultiplyByTwo);

            Console.WriteLine(pipeline);
		}

        public static int CountCharacters(string s)
        {
            return s.Length;
        }

        public static int MultiplyByTwo(int i)
        {
            return i * 2;
        }
	}
}
