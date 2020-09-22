using SP.Dto.ProcessBreezeRequests;
using UnityEngine;
using UnityEngine.UI;

public class CountryCodeListItem : MonoBehaviour
{
    public delegate void SelectCountry(ISO3166Country item, int codeId);
    public SelectCountry OnSelect = (i, j) => { };

    public Text CodeLabel;
    public Text CheckIcon;

    private ISO3166Country item;
    private int codeId;

    public void Populate(ISO3166Country item, bool isSelected, int codeId = 0)
    {
        this.item = item;        
        this.codeId = codeId;        

        CodeLabel.text = GetCountryCodeName(item, codeId);
        CheckIcon.enabled = isSelected;
    }

    public void SetSelected()
    {
        CheckIcon.enabled = true;
    }

    public void Select()
    {
        OnSelect(item, codeId);
    }

    public static string GetCountryCodeName(ISO3166Country country, int codeId)
    {
        int index = country.Name.IndexOf("(");
        string name = index < 0 ? country.Name : country.Name.Substring(0, index);
        if (name.Length > 37) name = name.Substring(0, 34) + "...";
        return name + " +" + country.DialCodes[codeId];
    }
}
