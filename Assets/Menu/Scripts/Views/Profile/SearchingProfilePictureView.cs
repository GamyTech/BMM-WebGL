using UnityEngine.UI;

public class SearchingProfilePictureView : ProfilePictureView
{
    public Text nameText;

    public void SetTexts(string name)
    {
        nameText.text = name;
    }
}
