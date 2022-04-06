using System.Linq;
using System.Text.RegularExpressions;

namespace KubernetesSyncronizer.Util;
static public class KeyStringExtensions
{

    public static string GetNs(this string value) {
        var ss = value.Split("::");
        return ss.Last();
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
