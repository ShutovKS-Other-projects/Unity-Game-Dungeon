using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject SkyBoxNight;
    [SerializeField] GameObject PlayerPrefab;
    private GameObject _player;

    private void Awake()
    {
        Instantiate(SkyBoxNight);
        _player = Instantiate(PlayerPrefab, new Vector3(1f, 0, 1f), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }
    }
}