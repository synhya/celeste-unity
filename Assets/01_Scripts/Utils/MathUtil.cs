using static UnityEngine.Mathf;

public static class MathUtil
{
    public static float Appr(float val, float target, float amount)
    {
        return (val > target
            ? Max(val - amount, target)
            : Min(val + amount, target));
    }
}


