using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace GT.Backgammon.View
{
    public class ButtonsView : MonoBehaviour
    {
        private ObjectPool m_pool;
        public ObjectPool pool
        {
            get
            {
                if (m_pool == null)
                    m_pool = GetComponent<ObjectPool>();
                return m_pool;
            }
        }

        private Dictionary<string, GameObject> activeButtons = new Dictionary<string, GameObject>();

        public void ActivateButtons(params GameControlButton[] buttonsToActivate)
        {
            for (int i = 0; i < buttonsToActivate.Length; i++)
            {
                GameControlButton current = buttonsToActivate[i];
                if (activeButtons.ContainsKey(current.id))
                {
                    Button button = activeButtons[current.id].GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(current.action);
                }
                else
                {
                    GameObject go = pool.GetObjectFromPool();
                    go.InitGameObjectAfterInstantiation(transform);
                    go.GetComponentInChildren<Text>().text = current.text;
                    go.GetComponent<Button>().onClick.AddListener(current.action);
                    activeButtons.Add(current.id, go);
                }
            }
        }

        public void DeactivateButtons(params string[] ids)
        {
            GameObject go;
            for (int i = 0; i < ids.Length; i++)
            {
                if(activeButtons.TryGetValue(ids[i], out go))
                {
                    pool.PoolObject(go);
                    activeButtons.Remove(ids[i]);
                }
            }
        }

        public void DeactivateAllButtons()
        {
            foreach (var button in activeButtons)
            {
                pool.PoolObject(button.Value);
            }
            activeButtons.Clear();
        }
    }


    public class GameControlButton
    {
        public string id;
        public string text;
        public UnityAction action;

        public GameControlButton(string id, string text, UnityAction action = null)
        {
            this.id = id;
            this.text = text;
            this.action = action;
        }
    }
}
