using System;
using System.Linq;

namespace MainLibrary.Tests
{
    public static class PostConverter
    {
        public static string Convert(string text, string prefix = null)
        {
            prefix = prefix ?? string.Empty;

            var boundary = text.Split(Environment.NewLine.ToCharArray()).FirstOrDefault() ?? string.Empty;
            var objects = text.Split(boundary, StringSplitOptions.RemoveEmptyEntries);
            objects = objects.Take(objects.Length - 1).ToArray();
            var pairs = objects.Select(i =>
            {
                var header = i.TrimStart().Split(Environment.NewLine.ToCharArray()).FirstOrDefault() ?? string.Empty;

                // Content-Disposition: form-data; name="d2l_action"
                var index1 = header.IndexOf('"', StringComparison.Ordinal) + 1;
                var index2 = header.IndexOf('"', index1);
                var name = header.Substring(index1, index2 - index1);

                var value = i.Replace(header, string.Empty).Trim();

                return (name, value);
            });
            
            return string.Join(Environment.NewLine, pairs
                .Select(pair => $"{prefix}{{ new StringContent(@\"{pair.Item2}\"), \"{pair.Item1}\" }},"))
                .TrimEnd(',');
        }
    }
}
