using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class GameManager : MonoBehaviour
{
    public Transform spawnMain;
    public Transform spawnGame;
    public EventManager startGame;
    public Button play;
    public GameObject cursoreHideButton;
    public SkinSystem skinSystem;
    public GemPets pets;
    private void Start()
    {
        if (DeviceChecker.IsMobile == true)
            cursoreHideButton.SetActive(false);
        else cursoreHideButton.SetActive(true);

        skinSystem.InitializeSkin();
        pets.InitializePet();
    }

    public void Lobby()
    {
        StartCoroutine(Delay(0));
        YG2.InterstitialAdvShow();
    }

    public void DieLobby()
    {
        StartCoroutine(Delay(4));
    }

    public void StartGame()
    {
        play.gameObject.SetActive(false);
        RobloxStyleController.instance.gameObject.transform.position = spawnGame.position;
        RobloxStyleController.instance.gameObject.transform.rotation = Quaternion.identity;
        startGame.StartRound();
    }

    private IEnumerator Delay(int sec)
    {
        yield return new WaitForSeconds(sec);
        RobloxStyleController.instance.gameObject.transform.position = spawnMain.position;
        RobloxStyleController.instance.gameObject.transform.rotation = Quaternion.identity;
        RobloxStyleController.instance.RespawnPlayer();
        PlayerStats.instance.Save();
        play.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Health.onPlayerDied += DieLobby;
    }

    private void OnDisable()
    {
        Health.onPlayerDied -= DieLobby;
    }
}
