using System;
using System.Collections.Generic;
using System.Linq;
using static decoding_information_theory.Dictionaries;

namespace decoding_information_theory
{
    class Program
    {
        static string firstLine = "110001100111001010010101001010000100001100001000001110001001100001111000110011000100001000";
        static string secondLine = "110011101000110101010100111000101100110000101010101100100010011111101110010010100010011010111011011100001011000100011011111000";
        static string thirdLine = "И НАКОНЕЦ УВИДЕЛ СВЕТ. ОН ПО-ФРАНЦУЗСКИ СОВЕРШЕННО";
        static void Main(string[] args)
        {

            Console.WriteLine("Фраза А - " + decodeFirstLine() + "\n");
            Console.WriteLine("Фраза B - " + decodeSecondLine() + "\n");
            List<char> symbols = new List<char>();
            List<int> chances = new List<int>();
            getTable(ref symbols, ref chances);
            Dictionary<string, string> newUnevenTable = new Dictionary<string, string>();
            Dictionary<string, string> newPhanosTable = new Dictionary<string, string>();
            newUnevenTable = getUnevenTable(symbols);
            newPhanosTable = getPhanosTable(symbols, chances);
            Console.WriteLine("Неравномерный код");
            foreach (KeyValuePair<string, string> a in newUnevenTable) {
                Console.WriteLine(a.Key + " " + a.Value);
            }
            Console.WriteLine("\nКод Шенонна-Фано ");
            foreach (KeyValuePair<string, string> a in newPhanosTable)
            {
                Console.WriteLine(a.Key + " " + a.Value);
            }
            List<double> chance = new List<double>();
            foreach (int i in chances) {
                chance.Add((double)i / thirdLine.Length);
            }
            Console.WriteLine("Энтропия " + getEntropy(chance));
            Console.WriteLine("Среднее число элементарных символов на букву при неравномерном кодировании " + getAveragePerCharacter(newUnevenTable, chance));
            Console.WriteLine("Среднее число элементарных символов на букву при неравномерном кодировании " + getAveragePerCharacter(newPhanosTable, chance));
            Console.WriteLine("Средняя информация на один двоичный символ при неравномерном кодировании " + getEntropy(chance) / getAveragePerCharacter(newUnevenTable, chance));
            Console.WriteLine("Средняя информация на один двоичный символ при кодировании методом Шеннона-Фано " + getEntropy(chance) / getAveragePerCharacter(newPhanosTable, chance));
            Console.ReadLine();
        }
        public static string decodeFirstLine()
        {
            string ans = "";
            string charCode = "";
            foreach(char c in firstLine)
            {               
                if (c == '1' && charCode.Contains("00")) {
                    string symbol = "";
                    if (charCode.Contains("00000"))
                    {
                        charCode = charCode.Remove(charCode.Length - 3);
                        unevenCode.TryGetValue(charCode, out symbol);
                        charCode = c.ToString();
                        ans += symbol + " ";
                        continue;
                    }
                    unevenCode.TryGetValue(charCode,out symbol);
                    charCode = c.ToString();
                    ans += symbol;
                    continue;
                }

                charCode += c;
            }
            string lastsymbol = "";
            unevenCode.TryGetValue(charCode, out lastsymbol);
            ans += lastsymbol;
            return ans;
        }
        public static string decodeSecondLine()
        {
            string ans = "";
            string charCode = "";
            foreach (char c in secondLine)
            {
                charCode += c.ToString();
                if (phanosCode.ContainsKey(charCode))
                {
                    string symbol = "";
                    phanosCode.TryGetValue(charCode, out symbol);
                    ans += symbol;
                    charCode = "";
                }

            }
            return ans;
        }

        public static void getTable(ref List<char> symbols, ref List<int> occasions) {

            foreach (char ch in thirdLine) {
                if (!symbols.Contains(ch))
                {
                    symbols.Add(ch);
                    occasions.Add((thirdLine.Length - thirdLine.Replace(ch.ToString(), "").Length));
                }
            }
            sort(ref symbols, ref occasions);
        }

        public static void sort(ref List<char> symbols, ref List<int> chances) {
            for(int i = 0; i < chances.Count - 1; i++)
            {
                int max = 0;
                int ind = 0;
                for(int j = i; j < chances.Count; j++)
                {
                    if (chances[j] > max)
                    {
                        max = chances[j];
                        ind = j;
                    }
                }
                int temp = chances[i];
                chances[i] = max;
                chances[ind] = temp;

                char tempch = symbols[i];
                symbols[i] = symbols[ind];
                symbols[ind] = tempch;
            }
        }

        public static Dictionary<string, string> getUnevenTable(List<char> symbols) {
            Dictionary<string, string> ans = new Dictionary<string, string>();
            int counter = 0;
            foreach(char ch in symbols)
            {
                ans.Add(ch.ToString(), unevenCode.ElementAt(counter).Key);
                counter++;
            }
            return ans;
        }

        public static Dictionary<string, string> getPhanosTable(List<char> symbols, List<int> occasions)
        {
            Dictionary<string, string> ans = new Dictionary<string, string>();
            string[] codes = new string[symbols.Count];
            for(int i = 0; i < codes.Length; i++)
            {
                codes[i] = "";
            }
            splitAndFill(0, occasions.Count - 1, ref codes, occasions);
            for (int i = 0; i < codes.Length; i++)
            {
                ans.Add(symbols[i].ToString(), codes[i]);
            }
            return ans;
        }

        public static void splitAndFill(int start, int end, ref string[] codes, List<int> occasions)
        {
            if(start < end) {
                if (end - start == 1) {
                    codes[start] += "0";
                    codes[end] += "1";
                    return;
                }
                int i = start;
                int m = end;
                int left = 0, right = occasions[end];
                for(int k = start; k < end; k++) {
                    left += occasions[k];
                }
                while (left > right) {
                    m--;
                    right += occasions[m];
                    left -= occasions[m];
                }
                

                for (int k = start; k <= end; k++)
                {
                    if(k < m) { 

                    codes[k] += "0";

                    } else
                    {
                        codes[k] += "1";
                    }
                }

                splitAndFill(start, m - 1, ref codes, occasions);
                splitAndFill(m, end, ref codes, occasions);
            }
        }

        public static double getEntropy(List<double> chances)
        {
            double ans = 0;
            foreach(double chance in chances)
            {
                ans -= chance * Math.Log2(chance);
            }
            return ans;
        }
        public static double getAveragePerCharacter(Dictionary<string, string> table, List<double> chances)
        {
            double ans = 0;
            int i = 0;
            foreach (KeyValuePair<string, string> pair in table)
            {
                ans += chances[i] * pair.Value.Length;
                i++;
            }
            return ans;
        }
    }
}
