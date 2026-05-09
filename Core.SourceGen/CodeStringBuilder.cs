using System.Text;

namespace FEZRepacker.Core.SourceGen
{
    public class CodeStringBuilder
    {
        private readonly StringBuilder _stringBuilder;
        private int _currentIndent;
        private bool _shouldIndent = false;
        
        public CodeStringBuilder()
        {
            _stringBuilder = new StringBuilder();
            _currentIndent = 0;
        }

        public void Append(string s)
        {
            if (_shouldIndent)
            {
                _stringBuilder.Append(new string(' ', _currentIndent * 4));
                _shouldIndent = false;
            }
            _stringBuilder.Append(s);
        }

        public void AppendLine()
        {
            _stringBuilder.AppendLine();
            _shouldIndent = true;
        }

        public void AppendLine(string s)
        {
            Append(s);
            AppendLine();
        }

        public void BeginCodeBlock(string openCharacter = "{")
        {
            AppendLine(openCharacter);
            _currentIndent++;
        }

        public void EndCodeBlock(string closeCharacter = "}")
        {
            if(_currentIndent > 0) _currentIndent--;
            AppendLine(closeCharacter);
        }
        
        public override string ToString() => _stringBuilder.ToString();
    }
}
