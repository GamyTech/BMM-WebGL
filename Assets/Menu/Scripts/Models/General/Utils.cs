using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Collections;
using I2.Loc;
using System.Text;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public static class Utils
{
    #region Constants
    public static readonly Color Color_Green = new Color(0.0431f, 0.5686f, 0.04705f); // 0b910c
    public static readonly Color Color_Red = new Color(0.91372f, 0.141176f, 0.141176f); // e92424
    #endregion Constants

    #region General Utils
    public static bool SetProperty<T>(ref T currentValue, T newValue)
    {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
        {
            return false;
        }
        currentValue = newValue;
        return true;
    }

    public static void OpenURL(string url, bool UseNativeBrowser = false)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_WEBGL
        WebGLBrowserTabKit.OpenURLOnMouseUp(url);
#elif UNITY_ANDROID || UNITY_IOS
        if(UseNativeBrowser)
            Application.OpenURL(url);
        else
            InAppBrowser.OpenURL(url);
#else
        Application.OpenURL(url);
#endif
    }

    public static void OpenURL(string url, string PageTitle)
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;
        options.pageTitle = PageTitle;
        OpenURL(url, options);
#else
        OpenURL(url);
#endif
    }

    public static void OpenURL(string url, InAppBrowser.DisplayOptions options)
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        InAppBrowser.OpenURL(url,options);
#else
        OpenURL(url);
#endif
    }

    public static string ListParentChain(GameObject start, int parents = 7)
    {
        string chain = "";
        int x = 0;
        Transform member = start.transform;
        do
        {
            chain = "/" + member.name + chain;
            member = member.parent;
            ++x;
        }
        while (x < parents && member != null);

        return chain;
    }

    #endregion General Utils

    #region Download Utils

    public static IEnumerator DownloadPic(string url, Action<Sprite> callback, Sprite defaultSprite = null)
    {
#if UNITY_WEBGL
        string URI = url;
#else
        string URI = null;
        try
        {
            URI = (new Uri(url)).AbsoluteUri;
        }
        catch (UriFormatException e)
        {
            Debug.LogError(url + e.Message);
            callback(defaultSprite);
        }
        if (string.IsNullOrEmpty(URI))
            yield break;
#endif

        WWW w = new WWW(URI);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            if (w.texture != null)
            {
                Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                w.LoadImageIntoTexture(tex);

                if (tex != null)
                    callback(tex.ToSprite());
                else
                {
                    Debug.LogError(url + "texture not loadable into format RGB32");
                    callback(defaultSprite);
                }
            }
            else
            {
                Debug.LogError(url + "Doesn't return a texture");
                callback(defaultSprite);
            }
        }
        else
        {
            Debug.LogError(url + " Download error : " + w.error);
            callback(defaultSprite);
        }
    }

    public static IEnumerator DownloadPicOrError(string url, Action<Texture2D> callback, Action<string> errorCallback)
    {
#if UNITY_WEBGL
        string URI = url;
#else

        string URI = null;
        try
        {
            URI = (new Uri(url)).AbsoluteUri;
        }
        catch (UriFormatException e)
        {
            errorCallback(url + e.Message);
        }
        if (string.IsNullOrEmpty(URI))
            yield break;
#endif

        WWW w = new WWW(URI);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            if (w.texture != null)
            {
                Texture2D tex = new Texture2D(1, 1);
                w.LoadImageIntoTexture(tex);
                if (tex != null)
                    callback(tex);
                else
                    errorCallback(url + "texture not loadable into format RGB32");
            }
            else
                errorCallback(url + "Doesn't return a texture");
        }
        else if (errorCallback != null)
            errorCallback(w.error);
    }
#endregion Download Utils

#region Enum Utils
    public static bool TryParseEnum<T>(object value, out T returnValue, bool ShowError = false)
    {
        bool succeed = false;
        returnValue = default(T);
        try
        {
            returnValue = (T)Enum.Parse(typeof(T), value.ToString());
            succeed = true;
        }
        catch (Exception e)
        {
            if(ShowError)
                Debug.LogError(e.Message);
        }
        return succeed;
    }

    public static List<string> EnumToList<EnumType>(EnumType e)
    {
        return new List<string>(e.ToString().Replace(" ", string.Empty).Split(','));
    }
#endregion Enum Utils

