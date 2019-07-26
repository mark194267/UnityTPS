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

    public int hot { get { return _hot; } }

    public int setHot { set { ChangeTemperature(value); } }

    private int _hot { get; set; }

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
    private void ChangeTemperature(int tempheat)
    {
        //Debug.Log("NowHeat: "+heat);
        //檢查是否還能更熱
        if (tempheat > 32)
        {
            _hot = 32;
        }
        else if (tempheat < 3)
        {
            //沒有溫度
            //Destroy(this);
        }
        else
        {
            _hot = tempheat;
        }
        ApplyTemperature();
    }
    /// <summary>
    /// 更新此物件的溫度，影響大小
    /// </summary>
    private void ApplyTemperature()
    {
        Debug.Log(_hot);
        Nv.area = _hot;
        Nv.size = Vector3.one * size;
        //transform.sc = Nv.size;
    }
    public void DeleteMe()
    {
        Destroy(this.gameObject);
    }
}