using UnityEngine;

public class HotArea : MonoBehaviour {
    public Vector3 Size;
    public int Heat;
    public float ExistTime;

    public void GettingCold(int cold)
    {
        Heat -= cold;
    }
    public void GettingHot(int heat)
    {
        Heat += heat;
    }
}