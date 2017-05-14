using System;

namespace SiftStringSimilarity
{
    /// <summary>
    ///  Helper class for Sift3, Levenstein and length distance and similarities
    /// </summary>
    public static class StringMetrics
    {
        private static readonly StringSift3 _sift = new StringSift3();

        /// <summary>
        /// Fast compute the distance between two strings based on several levels of accuracy
        /// First length, then Sift, then Levenstein
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public static float FastDistance(string s1, string s2, int maxDistance)
        {
            int ld = LengthDistance(s1, s2);
            if (ld >= maxDistance) return ld;
            float sd = SiftDistance(s1, s2);
            if (sd >= maxDistance) return sd;
            return LevensteinDistance(s1, s2);
        }

        /// <summary>
        /// Fast compute the similarity between two strings based on several levels of accuracy
        /// First length, then Sift, then Levenstein
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="minSimilarity"></param>
        /// <returns></returns>
        public static float FastSimilarity(string s1, string s2, float minSimilarity)
        {
            float ld = LengthSimilarity(s1, s2);
            if (ld <= minSimilarity) return ld;
            float sd = SiftSimilarity(s1, s2);
            if (sd <= minSimilarity) return sd;
            return LevensteinSimilarity(s1, s2);
        }

        /// <summary>
        /// Calculate the distance between two strings by length alone
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static int LengthDistance(string s1, string s2)
        {
            int l1 = (s1 ?? "").Length;
            int l2 = (s2 ?? "").Length;
            return Math.Abs(l1 - l2);
        }

        /// <summary>
        /// Calculate the similarity between two strings by length alone
        /// </summary>
        /// <param name="string1"></param>
        /// <param name="string2"></param>
        /// <returns></returns>
        public static float LengthSimilarity(string string1, string string2)
        {
            float l1 = (string1 ?? "").Length;
            float l2 = (string2 ?? "").Length;
            if (l1 < l2) return l1/l2;
            if (l1 == 0) return 1;
            return l2/l1;
        }


        /// <summary>
        /// Calculate the Sift3 distance between 2 strings
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static float SiftDistance(string s1, string s2)
        {
            return _sift.Distance(s1, s2);
        }

        /// <summary>
        /// Calculate the Sift3 similarity between 2 strings
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static float SiftSimilarity(string s1, string s2)
        {
            return _sift.Similarity(s1, s2);
        }

        /// <summary>
        /// Calculate the Levenstein string distance
        /// </summary>
        /// <param name="s">first string</param>
        /// <param name="t">second string</param>
        /// <returns>integer distance</returns>
        public static int LevensteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            var distance = new int[n + 1,m + 1]; // matrix
            if (n == 0) return m;
            if (m == 0) return n;
            //init1
            for (int i = 0; i <= n; i++)
                distance[i, 0] = i;
            for (int j = 0; j <= m; j++)
                distance[0, j] = j;
            //find min distance
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t.Substring(j - 1, 1) ==
                                s.Substring(i - 1, 1)
                                    ? 0
                                    : 1);

                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1,
                                                       distance[i, j - 1] + 1),
                                              distance[i - 1, j - 1] + cost);
                }
            }
            return distance[n, m];
        }

        /// <summary>
        /// Calculate the percentage of Levenstein similarity between two strings
        /// </summary>
        /// <param name="string1">first string</param>
        /// <param name="string2">second string</param>
        /// <returns>float between 0 and 1</returns>
        public static float LevensteinSimilarity(string string1, string string2)
        {
            float dis = LevensteinDistance(string1, string2);
            int maxLen = Math.Max(string1.Length, string2.Length);
            if (maxLen == 0)
                return 1;
            return 1 - dis/maxLen;
        }
    }
}