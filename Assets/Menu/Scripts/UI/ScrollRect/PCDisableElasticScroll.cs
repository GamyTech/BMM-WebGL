using UnityEngine;
using UnityEngine.UI;

public class PCDisableElasticScroll : MonoBehaviour {

	void Start ()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        this.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
#endif
	}
}
