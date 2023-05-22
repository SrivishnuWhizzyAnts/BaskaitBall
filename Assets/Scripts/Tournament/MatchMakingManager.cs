using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;

public class MatchMakingManager : MonoBehaviour
{
    [field: Header("Tournament Bracket Generator")]
    [field: SerializeField] private TextMeshProUGUI playerText;
    [field: SerializeField] private TextMeshProUGUI opponentText;
    [field: SerializeField] private TextMeshProUGUI currentTourneyReward;
    [field: SerializeField] private TextMeshProUGUI roundTitle;
    [field: SerializeField] private TextMeshProUGUI timerText;
    [field: SerializeField] private int maxTimeToSearchForPlayer = 10;
    [field: SerializeField] private int timeToGoToGame = 5;
    [field: SerializeField] private string firstRoundName = "Entry Round";
    [field: SerializeField] private string secondRoundName = "Finals";
    [field: SerializeField] private string mainGameSceneName = "TournamentMode";

    private int playerSearchingTime;
    private float timer;
    private bool startTimer = false;

    private int playersInvestingCoins;
    private float tempPlayersInvestingCoins;
    private int tournamentReward;
    private float tempTournamentReward;


    private float coinReductionRate;
    private bool startCoinChange = false;

    private void Start()
    {
        playerSearchingTime = Random.Range(3, maxTimeToSearchForPlayer);
        SetValues();
        if(UserDataHandler.instance.ReturnSavedValues().firstRound && !UserDataHandler.instance.ReturnSavedValues().secondRound)
        {
            timerText.text = "Searching...";
        }
        else if (UserDataHandler.instance.ReturnSavedValues().firstRound && UserDataHandler.instance.ReturnSavedValues().secondRound)
        {
            timerText.text = "Starting...";
        }
        currentTourneyReward.text = "0";
        timer = timeToGoToGame;
    }

    private void Update()
    {
        if(startTimer)
        {
            timer -= Time.deltaTime;
            timerText.text = "Start in " + Mathf.RoundToInt(timer) + "...";
            if (timer <= 0)
            {
                timerText.text = "Loading...";
            }
        }

        if(startCoinChange)
        {
            tempPlayersInvestingCoins -= (Time.deltaTime * coinReductionRate * 2);
            tempTournamentReward += (Time.deltaTime * (coinReductionRate * 4));

            if (tempPlayersInvestingCoins >= 0 && tempTournamentReward <= tournamentReward)
            {
                playerText.text = Mathf.RoundToInt(tempPlayersInvestingCoins).ToString();
                opponentText.text = Mathf.RoundToInt(tempPlayersInvestingCoins).ToString();
                currentTourneyReward.text = Mathf.RoundToInt(tempTournamentReward).ToString();
            }
            else if(tempPlayersInvestingCoins < 0 && tempTournamentReward > tournamentReward)
            {
                playerText.text = "0";
                opponentText.text = "0";
                currentTourneyReward.text = tournamentReward.ToString();
                GoToGameScene();
                startCoinChange = false;
            }
        }
    }

    private void SetValues()
    {
        if(!UserDataHandler.instance.ReturnSavedValues().firstRound)
        {
            UserDataHandler.instance.ReturnSavedValues().firstRound = true;
            UserDataHandler.instance.SaveUserData();
            playerText.text = UserDataHandler.instance.ReturnSavedValues().userName;
            roundTitle.text = firstRoundName;
            opponentText.text = "???";
            SetOpponentUsernameFirstRound();
        }
        else if(UserDataHandler.instance.ReturnSavedValues().firstRound && !UserDataHandler.instance.ReturnSavedValues().secondRound)
        {
            UserDataHandler.instance.ReturnSavedValues().secondRound = true;
            UserDataHandler.instance.SaveUserData();
            playerText.text = UserDataHandler.instance.ReturnSavedValues().userName;
            roundTitle.text = secondRoundName;
            opponentText.text = AINamesGenerator.Utils.GetRandomName();
            GoToCoinAnimation();
        }
        else
        {
            UserDataHandler.instance.ReturnSavedValues().firstRound = false;
            UserDataHandler.instance.ReturnSavedValues().secondRound = false;
            UserDataHandler.instance.SaveUserData();
            return;
        }
    }
    private void AnimateCoins()
    {
        int selectedTournamentID = 0;
        for(int i = 0; i <=3; i++)
        {
            if (TournamentInfoDataHandler.instance.ReturnSavedValues().selected[i] == true)
            {
                selectedTournamentID = i; 
                break;
            }
        }
        if (UserDataHandler.instance.ReturnSavedValues().firstRound == true && UserDataHandler.instance.ReturnSavedValues().secondRound == false)
        {
            playersInvestingCoins = TournamentInfoDataHandler.instance.ReturnSavedValues().prices[selectedTournamentID];
            coinReductionRate = Mathf.Pow(10,((TournamentInfoDataHandler.instance.ReturnSavedValues().prices[selectedTournamentID] * 4).ToString().Length-2));
            playerText.text = playersInvestingCoins.ToString();
            opponentText.text = playersInvestingCoins.ToString();
            tournamentReward = playersInvestingCoins * 2;
            tempPlayersInvestingCoins = playersInvestingCoins;
            tempTournamentReward = 0;
            currentTourneyReward.text = "0";
            StartAnimatingCoins();
        }
        else if(UserDataHandler.instance.ReturnSavedValues().firstRound == true && UserDataHandler.instance.ReturnSavedValues().secondRound == true)
        {
            playersInvestingCoins = (TournamentInfoDataHandler.instance.ReturnSavedValues().prices[selectedTournamentID] * 2);
            coinReductionRate = Mathf.Pow(10, ((TournamentInfoDataHandler.instance.ReturnSavedValues().prices[selectedTournamentID] * 4).ToString().Length - 2));
            playerText.text = playersInvestingCoins.ToString();
            opponentText.text = playersInvestingCoins.ToString();
            tournamentReward = playersInvestingCoins * 2;
            tempPlayersInvestingCoins = playersInvestingCoins;
            tempTournamentReward = 0;
            currentTourneyReward.text = "0";
            StartAnimatingCoins();
        }
    }
    private async void SetOpponentUsernameFirstRound()
    {
        await Task.Delay(playerSearchingTime * 1000);
        opponentText.text = AINamesGenerator.Utils.GetRandomName();
        timerText.text = "Starting...";
        GoToCoinAnimation();
    }
    private async void GoToGameScene()
    {
        startTimer = true;
        await Task.Delay(timeToGoToGame * 1000);
        startTimer = false;
        SceneManager.LoadScene(mainGameSceneName);
    }
    private async void StartAnimatingCoins()
    {
        await Task.Delay(2000);
        startCoinChange = true;
    }

    private async void GoToCoinAnimation()
    {
        await Task.Delay(1000);
        AnimateCoins();
    }
}
