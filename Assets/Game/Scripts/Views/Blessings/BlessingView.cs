using UnityEngine;
using System.Collections;
using GT.Assets;

public class BlessingView : MonoBehaviour
{
    public Animator animator;
    public string PlayerBlessing;
    public string OpponentBlessing;
    private GameObject blessingObject;

    public void SetBlessingId(bool localPlayer, string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return;

        if (localPlayer)
            PlayerBlessing = itemID;
        else
            OpponentBlessing = itemID;
    }

    public IEnumerator ShowBlessing(bool isPlayerBlessing)
    {
        string Blessing = isPlayerBlessing ? PlayerBlessing : OpponentBlessing;
        if (!string.IsNullOrEmpty(Blessing))
        {
            LoadBlessing(Blessing);

            animator.SetBool("IsOpen", true);
            yield return new WaitForSeconds(2);
            animator.SetBool("IsOpen", false);
            yield return new WaitForSeconds(1);

            UnloadBlessing(true, isPlayerBlessing);
        }
    }

    private void LoadBlessing(string Id)
    {
        BlessingItemData blessingData = AssetController.Instance.GetStoreAsset(Id) as BlessingItemData;
        if(blessingData != null)
        {
            blessingObject = Instantiate(blessingData.prefab);
            blessingObject.InitGameObjectAfterInstantiation(transform);
        }
    }

    private void UnloadBlessing(bool clearID, bool isLocal)
    {
        if (blessingObject != null)
            Destroy(blessingObject);
        if (clearID)
        {
            if (isLocal)
            {
                PlayerBlessing = string.Empty;
                GT.Store.Store blessingsStore = UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Blessings);
                if (blessingsStore != null)
                {
                    GT.Store.Selected currentBlessingSelect = blessingsStore.selected;
                    GT.Store.StoreItem currentBlessingItem = currentBlessingSelect.GetFirstSelected();

                    if (currentBlessingItem != null)
                    {
                        currentBlessingSelect.SelectItem(false, currentBlessingItem);
                        if (currentBlessingItem is GT.Store.ConsumableItem)
                            (currentBlessingItem as GT.Store.ConsumableItem).ConsumeItem();
                    }
                }
            }
            else
                OpponentBlessing = string.Empty;
        }
    }
}
