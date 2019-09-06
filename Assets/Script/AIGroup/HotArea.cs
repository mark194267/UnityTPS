using Assets.Script.AIGroup;
using UnityEngine;
using UnityEngine.AI;

public class HotArea : MonoBehaviour {

    public HotAreaManager hotAreaManager { get; set; }
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

    private int _maxhot = 13;
    private int _normalhot = 10;
    private int _minhot = 7;

    public int hot { get { return _hot; } }

    public int setHot { set { ChangeTemperature(value); } }

    private int _hot { get; set; }

    private float existTime;
    public NavMeshModifierVolume Nv { get; set; }

    public float cooldown = 5f;
    private float _timer;

    private void OnEnable() 
    {
        Nv = GetComponent<NavMeshModifierVolume>();
    }
    private void Update()
    {
        //變冷
        _timer += Time.deltaTime*1f;
        if (_timer > cooldown)
        {
            //時間到.降溫
            if (hot > _normalhot) setHot = hot - 1;
            else if (hot < _normalhot) setHot = hot + 1;
            else
            {
                hotAreaManager.DeleteHotArea(this);
            }
        }
    }

    /// <summary>
    /// *需套用ApplyTemperature才有效果，改變此物件的溫度
    /// </summary>
    /// <param name="heat"></param>
    private void ChangeTemperature(int tempheat)
    {
        //Debug.Log("NowHeat: "+heat);
        //檢查是否還能更熱
        if (tempheat > _maxhot)
        {
            _hot = _maxhot;
        }
        else if (tempheat < _minhot)
        {
            _hot = _minhot;
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
        //Debug.Log(_hot);
        Nv.area = _hot;
        Nv.size = Vector3.one * size;
        _timer = 0;
        //transform.sc = Nv.size;
    }
    public void DeleteMe()
    {
        Destroy(this.gameObject);
    }
}