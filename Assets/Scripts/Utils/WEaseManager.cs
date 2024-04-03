using UnityEngine;

public enum WEaseType
{
    Linear,
    SineIn,
    SineOut,
    SineInOut,
    QuadIn,
    QuadOut,
    QuadInOut,
    CubicIn,
    CubicOut,
    CubicInOut,
    QuartIn,
    QuartOut,
    QuartInOut,
    QuintIn,
    QuintOut,
    QuintInOut,
    ExpoIn,
    ExpoOut,
    ExpoInOut,
    CircIn,
    CircOut,
    CircInOut,
    ElasticIn,
    ElasticOut,
    ElasticInOut,
    BackIn,
    BackOut,
    BackInOut,
    BounceIn,
    BounceOut,
    BounceInOut,
    // Custom
}

public class WEaseManager : MonoBehaviour
{
    const float _PiOver2 = Mathf.PI * 0.5f;
    const float _TwoPi = Mathf.PI * 2;

    /// <summary>
    /// Returns a value between 0 and 1 (inclusive) based on the elapsed time and ease selected
    /// </summary>
    public static float Evaluate(WEaseType WEaseType, float time, float duration,
        float overshootOrAmplitude = 1.70158f,
        float period = 0f)
    {
        if (duration <= 0f)
            return 1f;

        switch (WEaseType)
        {
            case WEaseType.Linear:
                return time / duration;
            case WEaseType.SineIn:
                return -Mathf.Cos(time / duration * _PiOver2) + 1f;
            case WEaseType.SineOut:
                return Mathf.Sin(time / duration * _PiOver2);
            case WEaseType.SineInOut:
                return -0.5f * (Mathf.Cos(Mathf.PI * time / duration) - 1f);
            case WEaseType.QuadIn:
                return (time /= duration) * time;
            case WEaseType.QuadOut:
                return -(time /= duration) * (time - 2f);
            case WEaseType.QuadInOut:
                if ((time /= duration * 0.5f) < 1f) return 0.5f * time * time;
                return -0.5f * ((--time) * (time - 2f) - 1f);
            case WEaseType.CubicIn:
                return (time /= duration) * time * time;
            case WEaseType.CubicOut:
                return ((time = time / duration - 1f) * time * time + 1f);
            case WEaseType.CubicInOut:
                if ((time /= duration * 0.5f) < 1f) return 0.5f * time * time * time;
                return 0.5f * ((time -= 2f) * time * time + 2f);
            case WEaseType.QuartIn:
                return (time /= duration) * time * time * time;
            case WEaseType.QuartOut:
                return -((time = time / duration - 1) * time * time * time - 1);
            case WEaseType.QuartInOut:
                if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time;
                return -0.5f * ((time -= 2) * time * time * time - 2);
            case WEaseType.QuintIn:
                return (time /= duration) * time * time * time * time;
            case WEaseType.QuintOut:
                return ((time = time / duration - 1) * time * time * time * time + 1);
            case WEaseType.QuintInOut:
                if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time * time;
                return 0.5f * ((time -= 2) * time * time * time * time + 2);
            case WEaseType.ExpoIn:
                return (time == 0) ? 0 : Mathf.Pow(2, 10 * (time / duration - 1));
            case WEaseType.ExpoOut:
                if (time == duration) return 1;
                return (-Mathf.Pow(2, -10 * time / duration) + 1);
            case WEaseType.ExpoInOut:
                if (time == 0) return 0;
                if (time == duration) return 1;
                if ((time /= duration * 0.5f) < 1) return 0.5f * Mathf.Pow(2, 10 * (time - 1));
                return 0.5f * (-Mathf.Pow(2, -10 * --time) + 2);
            case WEaseType.CircIn:
                return -(Mathf.Sqrt(1 - (time /= duration) * time) - 1);
            case WEaseType.CircOut:
                return Mathf.Sqrt(1 - (time = time / duration - 1) * time);
            case WEaseType.CircInOut:
                if ((time /= duration * 0.5f) < 1) return -0.5f * (Mathf.Sqrt(1 - time * time) - 1);
                return 0.5f * (Mathf.Sqrt(1 - (time -= 2) * time) + 1);
            case WEaseType.ElasticIn:
                float s0;
                if (time == 0) return 0;
                if ((time /= duration) == 1) return 1;
                if (period == 0) period = duration * 0.3f;
                if (overshootOrAmplitude < 1)
                {
                    overshootOrAmplitude = 1;
                    s0 = period / 4;
                }
                else s0 = period / _TwoPi * Mathf.Asin(1 / overshootOrAmplitude);

                return -(overshootOrAmplitude * Mathf.Pow(2, 10 * (time -= 1)) *
                         Mathf.Sin((time * duration - s0) * _TwoPi / period));
            case WEaseType.ElasticOut:
                float s1;
                if (time == 0) return 0;
                if ((time /= duration) == 1) return 1;
                if (period == 0) period = duration * 0.3f;
                if (overshootOrAmplitude < 1)
                {
                    overshootOrAmplitude = 1;
                    s1 = period / 4;
                }
                else s1 = period / _TwoPi * Mathf.Asin(1 / overshootOrAmplitude);

                return (overshootOrAmplitude * Mathf.Pow(2, -10 * time) *
                    Mathf.Sin((time * duration - s1) * _TwoPi / period) + 1);
            case WEaseType.ElasticInOut:
                float s;
                if (time == 0) return 0;
                if ((time /= duration * 0.5f) == 2) return 1;
                if (period == 0) period = duration * (0.3f * 1.5f);
                if (overshootOrAmplitude < 1)
                {
                    overshootOrAmplitude = 1;
                    s = period / 4;
                }
                else s = period / _TwoPi * Mathf.Asin(1 / overshootOrAmplitude);

                if (time < 1f)
                    return -0.5f * (overshootOrAmplitude * Mathf.Pow(2f, 10f * (time -= 1f)) *
                                    Mathf.Sin((time * duration - s) * _TwoPi / period));
                return overshootOrAmplitude * Mathf.Pow(2f, -10 * (time -= 1f)) *
                    Mathf.Sin((time * duration - s) * _TwoPi / period) * 0.5f + 1f;
            case WEaseType.BackIn:
                return (time /= duration) * time * ((overshootOrAmplitude + 1f) * time - overshootOrAmplitude);
            case WEaseType.BackOut:
                return ((time = time / duration - 1f) * time *
                    ((overshootOrAmplitude + 1f) * time + overshootOrAmplitude) + 1f);
            case WEaseType.BackInOut:
                if ((time /= duration * 0.5f) < 1f)
                    return 0.5f * (time * time *
                                   (((overshootOrAmplitude *= (1.525f)) + 1) * time - overshootOrAmplitude));
                return 0.5f * ((time -= 2f) * time *
                    (((overshootOrAmplitude *= (1.525f)) + 1) * time + overshootOrAmplitude) + 2);
            case WEaseType.BounceIn:
                return Bounce.EaseIn(time, duration);
            case WEaseType.BounceOut:
                return Bounce.EaseOut(time, duration);
            case WEaseType.BounceInOut:
                return Bounce.EaseInOut(time, duration);

            // case WEaseType.Custom:
            //     return customEase != null ? customEase.Evaluate(time / duration) : (time / duration);

            default:
                return -(time /= duration) * (time - 2);
        }
    }

    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    static class Bounce
    {
        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
        /// </summary>
        /// <param name="time">
        /// Current time (in frames or seconds).
        /// </param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>
        /// The eased value.
        /// </returns>
        public static float EaseIn(float time, float duration)
        {
            return 1 - EaseOut(duration - time, duration);
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
        /// </summary>
        /// <param name="time">
        /// Current time (in frames or seconds).
        /// </param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>
        /// The eased value.
        /// </returns>
        public static float EaseOut(float time, float duration)
        {
            if ((time /= duration) < (1 / 2.75f))
            {
                return (7.5625f * time * time);
            }

            if (time < (2 / 2.75f))
            {
                return (7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f);
            }

            if (time < (2.5f / 2.75f))
            {
                return (7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f);
            }

            return (7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f);
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="time">
        /// Current time (in frames or seconds).
        /// </param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>
        /// The eased value.
        /// </returns>
        public static float EaseInOut(float time, float duration)
        {
            if (time < duration * 0.5f)
            {
                return EaseIn(time * 2, duration) * 0.5f;
            }

            return EaseOut(time * 2 - duration, duration) * 0.5f + 0.5f;
        }
    }
}
