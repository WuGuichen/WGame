using UnityEngine;

public class PreBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject splashScene;

    private void Awake()
    {
        BootstrapStart();
    }

    void BootstrapStart()
    {
        splashScene.SetActive(true);
        gameObject.SetActive(false);
    }
}
