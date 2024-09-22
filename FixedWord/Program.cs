namespace FixedWord
{
    internal class Program
    {
        static Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            {"getmək", "getmək"},
            {"gəlmək", "gəlmək"},
            {"gedəcək", "gedəcək"},
            {"gedir", "gedir"},
            {"getdim", "getdim"},
            {"gəldim", "gəldim"},
            {"gedirəm", "gedirəm"},
            {"gəlirəm", "gəlirəm"}
        };

        static HashSet<string> recentlyLearnedWords = new HashSet<string>();
        static TimeSpan suggestionCooldown = TimeSpan.FromSeconds(10); 
        static Dictionary<string, DateTime> wordLearnedTimes = new Dictionary<string, DateTime>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Bir söz girin (çixmag üçün 'exit' yazin):");
                string input = Console.ReadLine();

                if (input.ToLower() == "exit") break;

                string suggestion = FindBestMatch(input);

                if (recentlyLearnedWords.Contains(suggestion) && wordLearnedTimes.ContainsKey(suggestion))
                {
                    DateTime learnedTime = wordLearnedTimes[suggestion];
                    if ((DateTime.Now - learnedTime) < suggestionCooldown)
                    {
                        Console.WriteLine($"Bu sözü yeni öyrendim: {suggestion}");
                        UpdateLearnedWord(suggestion, input);
                        continue;
                    }
                }

                

                if (suggestion == null)
                {
                    string closestWord = FindClosestWord(input);
                    if (closestWord != null)
                    {
                        Console.WriteLine($"'{input}' kelimesi '{closestWord}' kelimesine çox yaxin. Güncelleyirem...");
                        UpdateLearnedWord(closestWord, input); 
                    }
                    else
                    {
                        Console.WriteLine($"Bu sözü bilmirem. Sözü lügete elave eliyirem: {input}");
                        
                        LearnNewWord(input);
                    }
                }
                else
                {
                    Console.WriteLine($"Önerilen düzeltme: {suggestion}");
                }
            }
        }

        static int DamerauLevenshteinDistance(string s1, string s2)
        {
            int len1 = s1.Length;
            int len2 = s2.Length;
            int[,] dp = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++) dp[i, 0] = i;
            for (int j = 0; j <= len2; j++) dp[0, j] = j;

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);

                    if (i > 1 && j > 1 && s1[i - 1] == s2[j - 2] && s1[i - 2] == s2[j - 1])
                    {
                        dp[i, j] = Math.Min(dp[i, j], dp[i - 2, j - 2] + 1);
                    }
                }
            }

            return dp[len1, len2];
        }

        static string FindBestMatch(string input)
        {
            string bestMatch = null;
            int minDistance = int.MaxValue;

            foreach (var entry in dictionary)
            {
                string word = entry.Key;
                int distance = DamerauLevenshteinDistance(input, word);

                int threshold = Math.Max(2, input.Length / 2); 
                if (distance < minDistance && distance <= threshold)
                {
                    minDistance = distance;
                    bestMatch = entry.Value;
                }
            }

            return bestMatch;
        }

        static string FindClosestWord(string input)
        {
            string closestWord = null;
            int minDistance = int.MaxValue;

            foreach (var entry in dictionary)
            {
                string word = entry.Key;
                int distance = DamerauLevenshteinDistance(input, word);

                int threshold = Math.Max(2, input.Length / 2); 
                if (distance < minDistance && distance <= threshold)
                {
                    minDistance = distance;
                    closestWord = word;
                }
            }

            return closestWord;
        }

        static void LearnNewWord(string input)
        {
            if (!dictionary.ContainsKey(input))
            {
                dictionary.Add(input, input);
                recentlyLearnedWords.Add(input); 
                wordLearnedTimes[input] = DateTime.Now; 
            }
            else
            {
                Console.WriteLine("Bu kelime zaten lügetde var.");
            }
        }

        static void UpdateLearnedWord(string oldWord, string newWord)
        {
            if (dictionary.ContainsKey(oldWord))
            {
                dictionary.Remove(oldWord);
                dictionary.Add(newWord, newWord);
                recentlyLearnedWords.Add(newWord); 
                wordLearnedTimes[newWord] = DateTime.Now; 
                Console.WriteLine($"'{oldWord}' kelimesi güncellendi: '{newWord}' olarak değiştirildi.");
            }
        }
    }
}
