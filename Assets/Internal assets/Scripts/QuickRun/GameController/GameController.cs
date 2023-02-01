using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        GameObject player = Instantiate(_playerPrefab, new Vector3(1f, 0, 1f), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }
    }
}