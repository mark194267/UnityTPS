using UnityEngine;
using UnityEngine.AI;

public class HotArea : MonoBehaviour {
    private int Size;
    private int Heat;
    private float ExistTime;
    private NavMeshModifierVolume nv;

    private void Start() 
    {
        nv = GetComponent<NavMeshModifierVolume>();
    }
    /// <summary>
    /// *需套用ApplyTemperature才有效果，改變此物件的溫度
    /// </summary>
    /// <param name="heat"></param>
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
    /// 更新此物件的溫度，影響大小
    /// </summary>
    public void ApplyTemperature()
    {
        nv.area = Heat;
        nv.size = Vector3.one * Heat;
    }
}