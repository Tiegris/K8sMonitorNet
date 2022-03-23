namespace KubernetesSyncronizer.Util
{
    public static class Extensions
    {
        public static bool IsNotNullOrWhiteSpace(this string it) {
            return !string.IsNullOrWhiteSpace(it);
        }
    }
}