using System.Text;

namespace PrinceXML.Wrapper.Util
{
    internal class Json
    {
        private StringBuilder _builder;
        private bool _comma;

        public Json()
        {
            _builder = new StringBuilder();
            _comma = false;
        }

        public Json BeginObj(string? name = null)
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

        public Json Field(string name, string value)
        {
            return Field(name).Value(value);
        }

        public Json Field(string name, int value)
        {
            return Field(name).Value(value);
        }

        public Json Field(string name, bool value)
        {
            return Field(name).Value(value);
        }

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

        public override string ToString()
        {
            return _builder.ToString();
        }

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
}
