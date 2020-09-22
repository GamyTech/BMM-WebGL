using UnityEngine;

public class DoubleCubeView : MonoBehaviour {

    public TextMesh ValueLabel;
    public Animator animator;

    private Vector3 upVector = new Vector3(-4.2f,2.35f,0);
    private Vector3 startVector = new Vector3(-4.3f, 0.1f, 0);
    private Vector3 downVector = new Vector3(-4.42f,-2.15f,0);
    

    private bool OnBottom;
    private bool HasMoved;

    public void MoveDoubleDice(bool fromBottomPlayer, int cubeNum)
    {
        if (fromBottomPlayer)
            MoveDoubleDiceToTopPlayer(cubeNum);
        else
            MoveDoubleDiceToBottomPlayer(cubeNum);
    }

    public void Reset()
    {
        transform.position = startVector;
        animator.SetBool("IsOn", true);
        ValueLabel.text = "64";
        HasMoved = false;
    }

    private void AnimationStarted()
    {
        transform.position = OnBottom ? downVector : upVector;
        animator.SetBool("IsOn", true);
    }

    private void AnimationFinished() {}

    private void MoveDoubleDiceToBottomPlayer(int cubeNum)
    {
        if(!HasMoved || !OnBottom)
        {
            animator.SetBool("IsOn", false);
            ValueLabel.text = cubeNum.ToString();
            OnBottom = true;
            HasMoved = true;
        }
    }

    private void MoveDoubleDiceToTopPlayer(int cubeNum)
    {
        if(!HasMoved || OnBottom)
        {
            animator.SetBool("IsOn", false);
            ValueLabel.text = cubeNum.ToString();
            OnBottom = false;
            HasMoved = true;
        }
    }

}
