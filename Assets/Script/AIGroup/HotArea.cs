using UnityEngine;
using UnityEngine.AI;

public class HotArea : MonoBehaviour {
    public int Size;
    public int Heat;
    public float ExistTime;
    public NavMeshModifierVolume nv;

    private void Start() 
    {
        nv = GetComponent<NavMeshModifierVolume>();
    }
    public void ChangeTemperature(int heat)
    {
        //檢查是否還能更熱
        if (Heat + heat > 32)
        {
            Heat = 32;
        }
        else if (Heat + heat < 3)
        {
            Destroy(this);
        }
        else
        {
            Heat += heat;
        }
    }
    /// <summary>
    /// 更新此物件的溫度。
    /// </summary>
    public void ApplyTemperature()
    {
        nv.area = Heat;
        nv.size = Vector3.one * Size;
    }
}