#region Format Validity Utils
    public const int MIN_USERNAME_LENGTH = 4;
    public const int MAX_USERNAME_LENGTH = 12;
    public static List<string> bannedWords = new List<string>() { "fuck", "fils de pute", "encule", "con", "connard", "batard", "periprostetipute" };

    public static bool IsUserNameValid(string name, out string error)
    {
        error = null;
        IsStringValid(name, "name", out error, MIN_USERNAME_LENGTH, MAX_USERNAME_LENGTH);

        for (int i = 0; i < bannedWords.Count; i++)
            if (name.ToLower().Contains(bannedWords[i]))
                error += (!string.IsNullOrEmpty(error) ? " And " : "") + "Can't Use This Name";

        return string.IsNullOrEmpty(error);
    }

    public static readonly string VALID_EMAIL_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

    public static bool IsEmailValid(string email, out string error)
    {
        error = null;
        if (string.IsNullOrEmpty(email))
            error = LocalizeTerm("You must enter email");

        else if (System.Text.RegularExpressions.Regex.IsMatch(email, VALID_EMAIL_PATTERN) == false)
            error = LocalizeTerm("Not a valid email");
        else
            return true;

        return false;
    }

    public static bool IsEmailValid(string email, string name, out string error)
    {
        error = null;
        if (string.IsNullOrEmpty(email))
            error = LocalizeTerm("You must enter " + name);

        else if (System.Text.RegularExpressions.Regex.IsMatch(email, VALID_EMAIL_PATTERN) == false)
            error = LocalizeTerm(name + " is not a valid email");
        else
            return true;

        return false;
    }

    public const int MIN_ACCOUNT_PASSWORD_LENGTH = 6;
    public const int MAX_ACCOUNT_PASSWORD_LENGTH = 20;
    public static bool IsPasswordValid(string password, out string error)
    {
        error = null;
        return IsStringValid(password, "password", out error, MIN_ACCOUNT_PASSWORD_LENGTH, MAX_ACCOUNT_PASSWORD_LENGTH);
    }

public static bool IsStringValid(string value, string fieldName, out string error)
    {
        error = null;
        if (string.IsNullOrEmpty(value))
        {
            error = "You must enter " + fieldName;
            return false;
        }
        return true;
    }

    public static bool IsStringValid(string value, string fieldName, out string error, int minLength = -1, int maxlength = -1)
    {
        error = null;
        if (string.IsNullOrEmpty(value))
            error = "You must enter " + fieldName;

        else if (minLength != -1 && value.Length < minLength)
            error = fieldName + " is too short";

        else if(maxlength != -1 && value.Length > maxlength)
            error = fieldName + " is too long";

        else
            return true;

        return false;
    }
    
    public static bool IsPhoneValid(string value, out string error)
    {
        error = null;

        string stripped = StripExtraPhoneNumberChars(value);
        if (string.IsNullOrEmpty(value))
        {
            error = "You must enter a phone number";
            return false;
        }
        Regex regex = new Regex(@"^[0-9]*$");
        if (!regex.IsMatch(stripped))
        {
            error = "Not a valid phone number";
            return false;
        }
        if (stripped.Length < 6)
        {
            error = "Phone number is too short";
            return false;
        }
        if (stripped.Length > 15)
        {
            error = "Phone number is too long";
            return false;
        }

        return true;
    }

    private static string StripExtraPhoneNumberChars(string value)
    {
        return RemoveFirstZero(value).Replace("+", "").Replace("-", "").Replace(" ", "").Replace(".", "").Replace("(", "").Replace(")", "");
    }

    public static string RemoveFirstZero(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        string result = input;
        if (input.Length > 0 && input.Substring(0, 1) == "0")
            result = input.Substring(1, input.Length - 1);
        return result;
    }

    public static string StripPhoneNumber(string value)
    {
        string stripped = StripExtraPhoneNumberChars(value);
        int number;
        if (int.TryParse(stripped, out number))
            return number.ToString();
        return stripped;
    }

    public static bool IsNumberValid(string number, string fieldName, out string error)
    {
        error = null;
        if (string.IsNullOrEmpty(number))
        {
            error = "You must enter " + fieldName;
            return false;
        }

        int num;
        bool isNumeric = int.TryParse(number, out num);
        if (isNumeric == false)
        {
            error = fieldName + " not Valid Number";
            return false;
        }
        return true;
    }

    public static int Age(DateTime birthday)
    {
        DateTime now = DateTime.Today;
        int age = now.Year - birthday.Year;
        if (now < birthday.AddYears(age)) age--;
        return age;
    }
#endregion Format Validity Utils

