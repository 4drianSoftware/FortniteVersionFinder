using System.Text;
namespace FortniteVersionFinder
{
    public class Finder
    {
        public static string GetVersion(string ClientPath)
        {
            byte[] SequenceToFind = { 0x46, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x74, 0x00, 0x6E, 0x00, 0x69, 0x00, 0x74, 0x00, 0x65, 0x00, 0x2B, 0x00, 0x52, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x65, 0x00, 0x61, 0x00, 0x73, 0x00, 0x65, 0x00 };
            byte[] FortniteClientBytes = File.ReadAllBytes(ClientPath);
            var searcher = new BoyerMoore(SequenceToFind);
            foreach (int index in searcher.Search(FortniteClientBytes))
            {
                byte[] VersionBytes = new byte[64];
                Array.Copy(FortniteClientBytes, index + 0x1C, VersionBytes, 0, 64);
                string Version = Encoding.Unicode.GetString(VersionBytes).PadLeft(3);
                string VersionNumber = CutStart(Version, "se-").Split('-')[0];
                bool isDouble = Double.TryParse(VersionNumber, out double b);
                if (isDouble)
                {
                    return VersionNumber;
                }
                else
                {
                    string rvStr = ReverseFunc(VersionNumber);
                    int cutAmnt = rvStr.Length - 5;
                    return ReverseFunc(rvStr.Substring(cutAmnt));
                }
            }
            return "Invalid version! May be 2.4.2 or below.";
        }
        public static string ReverseFunc(string str)
        {
            char[] ch = str.ToCharArray();
            Array.Reverse(ch);
            return new string(ch);
        }
        public static string CutStart(string s, string what)
        {
            if (s.StartsWith(what))
                return s.Substring(what.Length);
            else
                return s;
        }
    }
    public sealed class BoyerMoore
    {
        readonly byte[] needle;
        readonly int[] charTable;
        readonly int[] offsetTable;
        public BoyerMoore(byte[] needle)
        {
            this.needle = needle;
            this.charTable = makeByteTable(needle);
            this.offsetTable = makeOffsetTable(needle);
        }
        public IEnumerable<int> Search(byte[] haystack)
        {
            if (needle.Length == 0)
                yield break;
            for (int i = needle.Length - 1; i < haystack.Length;)
            {
                int j;
                for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
                {
                    if (j != 0)
                        continue;
                    yield return i;
                    i += needle.Length - 1;
                    break;
                }
                i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
            }
        }
        static int[] makeByteTable(byte[] needle)
        {
            const int ALPHABET_SIZE = 256;
            int[] table = new int[ALPHABET_SIZE];
            for (int i = 0; i < table.Length; ++i)
                table[i] = needle.Length;
            for (int i = 0; i < needle.Length - 1; ++i)
                table[needle[i]] = needle.Length - 1 - i;
            return table;
        }
        static int[] makeOffsetTable(byte[] needle)
        {
            int[] table = new int[needle.Length];
            int lastPrefixPosition = needle.Length;
            for (int i = needle.Length - 1; i >= 0; --i)
            {
                if (isPrefix(needle, i + 1))
                    lastPrefixPosition = i + 1;
                table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
            }
            for (int i = 0; i < needle.Length - 1; ++i)
            {
                int slen = suffixLength(needle, i);
                table[slen] = needle.Length - 1 - i + slen;
            }
            return table;
        }
        static bool isPrefix(byte[] needle, int p)
        {
            for (int i = p, j = 0; i < needle.Length; ++i, ++j)
                if (needle[i] != needle[j])
                    return false;
            return true;
        }
        static int suffixLength(byte[] needle, int p)
        {
            int len = 0;
            for (int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; --i, --j)
                ++len;
            return len;
        }
    }
}