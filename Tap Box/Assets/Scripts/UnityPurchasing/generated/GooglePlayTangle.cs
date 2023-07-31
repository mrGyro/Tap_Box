// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ewzTknCawNp4zF47/pvu039+Ov8ON0qAoIfyAubbixJNfaFEAilJ9H+imjYIkD/yaN+AbupeBR2WCEj6Ok3TiwzdOxtAuhLveGWgBo8Jh6wkROWYIgIPo3BmUH1XfRc6wll1et5HF1YzUrNJ+RX3/hIx1pNqiltohgULBDSGBQ4GhgUFBKkZ5G13w46JL7eIrt7t/Y+y2YQ1pzaYUvY23V5WYl98/YbAGmi+kY7snnakH1i971cbHGZFfH71Z/ZGQQONtxMUDsBym5azfNA9U0VNuL7fTFWiSpnMS3zeSWs+kqZiKZOWJpufUmGf4t+WOf2n7NCy5r1Pvn8WuXrcX+FVsuE0hgUmNAkCDS6CTILzCQUFBQEEB38mvyhr+L/dlQYHBQQF");
        private static int[] order = new int[] { 11,6,8,13,8,13,6,9,12,10,12,13,12,13,14 };
        private static int key = 4;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
