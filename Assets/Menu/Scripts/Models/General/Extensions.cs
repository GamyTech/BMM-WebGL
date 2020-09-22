using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;


public static class Extensions
{
    #region IDictionary Extenstions
    public static void AddOrIgnoreValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue valueToAdd)
    {
        if (dict.ContainsKey(key) == false)
        {
            dict.Add(key, valueToAdd);
        }
    }

    public static void AddOrOverrideValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue valueToAdd)
    {
        if (dict == null) return;

        if(dict.ContainsKey(key))
        {
            Debug.LogWarning("AddOrOverrideValue :: Overriding Key " + key.ToString());
            dict[key] = valueToAdd;
        }
        else
        {
            dict.Add(key, valueToAdd);
        }
    }

    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> slave, IDictionary<TKey, TValue> master)
    {
        foreach (var item in master)
        {
            if(slave.ContainsKey(item.Key))
            {
                slave[item.Key] = item.Value;
            }
            else
            {
                slave.Add(item.Key, item.Value);
            }
        }
    }

    public static TValue GetFirstValue<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0) return default(TValue);
        return dict[dict.Keys.First()];
    }

    public static TKey GetFirstKey<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0) return default(TKey);
        return dict.Keys.First();
    }

    public static TValue GetFirstValueExcluding<TKey, TValue>(this IDictionary<TKey, TValue> dict, params TKey[] exceptKeys)
    {
        foreach (var item in dict)
        {
            if (exceptKeys.Contains(item.Key) == false)
                return item.Value; 
        }
        return default(TValue);
    }
    #endregion IDictionary Extenstions

    #region IList Extenstions

    public static TKey GetLastKey<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0) return default(TKey);
        return dict.Keys.Last();
    }

    public static T SafeGet<T>(this IList<T> list, int index)
    {
        return list[Mathf.Clamp(index, 0, list.Count)];
    }

    public static TValue GetLastValue<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0) return default(TValue);
        return dict[dict.Keys.Last()];
    }
    #endregion IDictionary Extenstions

    #region IList Extenstions
    public static bool IsValidIndex(this IList list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    /// <summary>
    /// Object comparer
    /// </summary>
    /// <param name="list"></param>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool Contains(this IList list, object o)
    {
        if (o == null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    return true;
                }
            }
            return false;
        }

        for (int j = 0; j < list.Count; j++)
        {
            if(list[j].Equals(o))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Adds other list to list if items are missing from it
    /// </summary>
    /// <param name="list">Must be non read only and variable size IList</param>
    /// <param name="other">Any list</param>
    /// <returns></returns>
    public static IList AddMissingItems(this IList list, IList other)
    {
        if (list.IsFixedSize || list.IsReadOnly || other == null) return list;

        for (int i = 0; i < other.Count; i++)
        {
            if (list.Contains(other[i]) == false)
            {
                list.Add(other[i]);
            }
        }
        return list;
    }

    public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            list.Remove(item);
        }
    }
    #endregion IList Extenstions

    #region IEnumrable Extensions
    public static IEnumerable<IList<T>> Chunks<T>(this IEnumerable<T> xs, int size)
    {
        var curr = new List<T>(size);

        foreach (var x in xs)
        {
            curr.Add(x);

            if (curr.Count == size)
            {
                yield return curr;
                curr = new List<T>(size);
            }
        }
    }

    /// <summary>
    /// Splits an array into several smaller arrays.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="array">The array to split.</param>
    /// <param name="size">The size of the smaller arrays.</param>
    /// <returns>An array containing smaller arrays.</returns>
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
    {
        for (var i = 0; i < (float)array.Length / size; i++)
        {
            yield return array.Skip(i * size).Take(size);
        }
    }

    public static bool IsValuesEqual(this IList list, IList other)
    {
        if (list.Count != other.Count)
            return false;
        for (int i = 0; i < list.Count; i++)
            if (!list[i].Equals(other[i]))
                return false;
        return true;
    }

    public static string Display<K, V>(this KeyValuePair<K, V> pair, string separator = " , ")
    {
        string s = "[" + pair.Key.ToString() + ":";
        if (pair.Value is string)
            s += pair.Value.ToString();
        else if (pair.Value is IEnumerable)
        {
            s += ((IEnumerable)pair.Value).Display();
        }
        else
        {
            s += pair.Value.ToString();
        }
        s += "]";
        return s;
    }

    public static string Display<K, V>(this IEnumerable collection, string separator = " , ")
    {
        if (collection == null)
            return "{ }";
        string s = "{ ";
        bool first = true;
        foreach (var item in collection)
        {
            if (first)
                first = false;
            else
                s += separator;
            if (item is string)
                s += item.ToString();
            else if (item is KeyValuePair<K, V>)
            {
                s += ((KeyValuePair<K, V>)item).Display(separator);
            }
            else if (item is IEnumerable)
            {
                s += ((IEnumerable)item).Display(separator);
            }
            else
            {
                s += item.ToString();
            }
        }
        return s + " }";
    }

    public static string Display(this IEnumerable collection, string separator = " , ")
    {
        if (collection == null)
            return "{ }";
        string s = "{ ";
        bool first = true;
        foreach (var item in collection)
        {
            if (first)
                first = false;
            else
                s += separator;
            if (item is string)
                s += item.ToString();
            else if(item is IDictionary<string ,object>)
            {
                s += ((IEnumerable)item).Display(separator);
            }
            else if (item is KeyValuePair<string, object> )
            {
                s += ((KeyValuePair<string, object>)item).Display();
            }
            else if (item is IEnumerable)
            {
                s += ((IEnumerable)item).Display(separator);
            }
            else if(item == null)
            {
                s += "null";
            }
            else
            {
                s += item.ToString();
            }
        }
        return s + " }";
    }

    public static string ToSeparatedString<T>(this IEnumerable<T> collection, string separator, System.Func<T, string> toStringFunc = null)
    {
        if (toStringFunc == null)
            toStringFunc = t => { return t.ToString(); };

        string s = string.Empty;
        bool first = true;
        foreach (var item in collection)
        {
            if (!first)
                s += separator;
            else first = false;
            s += toStringFunc(item);
        }
        return s;
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        System.Random rng = new System.Random();
        var buffer = source.ToList();
        for (int i = 0; i < buffer.Count; i++)
        {
            int j = rng.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    public static List<string> ToListOfStrings(this IEnumerable source)
    {
        List<string> list = new List<string>();
        foreach (var item in source)
        {
            list.Add(item.ToString());
        }
        return list;
    }
    #endregion IEnumrable Extensions

    #region List Extensions
    public static List<T> AddRepeated<T>(this List<T> list, T num, int repeats)
    {
        for (int i = 0; i < repeats; i++)
        {
            list.Add(num);
        }
        return list;
    }

    public static List<int> AddSequence(this List<int> list, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            list.Add(i);
        }
        return list;
    }


    public static List<string> ToLower(this List<string> source)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < source.Count; i++)
            list.Add(source[i].ToLower());
        return list;
    }
    #endregion List Extensions

    #region GameObject Extensions
    public static void InitGameObjectAfterInstantiation(this GameObject go, Transform parent)
    {
        go.transform.SetParent(parent, false);
        go.ResetGameObject();
    }

    public static void ResetGameObject(this GameObject go)
    {
        go.transform.localScale = new Vector3(1, 1, 1);
        Vector3 temp = go.transform.localPosition;
        temp.z = 0;
        go.transform.localPosition = temp;
    }

    public static void SetLayer(this GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            child.gameObject.SetLayer(layer);
    }
    #endregion GameObject Extensions

    #region Buttton Extensions
    public static void RegisterCallbackOnPressedDown(this UnityEngine.UI.Button button)
    {
        ExtendedInputModule.OnPressedDownCallbacks.AddOrOverrideValue(button.GetInstanceID(), button.onClick.Invoke);
    }

    public static void UnregisterCallbackOnPressedDown(this UnityEngine.UI.Button button)
    {
        int instanceID = button.GetInstanceID();
        if (ExtendedInputModule.OnPressedDownCallbacks.ContainsKey(instanceID))
            ExtendedInputModule.OnPressedDownCallbacks.Remove(instanceID);
    }
    #endregion Buttton Extensions

    #region RectTransform
    public static Bounds GetWorldBounds(this RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        Vector3 vector = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
        Vector3 vector2 = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
        rt.GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            vector = Vector3.Min(corners[i], vector);
            vector2 = Vector3.Max(corners[i], vector2);
        }
        Bounds result = new Bounds(vector, new Vector3(0,0,0));
        result.Encapsulate(vector2);
        return result;
    }
    #endregion RectTransform

    #region Texture
    public static Sprite ToSprite(this Texture2D t)
    {
        return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
    }
    #endregion Texture

    #region object
    public static float ParseFloat(this object o)
    {
        float parsedValue;
        if (!float.TryParse(o.ToString(), out parsedValue))
            Debug.LogError("Unable to parse " + o.ToString());
        
        return parsedValue;
    }

    public static int ParseInt(this object o)
    {
        int parsedValue;
        if (!int.TryParse(o.ToString(), out parsedValue))
            Debug.LogError("Unable to parse " + o.ToString());
        
        return parsedValue;
    }

    public static bool ParseBool(this object o)
    {
        bool parsedValue;
        if (!bool.TryParse(o.ToString(), out parsedValue))
        {
            int i = 2;
            if (int.TryParse(o.ToString(), out i) && (i == 0 || i == 1))
                parsedValue = i == 0 ? false : true;
            else
                Debug.LogError("Unable to parse " + o.ToString());
        }
        return parsedValue;
    }
    #endregion object

    #region float
    public static bool IsValidNum(this float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
    #endregion float

    #region String Extensions
    public static bool EqualsIgnoreCase(this string str1, string str2)
    {
        return str1.ToLower().Equals(str2.ToLower());
    }

    public static string AddCharRepeated(this string str, string toAdd, int atIndex, int times)
    {
        return new StringBuilder(str).Insert(atIndex, toAdd, times).ToString();
    }

    public static string AddCharRepeatedToEnd(this string str, char toAdd, int times)
    {
        return new StringBuilder(str).Append(toAdd, times).ToString();
    }

    public static int DiffCount(this string str, string other)
    {
        string[] c1 = str.Split(',');
        string[] c2 = other.Split(',');
        int count = 0;
        for (int i = 0; i < c1.Length; i++)
        {
            for (int j = 0; j < c2.Length; j++)
            {
                if (c1[i].Equals(c2[j])) count++;
            }
        }
        return count;
    }

    public static bool ContainsCharsNotInString(this string str, string chars)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if(chars.Contains(str[i]) == false)
            {
                return true;
            }
        }
        return false;
    }
    #endregion String Extensions

    #region Color
    public static Color OppositeColor(this Color color)
    {
        return new Color(1 - color.r, 1 - color.g, 1 - color.b);
    }
    #endregion Color

    #region Arrays
    public static T[] MergeArray<T>(this T[] array1, T[] array2)
    {
        int array1OriginalLength = array1.Length;
        Array.Resize<T>(ref array1, array1OriginalLength + array2.Length);
        Array.Copy(array2, 0, array1, array1OriginalLength, array2.Length);
        return array1;
    }
    #endregion Arrays
}
