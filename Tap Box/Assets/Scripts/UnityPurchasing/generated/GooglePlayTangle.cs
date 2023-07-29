// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("HbsjHDpKeWkbJk0QoTOiDMZiokmgEpGyoJ2WmboW2BZnnZGRkZWQk67ZRx+YSa+P1C6Ge+zxNJIbnRM4EpGfkKASkZqSEpGRkD2NcPnjVxroSt3/qgYy9r0HArIPC8b1C3ZLArDQcQy2lps35PLE6cPpg65WzeHuStODwqfGJ91tgWNqhqVCB/4ez/zKwvbL6GkSVI78KgUaeAriMIvMKe+YRwbkDlRO7FjKr2oPekfr6q5r6zYOopwEq2b8SxT6fsqRiQKc3G6taTN4RCZyKdsq64It7kjLdcEmdeYPAifoRKnH0dksKkvYwTbeDVjfe8OPiPLR6Oph82LS1ZcZI4eAmlSao94UNBNmlnJPH4bZ6TXQlr3dYOuyK7z/bCtJAZKTkZCR");
        private static int[] order = new int[] { 9,9,13,9,5,13,9,12,11,11,12,13,12,13,14 };
        private static int key = 144;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
