using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject myPlayer;

    bool upKeyDown;
    bool downKeyDown;

    public enum GameState { MAIN_MENU, SELECT_STAGE, GAME, OPTIONS };
    public GameState gameState;

    // Main Menu
    public GameObject pressStart;
    public bool splashScreen = true;
    float timeToSelect = 1.25f;
    float selectTimer = 0;

    public GameObject play;
    public GameObject options;
    public GameObject select;
    public float[] selectY;
    public int selection = 0;
    int maxSelect = 1;

    // Start is called before the first frame update
    void Start()
    {
        SetGameState(gameState);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (gameState)
        {
            case GameState.MAIN_MENU:

                if (splashScreen)
                {
                    if (Input.GetKey(KeyCode.Return))
                    {
                        pressStart.GetComponent<Animator>().SetTrigger("PressStart");
                        splashScreen = false;
                        selection = 0;
                        selectTimer = 0;
                    }
                }
                else
                {
                    if (selectTimer < timeToSelect)
                    {
                        selectTimer += Time.fixedDeltaTime;

                        if (selectTimer >= timeToSelect)
                        {
                            selectTimer = timeToSelect;

                            play.SetActive(true);
                            options.SetActive(true);
                            select.SetActive(true);
                        }
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.W))
                        {
                            if (!upKeyDown)
                            {
                                upKeyDown = true;
                                selection--;
                            }
                        }
                        else
                        {
                            upKeyDown = false;
                        }

                        if (Input.GetKey(KeyCode.S))
                        {
                            if (!downKeyDown)
                            {
                                downKeyDown = true;
                                selection++;
                            }
                        }
                        else
                        {
                            downKeyDown = false;
                        }

                        if (selection < 0) selection = maxSelect;
                        else if (selection > maxSelect) selection = 0;

                        select.transform.position = new Vector2(select.transform.position.x, selectY[selection]);

                        if (Input.GetKey(KeyCode.Return))
                        {
                            select.GetComponent<Animator>().SetTrigger("Select");

                            if (selection == 0) // Play
                            {
                                SetGameState(GameState.SELECT_STAGE);
                            }
                        }
                    }
                }


                break;

            case GameState.SELECT_STAGE:
                break;

            case GameState.GAME:
                break;
        }
    }

    public void SetGameState(GameState gS)
    {
        gameState = gS;

        switch (gS)
        {
            case GameState.MAIN_MENU:

                play.SetActive(false);
                options.SetActive(false);
                select.SetActive(false);

                break;

            case GameState.SELECT_STAGE:
                break;

            case GameState.GAME:
                if (myPlayer == null)
                {
                    myPlayer = GameObject.Instantiate(playerPrefab);
                    myPlayer.name = "Player";
                    GetComponent<CameraManager>().targetTransform = myPlayer.transform;
                }
                break;
        }
    }
}
