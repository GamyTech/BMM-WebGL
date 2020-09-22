using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/AppInformationData")]
public class AppInformationData : ScriptableObject
{
    public Enums.GameID gameId;
    public string companyName;
    public string bundleId;
    public Enums.MatchKind matchKind;
    public string version;
    public Sprite logo;
    public Sprite icon;
    public int ReleasedVersion;
    public string BundleVersion;
    public string SigningAppleTeam;
    public string keyStoreName;
    public string keyStorePassword;
    public string keyStoreAlias;
    public string facebookAppID;
}
