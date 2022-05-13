using System.Text;

namespace FEZRepacker
{
    class PAKFile
    {
        public string Identifier { get; protected set; }
        public bool IsValid { get; protected set; }

        protected byte[] _content;
        public byte[] Content => _content;

        public PAKFile()
        {
            Identifier = "";
            IsValid = false;
            _content = new byte[0];
        }

        public void SetData(byte[] data)
        {
            _content = data;

            Validate();
        }

        public virtual void Validate()
        {
            IsValid = true;
        }

        public virtual string GetInfo()
        {
            string identifierName = "unknown";
            if (Identifier.All(Char.IsLetter))
            {
                identifierName = Identifier;
            }

            return $"{identifierName} file; size: {_content.Length}B";
        }

        public virtual int GetSize()
        {
            return _content.Length;
        }

        public virtual string GetExtension()
        {
            string ext = "unk";
            if (Identifier.All(Char.IsLetter))
            {
                ext = Identifier.ToLower();
            }
            return ext;
        }

        public static PAKFile FromData(byte[] data)
        {
            PAKFile content = new PAKFile();

            content.Identifier = GetIdentifierFromData(data);
            content.SetData(data);

            return content;
        }

        public static string GetIdentifierFromData(byte[] data)
        {
            if (data == null || data.Length < 3) return "";
            return Encoding.UTF8.GetString(data.Take(3).ToArray());
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(_content);
        }
    }
}
