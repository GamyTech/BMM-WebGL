using System;
using System.Collections;
using UnityEngine;


public static class Change
{
    public enum ChangeType
    {
        Linear,
        QuadEaseIn,
        QuadEaseOut,
        QuadEaseInOut,
        ExpoEaseIn,
        ExpoEaseOut,
        ExpoEaseInOut,
        QuadBounce,
    }

    public static IEnumerator GenericChange<T>(T start, T end, float duration, Func<T, T, float, T> changeAction, Action<T> onChanged, Action onFinished)
    {
        float counter = 0;
        while (counter < duration)
        {
            if (onChanged != null)
                onChanged(changeAction(start, end, counter / duration));
            yield return null;
            counter += Time.deltaTime;
        }
        if (onChanged != null)
            onChanged(end);
        if (onFinished != null)
            onFinished();
    }



    #region Float
    public static float ChangeWithType(ChangeType type, float start, float end, float duration)
    {
        switch (type)
        {
            case ChangeType.Linear:
                return Lerp(start, end, duration);
            case ChangeType.QuadEaseIn:
                return EaseInQuad(start, end, duration);
            case ChangeType.QuadEaseOut:
                return EaseOutQuad(start, end, duration);
            case ChangeType.QuadEaseInOut:
                return EaseInOutQuad(start, end, duration);
            case ChangeType.ExpoEaseIn:
                return EaseInExpo(start, end, duration);
            case ChangeType.ExpoEaseOut:
                return EaseOutExpo(start, end, duration);
            case ChangeType.ExpoEaseInOut:
                return EaseInOutExpo(start, end, duration);
            case ChangeType.QuadBounce:
                return BounceQuad(start, end, duration);
            default:
                return Lerp(start, end, duration);
        }
    }
    public static float Lerp(float start, float end, float duration)
    {
        return Mathf.Lerp(start, end, duration);
    }
    public static float EaseInQuad(float start, float end, float duration)
    {
        return (end - start) * duration * duration + start;
    }
    public static float EaseOutQuad(float start, float end, float duration)
    {
        return -(end - start) * duration * (duration - 2) + start;
    }
    public static float EaseInOutQuad(float start, float end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * duration * duration + start;
        duration--;
        return -(end - start) / 2 * (duration * (duration - 2) - 1) + start;
    }

    public static float EaseInExpo(float start, float end, float duration)
    {
        return (end - start) * duration * duration + start;
    }
    public static float EaseOutExpo(float start, float end, float duration)
    {
        return -(end - start) * duration * (duration - 2) + start;
    }
    public static float EaseInOutExpo(float start, float end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * Mathf.Pow(2, 10 * (duration - 1)) + start;
        duration--;
        return (end - start) / 2 * (-Mathf.Pow(2, -10 * duration) + 2) + start;
    }

    public static float BounceQuad(float start, float end, float duration)
    {
        duration *= 4;
        float dif = end - start;
        if (duration < 3)
            return dif / 4 * duration * duration + start;
        duration -= 3;
        return dif / (-duration) * duration + start;
    }
    #endregion Float

    #region Vector2
    public static Vector2 ChangeWithType(ChangeType type, Vector2 start, Vector2 end, float duration)
    {
        switch (type)
        {
            case ChangeType.Linear:
                return Lerp(start, end, duration);
            case ChangeType.QuadEaseIn:
                return EaseInQuad(start, end, duration);
            case ChangeType.QuadEaseOut:
                return EaseOutQuad(start, end, duration);
            case ChangeType.QuadEaseInOut:
                return EaseInOutQuad(start, end, duration);
            case ChangeType.ExpoEaseIn:
                return EaseInExpo(start, end, duration);
            case ChangeType.ExpoEaseOut:
                return EaseOutExpo(start, end, duration);
            case ChangeType.ExpoEaseInOut:
                return EaseInOutExpo(start, end, duration);
            case ChangeType.QuadBounce:
                return Lerp(start, end, duration);
            default:
                return Lerp(start, end, duration);
        }
    }
    public static Vector2 Lerp(Vector2 start, Vector2 end, float duration)
    {
        return Vector2.Lerp(start, end, duration);
    }
    public static Vector2 EaseInQuad(Vector2 start, Vector2 end, float duration)
    {
        return (end - start) * duration * duration + start;
    }
    public static Vector2 EaseOutQuad(Vector2 start, Vector2 end, float duration)
    {
        return -(end - start) * duration * (duration - 2) + start;
    }
    public static Vector2 EaseInOutQuad(Vector2 start, Vector2 end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * duration * duration + start;
        duration--;
        return -(end - start) / 2 * (duration * (duration - 2) - 1) + start;
    }
    public static Vector2 EaseInExpo(Vector2 start, Vector2 end, float duration)
    {
        return (end - start) * duration * duration + start;
    }
    public static Vector2 EaseOutExpo(Vector2 start, Vector2 end, float duration)
    {
        return -(end - start) * duration * (duration - 2) + start;
    }
    public static Vector2 EaseInOutExpo(Vector2 start, Vector2 end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * Mathf.Pow(2, 10 * (duration - 1)) + start;
        duration--;
        return (end - start) / 2 * (-Mathf.Pow(2, -10 * duration) + 2) + start;
    }
    #endregion Vector2

