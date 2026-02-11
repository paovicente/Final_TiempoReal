using UnityEngine;

public class MenuOptions : MonoBehaviour
{
    public void OpenOptions()
    {
        LevelManager.instance.ShowOptionsPanel();
    }
}