#region Date Utils
    public static int[] GetDays(int year, int month)
    {
        int daysCount = DateTime.DaysInMonth(year, month);
        int[] days = new int[daysCount];
        for (int i = 0; i < days.Length; i++)
        {
            days[i] = i + 1;
        }
        return days;
    }

    public static int[] GetMonths()
    {
        int[] months = new int[12];
        for (int i = 0; i < months.Length; i++)
        {
            months[i] = i + 1;
        }
        return months;
    }

    public static int[] GetYears(int count)
    {
        int[] years = new int[count];
        DateTime date = DateTime.Now;
        for (int i = 0; i < years.Length; i++)
        {
            years[i] = date.Year;
            date = date.AddYears(-1);
        }
        return years;
    }

    public static string SecondsToTimeFormat(int totalSec)
    {
        int hours = (int)Mathf.Floor(totalSec / 3600);
        int minutes = (int)Mathf.Floor((totalSec - hours * 3600) / 60);
        int seconds = totalSec - hours * 3600 - minutes * 60;
        return AddZero(hours) + ":" + AddZero(minutes) + ":" + AddZero(seconds);
    }

    public static string SecondsToShortTimeString(int totalSec)
    {
        int hours = (int)Mathf.Floor(totalSec / 3600);
        int minutes = (int)Mathf.Floor((totalSec - hours * 3600) / 60);
        int seconds = totalSec - hours * 3600 - minutes * 60;
        return 
            (hours > 0 ? AddZero(hours) + ":" : "") + 
            (minutes > 0 ? AddZero(minutes) + ":" : "") +
            (seconds == totalSec ? seconds.ToString() + "s" : AddZero(seconds));
    }

    public static string AddZero(int number)
    {
        if (number < 10)
            return "0" + number;
        return number.ToString();
    }
#endregion Date Utils

    #region String Utils
    public static string ObjectsToString(string separator, params object[] objects)
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < objects.Length; i++)
        {
            if (i != 0) { str.Append(separator); }
            str.Append(objects[i].ToString());
        }
        return str.ToString();
    }

    public static string AddSpaceAfterUppercase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static string GetNumberPostfix(string number)
    {
        if (string.IsNullOrEmpty(number) || number.Length == 0) return "";
        string lastDigit = number.Substring(number.Length - 1, 1);
        if (lastDigit == "1")
            return "st";
        if (lastDigit == "2")
            return "nd";
        if (lastDigit == "3")
            return "rd";
        return "th";
    }
    #endregion String Utils

    #region Float Utils
    public static bool Approximately(float a, float b, float epsilon)
    {
        return Mathf.Abs(b - a) < epsilon;
    }
#endregion Float Utils

#region Vector Utils
    public static bool Approximately(Vector3 a, Vector3 b, float epsilon)
    {
        if (Approximately(a.magnitude, b.magnitude, epsilon) == false)
            return false;
        float cosAngleError = Mathf.Cos(epsilon * Mathf.Deg2Rad);
        //A value between -1 and 1 corresponding to the angle.
        float cosAngle = Vector3.Dot(a.normalized, b.normalized);
        //The dot product of normalized Vectors is equal to the cosine of the angle between them.
        //So the closer they are, the closer the value will be to 1.  Opposite Vectors will be -1
        //and orthogonal Vectors will be 0.

        if (cosAngle >= cosAngleError)
        {
            //If angle is greater, that means that the angle between the two vectors is less than the error allowed.
            return true;
        }
        return false;
    }

    public static bool Approximately(Vector2 a, Vector2 b, float epsilon)
    {
        if (Approximately(a.magnitude, b.magnitude, epsilon) == false)
            return false;
        float cosAngleError = Mathf.Cos(epsilon * Mathf.Deg2Rad);
        //A value between -1 and 1 corresponding to the angle.
        float cosAngle = Vector2.Dot(a.normalized, b.normalized);
        //The dot product of normalized Vectors is equal to the cosine of the angle between them.
        //So the closer they are, the closer the value will be to 1.  Opposite Vectors will be -1
        //and orthogonal Vectors will be 0.

        if (cosAngle >= cosAngleError)
        {
            //If angle is greater, that means that the angle between the two vectors is less than the error allowed.
            return true;
        }
        return false;
    }
#endregion Vector Utils

#region List Utils
    public static List<int> SplitToIntList(string source, params char[] splitBy)
    {
        List<int> list = new List<int>();
        string[] splitStr = source.Split(splitBy);
        for (int i = 0; i < splitStr.Length; i++)
        {
            int num;
            if (int.TryParse(splitStr[i], out num))
                list.Add(num);
        }
        return list;
    }

    public static List<float> SplitToFloatList(string source, params char[] splitBy)
    {
        List<float> list = new List<float>();
        string[] splitStr = source.Split(splitBy);
        for (int i = 0; i < splitStr.Length; i++)
        {
            float num;
            if (float.TryParse(splitStr[i], out num))
                list.Add(num);
        }
        return list;
    }
#endregion List Utils

#region Localization Utils
    public static string LocalizeTerm(string term, params object[] args)
    {
        if (string.IsNullOrEmpty(term))
            return term;

        string localized = ScriptLocalization.Get(term, true);
        if (string.IsNullOrEmpty(localized))
        {
            //Debug.LogWarning("Localization of \"" + term + "\" failed. Check source! Using original term.");
            localized = term;
        }

        try
        {
            return string.Format(localized, args);
        }
        catch (FormatException)
        {
            return localized;
        }
    }
