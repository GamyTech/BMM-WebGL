using UnityEngine;
using GT.Assets;

public class MascotView : MonoBehaviour
{
    public TouchInputHandler touchInputHandler;
    public bool isPlayer;
    public GameObject mascotPerfab;
    public Animator mascotAnimator;
    public string TestIdToLoad;

    private void Start()
    {
        touchInputHandler.Init(GetMouseDown);
    }

    public void Init(string Id)
    {
        if (!string.IsNullOrEmpty(Id))
            LoadMascotData(Id);
    }

    public void LoadMascotData(string Id)
    {
        RemoveMascot();
        MascotItemData mascotData = AssetController.Instance.GetStoreAsset(Id) as MascotItemData;
        if(mascotData != null)
        {
            mascotPerfab = Instantiate(mascotData.prefab);
            mascotPerfab.InitGameObjectAfterInstantiation(transform);
            GameObject Mascot = mascotPerfab.gameObject;
            mascotAnimator = Mascot.GetComponent<Animator>();
        }
    }

    private void Reset()
    {
        RemoveMascot();
    }

    public void RemoveMascot()
    {
        if (mascotPerfab != null)
        {
            Destroy(mascotPerfab);
            mascotAnimator = null;
            mascotPerfab = null;
        }
    }

    public void GetMouseDown(Vector2 mousePos)
    {
        if(mascotAnimator != null)
            mascotAnimator.SetTrigger("Trigger");
    }

    #region Testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPlayer)
            {
                Reset();
                Init(TestIdToLoad);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
           if (isPlayer)
            {
                Reset();
            }
        }
    }
    #endregion Testing
}
