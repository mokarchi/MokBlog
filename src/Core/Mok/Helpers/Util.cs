using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mok.Helpers
{
    /// <summary>
    /// Utility helpers.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Removes all html tags from content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string CleanHtml(string content)
        {
            if (content.IsNullOrEmpty()) return content;
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);
            return document.DocumentNode.InnerText;
        }

        /// <summary>
        /// Returns the slug of a given string. 
        /// </summary>
        /// <remarks>
        /// Produces optional, URL-friendly version of a title, "like-this-one",
        /// </remarks>
        public static string Slugify(string title, int maxlen = 250, int randomCharCountOnEmpty = 0)
        {
            if (title == null)
                return randomCharCountOnEmpty <= 0 ? "" : Util.RandomString(randomCharCountOnEmpty);

            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase            
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    { sb.Append('-'); prevdash = true; }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen - 1) break;
            }

            var slug = prevdash ? sb.ToString().Substring(0, sb.Length - 1) : sb.ToString();
            if (slug == string.Empty && randomCharCountOnEmpty > 0) slug = Util.RandomString(randomCharCountOnEmpty);

            return slug;
        }

        /// <summary>
        /// Returns a random lowercase alpha + numeric chars of a certain length.
        /// </summary>
        public static string RandomString(int length)
        {
            Random random = new Random();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Repeat(0, length) // Range would give the same end result
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        /// <summary>
        /// Returns an ascii char for an international char.
        /// </summary>
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåa".Contains(s))
            {
                return "a";
            }
            else if ("èéêëe".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïi".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøo".Contains(s))
            {
                return "o";
            }
            else if ("ùúûü".Contains(s))
            {
                return "u";
            }
            else if ("çcc".Contains(s))
            {
                return "c";
            }
            else if ("zzž".Contains(s))
            {
                return "z";
            }
            else if ("ssš".Contains(s))
            {
                return "s";
            }
            else if ("ñn".Contains(s))
            {
                return "n";
            }
            else if ("ýŸ".Contains(s))
            {
                return "y";
            }
            else if (c == 'l')
            {
                return "l";
            }
            else if (c == 'd')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'g')
            {
                return "g";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Returns a new slug by appending a counter to the given slug.  This method is called 
        /// when caller already determined the slug is a duplicate.
        /// </summary>
        public static string UniquefySlug(string slug, IEnumerable<string> existingSlugs)
        {
            if (slug.IsNullOrEmpty() || existingSlugs.IsNullOrEmpty()) return slug;

            int i = 2;
            while (existingSlugs.Contains(slug))
            {
                var lookup = $"-{i}";
                if (slug.EndsWith(lookup))
                {
                    var idx = slug.LastIndexOf(lookup);
                    slug = slug.Remove(idx, lookup.Length).Insert(idx, $"-{++i}");
                }
                else
                {
                    slug = $"{slug}-{i}";
                }
            }

            return slug;
        }

    }
}
