using System;
using System.Collections.Generic;
using System.Linq;

namespace Turune.Models.Utils
{
    public class IniFile
    {
        private readonly char[] _commentChars;
        private readonly List<string> _sectionOrder = new(); //Ordered list of sections (preserves original order on serialization)
        private readonly Dictionary<string, List<string>> _keyOrder     // Ordered list of keys
            = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Dictionary<string, string>> _data
            = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates an empty INI document.
        /// Use <see cref="Parse"/> to load lines into it.
        /// </summary>
        /// <param name="commentChars">
        /// Characters treated as comment starters. Defaults to ';' and '#'.
        /// </param>
        public IniFile(char[]? commentChars = null)
        {
            _commentChars = commentChars ?? new[] { ';', '#' };
        }

        /// <summary>
        /// Parses an array of lines into this document. If called multiple times, it will merge additional lines.
        /// </summary>
        public void Parse(string[] lines)
        {
            string currentSection = "";

            foreach (string rawLine in lines)
            {
                string line = StripComment(rawLine.TrimEnd('\r')).Trim();

                if (string.IsNullOrEmpty(line)) continue;

                //Section header: [SectionName]
                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    currentSection = line[1..^1].Trim();
                    EnsureSection(currentSection);
                    continue;
                }

                //Key=Value
                int eq = line.IndexOf('=');
                if (eq < 0) continue;

                string key = line[..eq].Trim();
                string value = Unquote(line[(eq + 1)..].Trim());

                if (string.IsNullOrEmpty(key)) continue;

                EnsureSection(currentSection);

                if (!_data[currentSection].ContainsKey(key))
                    _keyOrder[currentSection].Add(key);

                _data[currentSection][key] = value;
            }
        }

        
        //Readers

        /// <summary>Returns the value for a key, or <paramref name="defaultValue"/> if not found.</summary>
        public string Read(string section, string key, string defaultValue = "") =>
            _data.TryGetValue(section, out var keys) && keys.TryGetValue(key, out var value)
                ? value
                : defaultValue;

        /// <summary>Returns true if the section exists.</summary>
        public bool SectionExists(string section) => _data.ContainsKey(section);

        /// <summary>Returns true if the key exists within the section.</summary>
        public bool KeyExists(string section, string key) =>
            _data.TryGetValue(section, out var keys) && keys.ContainsKey(key);

        /// <summary>Returns all section names in their original order.</summary>
        public IReadOnlyList<string> GetSections() => _sectionOrder.AsReadOnly();

        /// <summary>Returns all keys in a section in their original order.</summary>
        public IReadOnlyList<string> GetKeys(string section) =>
            _keyOrder.TryGetValue(section, out var keys)
                ? keys.AsReadOnly()
                : Array.Empty<string>();


        //Writers

        /// <summary>Sets a value. Creates the section if it does not exist.</summary>
        public void Write(string section, string key, string value)
        {
            EnsureSection(section);

            if (!_data[section].ContainsKey(key))
                _keyOrder[section].Add(key);

            _data[section][key] = value;
        }

        /// <summary>Deletes a key.</summary>
        /// <returns>True if the key was deleted.</returns>
        public bool DeleteKey(string section, string key)
        {
            if (!_data.TryGetValue(section, out var keys)) return false;
            if (!keys.Remove(key)) return false;
            _keyOrder[section].Remove(key);
            return true;
        }

        /// <summary>Deletes an entire section.</summary>
        /// <returns>True if the section was deleted.</returns>
        public bool DeleteSection(string section)
        {
            if (!_data.Remove(section)) return false;
            _keyOrder.Remove(section);
            _sectionOrder.Remove(section);
            return true;
        }


        /// <summary>
        /// Serializes the document to a string array.
        /// </summary>
        public string[] Create(bool includeEmptySections = false)
        {
            var lines = new List<string>();

            for (int s = 0; s < _sectionOrder.Count; s++)
            {
                string section = _sectionOrder[s];
                if (!includeEmptySections && _keyOrder[section].Count == 0) continue;
                if (s > 0) lines.Add(string.Empty);

                lines.Add($"[{section}]");

                foreach (string key in _keyOrder[section])
                {
                    string value = _data[section][key];
                    if (NeedsQuoting(value)) value = $"\"{value}\"";
                    lines.Add($"{key}={value}");
                }
            }

            return lines.ToArray();
        }


        //Helpers
        private string StripComment(string line)
        {
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"') { inQuotes = !inQuotes; continue; }

                if (!inQuotes && _commentChars.Contains(c))
                    return line[..i];
            }

            return line;
        }

        private void EnsureSection(string section)
        {
            if (_data.ContainsKey(section)) return;
            _sectionOrder.Add(section);
            _keyOrder[section] = new List<string>();
            _data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static string Unquote(string value) =>
            value.Length >= 2 && value[0] == '"' && value[^1] == '"'
                ? value[1..^1]
                : value;

        private bool NeedsQuoting(string value) =>
            value.Length > 0 &&
            (value != value.Trim() || value.IndexOfAny(_commentChars) >= 0);
    }
}
