using UnityEngine;

public class ActivateDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject door;
    [SerializeField] GameObject enemyBoss;

    private void Update()
    {
        ShowDoor();
    }

    private void ShowDoor()
    {
        if (enemyBoss == null)
            door.SetActive(true);
    }

}
