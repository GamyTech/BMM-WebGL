using UnityEngine;
using GT.Collections;

namespace GT.Assets
{
    public abstract class CategoriesData : ScriptableObject
    {
        public abstract string DefaultPath { get; }

        [SerializeField]
        public LocalDictionary CategoriesDictionary;

        [System.Serializable()]
        public class LocalDictionary : SerializableDictionaryBase<string, string>
        {

        }

        protected abstract string[] GetKeys();

        public void AddMissingKeys()
        {
            string[] keys = GetKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                if (CategoriesDictionary.ContainsKey(keys[i]) == false)
                {
                    Debug.Log("Adding Key " + keys[i]);
                    CategoriesDictionary.Add(keys[i], DefaultPath);
                }
            }
        }

        public void CleanObsoleteKeys()
        {
            string[] keys = GetKeys();
            LocalDictionary catDict = new LocalDictionary();
            for(int x = 0; x < keys.Length; ++x)
                if(CategoriesDictionary.ContainsKey(keys[x]))
                    catDict.Add(keys[x], CategoriesDictionary[keys[x]]);
            CategoriesDictionary = catDict;
        }
    }
}
