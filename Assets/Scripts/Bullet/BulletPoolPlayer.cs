using UnityEngine;
using System.Collections.Generic;

public class BulletPoolPlayer : MonoBehaviour
{
    public static BulletPoolPlayer Instance;

    [Header("Pool Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private bool allowPoolExpansion = true;

    private List<GameObject> bullets;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        bullets = new List<GameObject>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private GameObject CreateNewBullet()
    {
        GameObject b = Instantiate(bulletPrefab);
        b.SetActive(false);
        bullets.Add(b);
        return b;
    }

    public GameObject GetBullet()
    {
        foreach (GameObject b in bullets)
        {
            if (!b.activeInHierarchy)
                return b;
        }

        //if no bullets then allow expansion
        if (allowPoolExpansion)
        {
            GameObject newBullet = CreateNewBullet();
            return newBullet;
        }

        //if cant expand
        return null;
    }
}
