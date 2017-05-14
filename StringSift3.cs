// define V31 to make it even faster, but less reliable
//#define V31
using System;

namespace SiftStringSimilarity
{
    // Usage example:
    // var ss3=new StringSift3();
    // var distance=ss3.Distance(s1, s2);
    // var similarity=ss3.Similarity(s1, s2);

    /// <summary>
    /// Computes the distance and similarity between two strings
    /// a lot faster than Levenshtein
    /// </summary>
    public class StringSift3
    {
        /// <summary>
        /// describes the distance between characters at which it is
        /// cheaper to replace a character rather than move it
        /// </summary>
        private readonly int _maxOffset;

        /// <summary>
        /// Instantiate the class with a default value of MaxOffset=5
        ///  </summary>
        public StringSift3() : this(5)
        {
        }

        /// <summary>
        /// MaxOffset represents the maximum range the algorithm searches for the same character
        /// It is cheaper to replace a character rather than move it from a distance larger than MaxOffset.
        /// </summary>
        /// <param name="maxOffset"></param>
        public StringSift3(int maxOffset)
        {
            _maxOffset = maxOffset;
        }

        /// <summary>
        /// Calculate a distance similar to Levenstein, but faster and less reliable.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public float Distance(string s1, string s2)
        {
            if (String.IsNullOrEmpty(s1))
                return
                    String.IsNullOrEmpty(s2) ? 0 : s2.Length;
            if (String.IsNullOrEmpty(s2))
                return s1.Length;
            int c = 0;
            int offset1 = 0;
            int offset2 = 0;
            int lcs = 0;
            while ((c + offset1 < s1.Length)
                   && (c + offset2 < s2.Length))
            {
                if (s1[c + offset1] == s2[c + offset2]) lcs++;
                else
                {
#if V31
                    c += (offset1 + offset2)/2;
                    if (c >= s1.Length) c = s1.Length - 1;
                    if (c >= s2.Length) c = s2.Length - 1;
#endif
                    offset1 = 0;
                    offset2 = 0;
                    if (s1[c] == s2[c])
                    {
                        c++;
                        continue;
                    }
                    for (int i = 1; i < _maxOffset; i++)
                    {
                        if ((c + i < s1.Length)
                            && (s1[c + i] == s2[c]))
                        {
                            offset1 = i;
                            break;
                        }
                        if ((c + i < s2.Length)
                            && (s1[c] == s2[c + i]))
                        {
                            offset2 = i;
                            break;
                        }
                    }
                }
                c++;
            }
            return (s1.Length + s2.Length)/2 - lcs;
        }

        /// <summary>
        /// Calculate the similarity of two strings, as a percentage.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public float Similarity(string s1, string s2)
        {
            float dis = Distance(s1, s2);
            float maxLen = Math.Max(Math.Max(s1.Length, s2.Length), dis);
            if (maxLen == 0) return 1;
            return 1 - dis/maxLen;
        }
    }
}