namespace OmniView.Library.Common
{
    public class ByteSize
    {
        private const long Unit_Kb = 1024L;
        private const long Unit_Mb = 1024L * 1024L;
        private const long Unit_Gb = 1024L * 1024L * 1024L;
        private const long Unit_Tb = 1024L * 1024L * 1024L * 1024L;

        public long Value {get;}


        public ByteSize(long value)
        {
            this.Value = value;
        }

        public decimal ToKb()
        {
            return (decimal)Value / Unit_Kb;
        }

        public decimal ToMb()
        {
            return (decimal)Value / Unit_Mb;
        }

        public decimal ToGb()
        {
            return (decimal)Value / Unit_Gb;
        }

        public decimal ToTb()
        {
            return (decimal)Value / Unit_Tb;
        }

        public string ToRelative()
        {
            var v = (double)Value;

            if (v < 1024) return $"{v:N1} B";
            v /= 1024;

            if (v < 1024) return $"{v:N1} KB";
            v /= 1024;

            if (v < 1024) return $"{v:N1} MB";
            v /= 1024;

            if (v < 1024) return $"{v:N1} GB";
            v /= 1024;

            return $"{v:N0} TB";
        }

        public static ByteSize FromKb(int value)
        {
            return new ByteSize(value * Unit_Kb);
        }

        public static ByteSize FromMb(int value)
        {
            return new ByteSize(value * Unit_Mb);
        }

        public static ByteSize FromGb(int value)
        {
            return new ByteSize(value * Unit_Gb);
        }

        public static ByteSize FromTb(int value)
        {
            return new ByteSize(value * Unit_Tb);
        }
    }
}