#endregion Localization Utils

#region Screen Utils
    public static void ChangeOrientation(bool portrait)
    {
        Screen.autorotateToLandscapeLeft = !portrait;
        Screen.autorotateToLandscapeRight = !portrait;
        Screen.autorotateToPortrait = portrait;
        Screen.autorotateToPortraitUpsideDown = portrait;
        Screen.orientation = portrait ? ScreenOrientation.Portrait : ScreenOrientation.Landscape;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

#endregion Screen Utils

#region IEnumerator Utils
    public static IEnumerator Wait(float time, UnityAction action = null)
    {
        yield return new WaitForSeconds(time);
        if(action != null)
            action();
    }

    public static IEnumerator WaitForFrame(int count, UnityAction action)
    {
        int x = 0;
        while (x < count)
        {
            yield return null;
            ++x;
        }
        action();
    }

    public static IEnumerator WaitForCoroutine(Func<IEnumerator> coroutine, UnityAction action)
    {
        yield return coroutine();
        action();
    }

    public static IEnumerator WaitForBool(Func<bool> boolFunc, UnityAction action)
    {
        yield return new WaitUntil(boolFunc);
        action();
    }
#endregion IEnumerator Utils

#region Big Number Formatting
    //public static string BigNumberToString(System.Numerics.BigInteger number, int precision = 2)
    //{
    //    return BigNumberToString((double)number, precision);
    //}

    public static string BigNumberToString(long number, int precision = 2)
    {
        return BigNumberToString((double)number, precision);
    }

    public static string BigNumberToString(double number, int precision = 2)
    {
        string postfix = "";
        string num;
        if (number >= 1000000000000)
        {
            num = number.ToString("e" + precision);
            string[] parts = num.Split('e');
            postfix = "e" + parts[1];
            num = parts[0];
        }
        else
        {
            if (number >= 1000000000)
            {
                postfix = "B";
                num = (number / 1000000000f).ToString();
            }
            else if (number >= 1000000)
            {
                postfix = "M";
                num = (number / 1000000f).ToString();
            }
            else if (number >= 1000)
            {
                postfix = "K";
                num = (number / 1000f).ToString();
            }
            else
            {
                num = number.ToString();
            }
            if (num.Contains("."))
                num = num.Substring(0, Mathf.Min(num.Length, num.IndexOf('.') + precision + 1));
        }
        if (num.Contains("."))
        {
            while (precision > 0)
            {
                if (num[num.Length - 1] != '0')
                    break;
                precision--;
                num = num.Substring(0, num.Length - 1);
            }
            if (num[num.Length - 1] == '.')
            {
                num = num.Substring(0, num.Length - 1);
            }
        }
        return num + postfix;
    }
#endregion Big Number Formatting

#region Color Utils
    public static Color ColorFromHex(string hexColor)
    {
        float r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / (float)255;
        float g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / (float)255;
        float b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / (float)255;
        float a = int.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / (float)255;
        return new Color(r, g, b, a);
    }

    public static string HexFromColor(Color color)
    {
        StringBuilder hexColor = new StringBuilder();
        hexColor.Append(((int)(color.r * 255)).ToString("X"));
        hexColor.Append(((int)(color.g * 255)).ToString("X"));
        hexColor.Append(((int)(color.b * 255)).ToString("X"));
        hexColor.Append(((int)(color.a * 255)).ToString("X"));
        return hexColor.ToString();
    }
#endregion Color Utils

#region Hex
    public static string HexToString(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Substring(Math.Max(0, hex.Length - 64));
        hex = hex.TrimEnd('0');
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= hex.Length - 2; i += 2)
        {
            sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
        }
        return sb.ToString();
    }
#endregion Hex

#region Texture Utils

    public static void PathToTexture(string filePath, out Texture2D texture, out string error)
    {
        texture = null;
        error = null;

        if (!System.IO.File.Exists(filePath))
            error = "No File in " + filePath;
        else
        {
            byte[] imageData = System.IO.File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2);
            if (!texture.LoadImage(imageData))
                error = "Path is Not a Texture : " + filePath;
        }
    }

    public static Texture2D PathToTexture(string filePath)
    {
        string error;
        Texture2D texture;
        PathToTexture(filePath, out texture, out error);

        if (!string.IsNullOrEmpty(error))
            Debug.Log(error);

        return texture;
    }
#endregion Texture Utils
}

public static class SetPropertyUtility
{
    public static bool SetColor(ref Color currentValue, Color newValue)
    {
        if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
        {
            return false;
        }
        currentValue = newValue;
        return true;
    }

    public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
    {
        if (currentValue.Equals(newValue))
        {
            return false;
        }
        currentValue = newValue;
        return true;
    }

    public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
    {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
        {
            return false;
        }
        currentValue = newValue;
        return true;
    }
}
