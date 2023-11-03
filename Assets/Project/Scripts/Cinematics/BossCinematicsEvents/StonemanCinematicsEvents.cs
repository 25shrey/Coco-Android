using Game.BaseFramework;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class StonemanCinematicsEvents : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject cinematicsContainer;
    [SerializeField] private StonemanEnemy stonemanEnemy;
    [SerializeField] private Collider endCollider;

    [Header("Player Movement Parameters")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Player player;

    public void OnEndCinematicsStart()
    {
        stonemanEnemy.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        cinematicsContainer.SetActive(true);
        GameManager.instance.input.DisableInput();
        MenuInput.DisableInput();
    }

    public async void OnEndCinematicsCompleted()
    {
        endCollider.gameObject.SetActive(true);
        player.transform.position = spawnPoint.position;
        player.gameObject.SetActive(true);
        GameManager.instance.Player.playerPowerUps.DeactivateAllPower();
        var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        game.RemoveBossHealthBar();
        await Task.Delay(5000);
        GameManager.instance.OnCinematicsShowCloudEnd();
        cinematicsContainer.SetActive(false);
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }

    public void MovePlayerTowardsEnd()
    {
        player.transform.position = startPos.position;
        player.gameObject.SetActive(true);
        StartCoroutine(GameManager.instance.MovePlayerToParticularPoint(player.transform, endPos.position,
            moveSpeed, rotationSpeed,
            () =>
            {
                player.gameObject.SetActive(false);
                GameManager.instance.OnCinematicsShowCloud();
            }, 3f));
    }
}
