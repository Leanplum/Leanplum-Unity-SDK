using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CleverTap.Utilities;

namespace CleverTap.Utilities {
  public class Utils {
    public static string DictionaryToString(Dictionary<string, string> dictionary) {
      if (dictionary != null && dictionary.Count > 0) {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("{");
        foreach (KeyValuePair<string, string> entry in dictionary) {
          stringBuilder.Append(String.Format("{0}={1},", entry.Key, entry.Value));
        }
        if (stringBuilder.Length > 1) {
          stringBuilder.Length--;
        }
        stringBuilder.Append("}");
        return stringBuilder.ToString();
      } else {
        return "";
      }
    }
    
    public static string ListToString<T>(List<T> list) {
      if (list != null && list.Count > 0) {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("[");
        foreach (T element in list) {
          stringBuilder.Append(String.Format("{0},", element.ToString()));
        }
        if (stringBuilder.Length > 1) {
          stringBuilder.Length--;
        }
        stringBuilder.Append("]");
        return stringBuilder.ToString();
      } else {
        return "";
      }
    }

    public static Dictionary<string, string> JSONClassToDictionary(JSONClass json) {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (json != null) {
        IEnumerator enumerator = json.GetEnumerator();
        while (enumerator.MoveNext()) {
          KeyValuePair<string, JSONNode> entry = (KeyValuePair<string, JSONNode>)enumerator.Current;
          dictionary.Add(entry.Key, entry.Value);
        }
      }
      return dictionary;
    }
  }
}
