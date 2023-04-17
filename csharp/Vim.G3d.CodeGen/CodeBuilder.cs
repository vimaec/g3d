using System.Linq;
using System.Text;

namespace Vim.G3d.CodeGen
{
    public class CodeBuilder
    {
        private int _indentCount;
        private StringBuilder _sb = new StringBuilder();

        public CodeBuilder AppendRaw(string line)
        {
            _sb.Append(new string(' ', _indentCount * 4));
            _sb.AppendLine(line);
            return this;
        }

        public CodeBuilder AppendLine(string line = "")
        {
            var openBraces = line.Count(c => c == '{');
            var closeBraces = line.Count(c => c == '}');

            // Sometimes we have {} on the same line
            if (openBraces == closeBraces)
            {
                openBraces = 0;
                closeBraces = 0;
            }

            _indentCount -= closeBraces;
            _sb.Append(new string(' ', _indentCount * 4));
            _sb.AppendLine(line);
            _indentCount += openBraces;
            return this;
        }

        public void Indent()
        {
            ++_indentCount;
        }

        public void Unindent()
        {
            _indentCount = System.Math.Max(0, --_indentCount);
        }

        public void IndentOneLine(string line)
        {
            Indent();
            AppendLine(line);
            Unindent();
        }

        public void UnindentOneLine(string line)
        {
            Unindent();
            AppendLine(line);
            Indent();
        }

        public override string ToString()
            => _sb.ToString();
    }
}