    #region Vector3
    public static Vector3 ChangeWithType(ChangeType type, Vector3 start, Vector3 end, float duration)
    {
        switch (type)
        {
            case ChangeType.Linear:
                return Lerp(start, end, duration);
            case ChangeType.QuadEaseIn:
                return EaseInQuad(start, end, duration);
            case ChangeType.QuadEaseOut:
                return EaseOutQuad(start, end, duration);
            case ChangeType.QuadEaseInOut:
                return EaseInOutQuad(start, end, duration);
            case ChangeType.ExpoEaseIn:
                return EaseInExpo(start, end, duration);
            case ChangeType.ExpoEaseOut:
                return EaseOutExpo(start, end, duration);
            case ChangeType.ExpoEaseInOut:
                return EaseInOutExpo(start, end, duration);
            case ChangeType.QuadBounce:
                return Lerp(start, end, duration);
            default:
                return Lerp(start, end, duration);
        }
    }

    public static Vector3 Lerp(Vector3 start, Vector3 end, float index)
    {
        return Vector3.Lerp(start, end, index);
    }

    public static Vector3 EaseInQuad(Vector3 start, Vector3 end, float duration)
    {
        return (end - start) * duration * duration + start;
    }

    public static Vector3 EaseOutQuad(Vector3 start, Vector3 end, float duration)
    {
        return -(end - start) * duration * (duration - 2) + start;
    }

    public static Vector3 EaseInOutQuad(Vector3 start, Vector3 end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * duration * duration + start;
        duration--;
        return -(end - start) / 2 * (duration * (duration - 2) - 1) + start;
    }

    public static Vector3 EaseInExpo(Vector3 start, Vector3 end, float duration)
    {
        return (end - start) * duration * duration + start;
    }

    public static Vector3 EaseOutExpo(Vector3 start, Vector3 end, float duration)
    {
        return -(end - start) * duration * (duration - 2) + start;
    }

    public static Vector3 EaseInOutExpo(Vector3 start, Vector3 end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * Mathf.Pow(2, 10 * (duration - 1)) + start;
        duration--;
        return (end - start) / 2 * (-Mathf.Pow(2, -10 * duration) + 2) + start;
    }

    #endregion Vector3

    #region Color
    public static Color ChangeWithType(ChangeType type, Color start, Color end, float duration)
    {
        switch (type)
        {
            case ChangeType.Linear:
                return Lerp(start, end, duration);
            case ChangeType.QuadEaseIn:
                return EaseInQuad(start, end, duration);
            case ChangeType.QuadEaseOut:
                return EaseOutQuad(start, end, duration);
            case ChangeType.QuadEaseInOut:
                return EaseInOutQuad(start, end, duration);
            case ChangeType.ExpoEaseIn:
                return EaseInExpo(start, end, duration);
            case ChangeType.ExpoEaseOut:
                return EaseOutExpo(start, end, duration);
            case ChangeType.ExpoEaseInOut:
                return EaseInOutExpo(start, end, duration);
            case ChangeType.QuadBounce:
                return Lerp(start, end, duration);
            default:
                return Lerp(start, end, duration);
        }
    }
    public static Color Lerp(Color start, Color end, float duration)
    {
        return Color.Lerp(start, end, duration);
    }
    public static Color EaseInQuad(Color start, Color end, float duration)
    {
        return (end - start) * duration * duration + start;
    }
    public static Color EaseOutQuad(Color start, Color end, float duration)
    {
        return (start - end) * duration * (duration - 2) + start;
    }
    public static Color EaseInOutQuad(Color start, Color end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * duration * duration + start;
        duration--;
        return (start - end) / 2 * (duration * (duration - 2) - 1) + start;
    }
    public static Color EaseInExpo(Color start, Color end, float duration)
    {
        return (end - start) * duration * duration + start;
    }
    public static Color EaseOutExpo(Color start, Color end, float duration)
    {
        return (start - end) * duration * (duration - 2) + start;
    }
    public static Color EaseInOutExpo(Color start, Color end, float duration)
    {
        duration *= 2;
        if (duration < 1)
            return (end - start) / 2 * Mathf.Pow(2, 10 * (duration - 1)) + start;
        duration--;
        return (end - start) / 2 * (-Mathf.Pow(2, -10 * duration) + 2) + start;
    }
    #endregion Color
}
