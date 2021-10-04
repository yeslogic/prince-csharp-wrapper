using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PrinceXML.Wrapper.Util
{
    internal class CommandLine
    {
        public static string ToCommand(string key, object value = null) =>
            value == null ? $"--{key}" : $"--{key}={value}";

        public static string ToCommandCsvs<T>(string key, IEnumerable<T> values)
        {
            string csvs = string.Join(",", values);
            return ToCommand(key, csvs);
        }

        public static List<string> ToCommands<T>(string key, IEnumerable<T> values)
        {
            List<string> repeatingCommands = new List<string>();
            foreach (T v in values)
            {
                repeatingCommands.Add(ToCommand(key, v));
            }
            return repeatingCommands;
        }
    }

    internal class Json
    {
        private StringBuilder _builder;
        private bool _comma;

        public Json()
        {
            _builder = new StringBuilder();
            _comma = false;
        }

        public Json BeginObj(string name = null)
        {
            MaybeAppendComma();
            if (name != null)
            {
                _builder.Append(QuoteAndEscape(name));
                _builder.Append(':');
            }
            _builder.Append('{');
            _comma = false;
            return this;
        }

        public Json EndObj()
        {
            _builder.Append('}');
            _comma = true;
            return this;
        }

        public Json BeginList(string name)
        {
            MaybeAppendComma();
            _builder.Append(QuoteAndEscape(name));
            _builder.Append(':');
            _builder.Append('[');
            _comma = false;
            return this;
        }

        public Json EndList()
        {
            _builder.Append(']');
            _comma = true;
            return this;
        }

        public Json Field(string name)
        {
            MaybeAppendComma();
            _builder.Append(QuoteAndEscape(name));
            _builder.Append(':');
            _comma = false;
            return this;
        }

        public Json Field(string name, string value) => Field(name).Value(value);

        public Json Field(string name, int value) => Field(name).Value(value);

        public Json Field(string name, bool value) => Field(name).Value(value);

        public Json Value(string value)
        {
            MaybeAppendComma();
            _builder.Append(QuoteAndEscape(value));
            _comma = true;
            return this;
        }

        public Json Value(int value)
        {
            MaybeAppendComma();
            _builder.Append(value.ToString());
            _comma = true;
            return this;
        }

        public Json Value(bool value)
        {
            MaybeAppendComma();
            _builder.Append(value ? "true" : "false");
            _comma = true;
            return this;
        }

        public override string ToString() => _builder.ToString();

        private string QuoteAndEscape(string s)
        {
            string escaped = s.Replace("\\", "\\\\").Replace("\"", "\\\"");
            return $"\"{escaped}\"";
        }

        private void MaybeAppendComma()
        {
            if (_comma) { _builder.Append(','); }
        }
    }

    internal class Chunk
    {
        public string Tag { get; }
        public byte[] Bytes { get; }

        private Chunk(string tag, byte[] bytes) => (Tag, Bytes) = (tag, bytes);

        public override string ToString() => Encoding.UTF8.GetString(Bytes);

        public static Chunk ReadChunk(Stream input)
        {
            byte[] tagBytes = new byte[3];
            if (!ReadBytes(input, tagBytes))
            {
                throw new IOException("Failed to read chunk tag.");
            }
            string tag = Encoding.ASCII.GetString(tagBytes);

            if (input.ReadByte() != ' ')
            {
                throw new IOException("Expected space after chunk tag.");
            }

            const int maxNumLength = 9;
            int length = 0;
            int numLength = 0;

            for (; numLength < maxNumLength + 1; numLength++)
            {
                int b = input.ReadByte();

                if (b == '\n') { break; }
                if (b < '0' || b > '9')
                {
                    throw new IOException("Unexpected character in chunk length.");
                }

                length *= 10;
                length += b - '0';
            }

            if (numLength < 1 || numLength > maxNumLength)
            {
                throw new IOException("Invalid chunk length.");
            }

            byte[] dataBytes = new byte[length];
            if (!ReadBytes(input, dataBytes))
            {
                throw new IOException("Failed to read chunk data.");
            }

            if (input.ReadByte() != '\n')
            {
                throw new IOException("Expected newline after chunk data.");
            }

            return new Chunk(tag, dataBytes);
        }

        public static void WriteChunk(Stream output, string tag, string data)
        {
            WriteChunk(output, tag, Encoding.UTF8.GetBytes(data));
        }

        public static void WriteChunk(Stream output, string tag, byte[] data)
        {
            string s = tag + " " + data.Length + "\n";
            byte[] b = Encoding.UTF8.GetBytes(s);

            output.Write(b, 0, b.Length);
            output.Write(data, 0, data.Length);
            output.WriteByte(((byte) '\n'));
        }

        private static bool ReadBytes(Stream input, byte[] buffer)
        {
            int length = buffer.Length;
            int offset = 0;

            while (length > 0)
            {
                int count = input.Read(buffer, offset, length);

                if (count < 0)
                {
                    return false;
                }
                if (count > length)
                {
                    throw new IOException("Unexpected read overrun.");
                }

                length -= count;
                offset += count;
            }

            return true;
        }
    }

    /* System.Diagnostics.Process in .NET Standard 2.0 does not have an API that
       automatically escapes provided arguments.

       So, we provide this utility class that copies .NET 5.0 functionality. See:
       https://github.com/dotnet/runtime/blob/80f015da0fe4669b9ef0141cbdcf918d32441b43/src/libraries/System.Diagnostics.Process/src/System/Diagnostics/ProcessStartInfo.cs
       https://github.com/dotnet/runtime/blob/80f015da0fe4669b9ef0141cbdcf918d32441b43/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs
    */
    internal class Arguments
    {
        private const char Quote = '\"';
        private const char Backslash = '\\';

        public static string BuildArguments(List<string> argumentList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendArgumentsTo(stringBuilder, argumentList);
            return stringBuilder.ToString();
        }

        private static void AppendArgumentsTo(StringBuilder stringBuilder, List<string> argumentList)
        {
            if (argumentList != null && argumentList.Count > 0)
            {
                foreach (string argument in argumentList)
                {
                    AppendArgument(stringBuilder, argument);
                }
            }
        }

        private static void AppendArgument(StringBuilder stringBuilder, string argument)
        {
            if (stringBuilder.Length != 0)
            {
                stringBuilder.Append(' ');
            }

            if (argument.Length != 0 && ContainsNoWhitespaceOrQuotes(argument))
            {
                stringBuilder.Append(argument);
            }
            else
            {
                stringBuilder.Append(Quote);
                int idx = 0;
                while (idx < argument.Length)
                {
                    char c = argument[idx++];
                    if (c == Backslash)
                    {
                        int numBackSlash = 1;
                        while (idx < argument.Length && argument[idx] == Backslash)
                        {
                            idx++;
                            numBackSlash++;
                        }

                        if (idx == argument.Length)
                        {
                            stringBuilder.Append(Backslash, numBackSlash * 2);
                        }
                        else if (argument[idx] == Quote)
                        {
                            stringBuilder.Append(Backslash, numBackSlash * 2 + 1);
                            stringBuilder.Append(Quote);
                            idx++;
                        }
                        else
                        {
                            stringBuilder.Append(Backslash, numBackSlash);
                        }

                        continue;
                    }

                    if (c == Quote)
                    {
                        stringBuilder.Append(Backslash);
                        stringBuilder.Append(Quote);
                        continue;
                    }

                    stringBuilder.Append(c);
                }

                stringBuilder.Append(Quote);
            }
        }

        private static bool ContainsNoWhitespaceOrQuotes(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c) || c == Quote)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
