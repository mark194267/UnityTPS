using UnityEngine;
using UnityEngine.AI;

public class HotArea : MonoBehaviour {

    public Vector3 position { set { transform.position = value; } }
    public Quaternion rot { set { transform.rotation = value; } }

    public bool IsOn
    {
        get { return Nv.enabled; }
        set
        {
            Nv.enabled = value;
        }
    }
    public int size = 4;
    public int nowHeat
    {
        get { return heat; }
        set
        {
            heat = value;
            ApplyTemperature();
        }
    }

    private int heat;

    private float existTime;
    public NavMeshModifierVolume Nv { get; set; }

    private void OnEnable() 
    {
        Nv = GetComponent<NavMeshModifierVolume>();
    }
    /// <summary>
    /// *需套用ApplyTemperature才有效果，改變此物件的溫度
    /// </summary>
    /// <param name="heat"></param>
    public void ChangeTemperature(int tempheat)
    {
        //Debug.Log("NowHeat: "+heat);
        //檢查是否還能更熱
        if (heat + tempheat > 32)
        {
            heat = 32;
        }
        else if (heat + tempheat < 3)
        {
            //沒有溫度
            //Destroy(this);
        }
        else
        {
            heat += tempheat;
        }
    }
    /// <summary>
    /// 更新此物件的溫度，影響大小
    /// </summary>
    public void ApplyTemperature()
    {
        Debug.Log(heat);
        Nv.area = heat;
        Nv.size = Vector3.one * size;
    }
}