using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;

    [SerializeField]
    private TextMeshProUGUI blackScoreText;

    [SerializeField]
    private TextMeshProUGUI whiteScoreText;

    [SerializeField]
    private TextMeshProUGUI winnerText;

    [SerializeField]
    private Image blackOverlay;

    [SerializeField]
    private RectTransform playAgainButton;

    [SerializeField]
    private RectTransform homeButton;

    [Header("-----------")]
    public GameManager gameManager;

    [Header("MainView")]
    public GameObject mainView;
    public RectTransform gray;
    public RectTransform label;
    public RectTransform mainGroup;
    public RectTransform onlineGroup;
    public RectTransform offlineGroup;
    public RectTransform offlinePvEDifficultGroup;
    public RectTransform offlinePvEGroup;

    [Header("InGameView")]
    public GameObject inGameView;
    public RectTransform pausePopup;
    public GameObject pauseRestartButton;
    public RectTransform disconnectPopup;

    private bool waitConnect = false;

    private void Start()
    {
        StartCoroutine(ShowMainView());
    }

    private void Update()
    {
        if (waitConnect)
        {
            if (gameManager.client.state == (int)OnlineState.BLACK)
            {
                waitConnect = false;
                gameManager.clientPlayer = Player.Black;
                gameManager.client.state = (int)OnlineState.WAIT;
                ShowInGameView();
                gameManager.StartGame();
            }
            else if (gameManager.client.state == (int)OnlineState.WHITE)
            {
                waitConnect = false;
                gameManager.clientPlayer = Player.White;
                gameManager.client.state = (int)OnlineState.WAIT;
                ShowInGameView();
                gameManager.StartGame();
            }
        }
        else if(gameManager.inGame && gameManager.mode == Mode.Online && gameManager.client.state == (int)OnlineState.DISCONNECT)
        {
            gameManager.inGame = false;
            gameManager.DestroyClient();
            StartCoroutine(OnDisconnect());
        }
    }

    #region mainview
    public IEnumerator ShowMainView()
    {
        waitConnect = false;

        inGameView.SetActive(false);
        mainView.SetActive(true);
        gray.LeanAlpha(0, 0);
        label.localScale = Vector3.zero;
        mainGroup.localScale = Vector3.zero;
        onlineGroup.localScale = Vector3.zero;
        offlineGroup.localScale = Vector3.zero;
        offlinePvEDifficultGroup.localScale = Vector3.zero;
        offlinePvEGroup.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.25f);

        gray.LeanAlpha(0.3f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        label.LeanScale(Vector3.one, 0.5f);
        mainGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void ShowInGameView()
    {
        mainView.SetActive(false);
        inGameView.SetActive(true);
    }

    public IEnumerator ToOnlineGroup()
    {
        mainGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        onlineGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);

        gameManager.CreatClient(true);
        waitConnect = true;
        yield return new WaitForSeconds(5f);
        if (waitConnect)
        {
            gameManager.CreatClient(false);
        }
    }

    public IEnumerator ToOfflineGroup()
    {
        mainGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        offlineGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator ToOfflinePvEDifficultGroup()
    {
        offlineGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        offlinePvEDifficultGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator BackFromOnlineGroup()
    {
        gameManager.DestroyClient();
        waitConnect = false;
        onlineGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        mainGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator BackFromOfflineGroup()
    {
        offlineGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        mainGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator ToOfflinePvEGroup()
    {
        offlinePvEDifficultGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        offlinePvEGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator BackFromOfflinePvEDifficultGroup()
    {
        offlinePvEDifficultGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        offlineGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator BackFromOfflinePvEGroup()
    {
        offlinePvEGroup.LeanScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);

        offlinePvEDifficultGroup.LeanScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    #endregion

    #region button mainView
    public void OnPlayOfflineButtonClick()
    {
        StartCoroutine(ToOfflineGroup());
    }

    public void OnPlayOnlineButtonClick()
    {
        gameManager.mode = Mode.Online;
        StartCoroutine(ToOnlineGroup());
    }

    public void OnPvPButtonClick()
    {
        gameManager.mode = Mode.PvP;
        ShowInGameView();
        gameManager.StartGame();
    }

    public void OnPvEButtonClick()
    {
        gameManager.mode = Mode.PvE;
        StartCoroutine(ToOfflinePvEDifficultGroup());
    }

    public void OnBackOnlineButtonClick()
    {
        StartCoroutine(BackFromOnlineGroup());
    }

    public void OnBackOfflineButtonClick()
    {
        StartCoroutine(BackFromOfflineGroup());
    }

    public void OnPvEDifficultClick(int iteration)
    {
        AIMonteCarlo.iteration = iteration;
        StartCoroutine(ToOfflinePvEGroup());
    }

    public void OnBackOfflinePvEDifficultClick()
    {
        StartCoroutine(BackFromOfflinePvEDifficultGroup());
    }

    public void OnPvEStartClick(bool goFirst)
    {
        gameManager.AIPlayer = goFirst ? Player.White : Player.Black;
        ShowInGameView();
        gameManager.StartGame();
    }

    public void OnBackOfflinePvEClick()
    {
        StartCoroutine(BackFromOfflinePvEGroup());
    }
    #endregion

    #region ingame
    public void SetPlayerText(Player currentPlayer)
    {
        if (!topText.gameObject.activeSelf) {
            topText.gameObject.SetActive(true);
            topText.rectTransform.localScale = Vector3.one;
        }
        string player = "", color = "";
        if (gameManager.mode == Mode.PvP)
        {
            if (currentPlayer == Player.Black)
            {
                player = "Black's";
            }
            else if (currentPlayer == Player.White)
            {
                player = "White's";
            }
        }
        else if (gameManager.mode == Mode.PvE)
        {
            if (currentPlayer == gameManager.AIPlayer)
            {
                player = "AI's";
            }
            else
            {
                player = "Your";
            }
        }
        else if (gameManager.mode == Mode.Online)
        {
            if (currentPlayer == gameManager.clientPlayer)
            {
                player = "Your";
            }
            else
            {
                player = "Opponent";
            }
        }

        if (currentPlayer == Player.Black)
        {
            color = "DiscBlackUp";
        }
        else if (currentPlayer == Player.White)
        {
            color = "DiscWhiteUp";
        }
        topText.text = player + " Turn <sprite name=" + color + ">";
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            topText.text = "Black Cannot Move! <sprite name=DiscBlackUp>";
        }
        else if (skippedPlayer == Player.White)
        {
            topText.text = "White Cannot Move! <sprite name=DiscWhiteUp>";
        }
    }

    public void SetTopText(string message)
    {
        topText.text = message;
    }

    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        blackScoreText.text = $"<sprite name=DiscBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private IEnumerator ShowOverlay()
    {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay()
    {
        blackOverlay.rectTransform.LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoresDown()
    {
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Player winner)
    {
        if (gameManager.mode == Mode.PvP)
        {
            switch (winner)
            {
                case Player.Black:
                    winnerText.text = "Black Won!";
                    break;
                case Player.White:
                    winnerText.text = "White Won!";
                    break;
                case Player.None:
                    winnerText.text = "It's a Tie!";
                    break;
            }
        }
        else if (gameManager.mode == Mode.PvE)
        {
            if (winner == Player.None)
            {
                winnerText.text = "It's a Tie!";
            }
            else if (winner == gameManager.AIPlayer)
            {
                winnerText.text = "You Lost!";
            }
            else
            {
                winnerText.text = "You Won!";
            }
        }
        else if (gameManager.mode == Mode.Online)
        {
            if (winner == Player.None)
            {
                winnerText.text = "It's a Tie!";
            }
            else if (winner == gameManager.clientPlayer)
            {
                winnerText.text = "You Won!";
            }
            else
            {
                winnerText.text = "You Lost!";
            }
        }
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoresDown();
        yield return ScaleUp(winnerText.rectTransform);
        if (gameManager.mode != Mode.Online) yield return ScaleUp(playAgainButton);
        yield return ScaleUp(homeButton);
    }

    public IEnumerator HideEndScreen()
    {
        StartCoroutine(ScaleDown(winnerText.rectTransform));
        StartCoroutine(ScaleDown(blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(playAgainButton));
        StartCoroutine(ScaleDown(homeButton));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }

    public IEnumerator InGameToHomeView()
    {
        yield return HideEndScreen();
        yield return ShowMainView();
    }

    public IEnumerator ContinueFromPause()
    {
        yield return ScaleDown(pausePopup);
        gameManager.inGame = true;
    }

    public IEnumerator RestartFromPause()
    {
        yield return ScaleDown(pausePopup);
        gameManager.StartGame();
    }

    public IEnumerator HomeFromPause()
    {
        if (gameManager.mode == Mode.Online)
        {
            gameManager.DestroyClient();
        }
        yield return ScaleDown(pausePopup);
        yield return ShowMainView();
    }

    public IEnumerator OnDisconnect()
    {
        yield return ScaleUp(disconnectPopup);
    }

    public IEnumerator HomeFromDisconnect()
    {
        yield return ScaleDown(disconnectPopup);
        yield return ShowMainView();
    }
    #endregion

    #region button InGameView
    public void OnHomeButtonClick()
    {
        StartCoroutine(InGameToHomeView());
    }

    public void OnPauseButtonClick()
    {
        gameManager.inGame = false;
        pauseRestartButton.SetActive(gameManager.mode != Mode.Online);
        StartCoroutine(ScaleUp(pausePopup));
    }

    public void POnContinueButtonClick()
    {
        //gameManager.inGame = true;
        StartCoroutine(ContinueFromPause());
    }

    public void POnRestartButtonClick()
    {
        StartCoroutine(RestartFromPause());
    }

    public void POnHomeButtonClick()
    {
        StartCoroutine(HomeFromPause());
    }

    public void DOnHomeButtonClick()
    {
        StartCoroutine(HomeFromDisconnect());
    }
    #endregion
}
