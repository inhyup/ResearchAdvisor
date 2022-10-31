﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace ResearchAdvisor.Domain.Rake
{
    internal static class StopListHelper
    {
        public static HashSet<string> ParseFromPath(string? stopWordsPath)
        {
            var stopWords = new HashSet<string>(StringComparer.Ordinal);

            foreach (var line in string.IsNullOrWhiteSpace(stopWordsPath)
                ? ReadDefaultStopListLine()
                : File.ReadAllLines(stopWordsPath))
            {
                ReadOnlySpan<char> normalizedLine = line.AsSpan().Trim();

                if (normalizedLine.Length == 0 || normalizedLine[0] == '#') continue;

                var splitter = new StringSplitter(normalizedLine, ' ');

                while (splitter.TryGetNext(out var word))
                {
                    stopWords.Add(word.ToString());
                }
            }

            return stopWords;
        }

        private static IEnumerable<string> ReadDefaultStopListLine()
        {
            var assembly = Assembly.GetExecutingAssembly();
            // var resourceName = "Rake.SmartStoplist.txt";

            using var stream =  File.Open("../ResearchAdvisor.Domain/Rake/SmartStoplist.txt", FileMode.Open);
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}
