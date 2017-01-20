using System;
using System.Text.RegularExpressions;

namespace Steward.Helper
{
    public static class MessageHelper
    {
        public static string StripHtml(string text)
        {
            text = ReplaceHyperlinks(text);
            text = ReplaceLineBreaks(text);
            text = ReplaceParagraphs(text);

            text = Regex.Replace(text, @"<[^>]+>|&rsquo;", "").Trim();
            text = Regex.Replace(text, @"&.*?;", "").Trim();

            return text;
        }

        private static string ReplaceParagraphs(string text)
        {
            text = text.Replace("</p>", Environment.NewLine + Environment.NewLine);
            return text;
        }

        private static string ReplaceLineBreaks(string text)
        {
            text = text.Replace("<br>", Environment.NewLine);
            return text;
        }

        private static string ReplaceHyperlinks(string text)
        {
            text = text.Replace("\"", "'");
            try
            {
                // REPLACE HYPERLINKS
                const string pat = @"<a(.)+<\/a>";
                var r = new Regex(pat, RegexOptions.IgnoreCase);
                var m = r.Match(text);
                if (m.Success)
                {
                    var value = m.Value;
                    var replacement = Replacehyperlink(value);

                    text = text.Replace(value, replacement);
                }
            }
            catch (Exception)
            {
                // TODO: Log Error
            }

            return text;
        }

        private static string Replacehyperlink(string hyperlinktext)
        {
            var label = "";
            var linkTextminusHref = "";


            var pat = @"href='[^']+'";
            var r = new Regex(pat, RegexOptions.IgnoreCase);
            var m = r.Match(hyperlinktext);
            if (m.Success)
            {
                var g = m.Groups[0];
                var cc = g.Captures;
                var c = cc[0];
                var linktext = c.Value;

                linkTextminusHref = linktext.Replace("href='", "").Replace("'", "");
            }

            pat = @"(?<=<a href='[^']+'>)(.*)(?=<\/a\>)";
            r = new Regex(pat, RegexOptions.IgnoreCase);
            m = r.Match(hyperlinktext);
            if (m.Success)
            {
                var g = m.Groups[0];
                var cc = g.Captures;
                var c = cc[0];
                label = c.Value;
            }

            var newHrefText = "[" + label + "](" + linkTextminusHref + ")";
            return newHrefText;
        }
    }
}