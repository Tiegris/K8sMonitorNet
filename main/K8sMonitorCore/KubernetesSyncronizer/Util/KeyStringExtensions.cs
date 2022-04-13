using System.Linq;
using System.Text.RegularExpressions;

namespace KubernetesSyncronizer.Util;
static public class KeyStringExtensions
{
    public static string MakeKey(string ns, string srv) => $"{ns}::{srv}";
    public static string MakeKey(string ns, string srv, string pod) => $"{ns}::{srv}::{pod}";

    public static string GetNs(this string value) {
        var ss = value.Split("::");
        return ss.First();
    }

    public static string GetSrvNs(this string value) {
        var c = Regex.Matches(value, "::").Count;
        if (c == 1) {
            return value;
        }
        if (c == 2) {
            var ss = value.Split("::", 2);
            return ss.Last();
        }
        return value;
    }

}
