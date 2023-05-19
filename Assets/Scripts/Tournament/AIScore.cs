using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIScore : MonoBehaviour
{
    public static AIScore instance;

    [field: Header("AI Score Calculations")]
    [field: SerializeField] float scoreCalculationInterval = 1f;
    [field: SerializeField][field: Range(0f, 1f)] float playerWinPercentage;
    [field: SerializeField][field: Range(0f, 1f)] float percentageOfHittingShot;
    [field: SerializeField][field: Range(0f, 1f)] float percentageofHittingDirectShot;
    [field: SerializeField] TextMeshProUGUI AIScoreDisplay;
    [field: SerializeField] TextMeshProUGUI winPercentage;
    public int opponentScore = 0;
    public Action OnGameStart;

    private bool hasCoroutineStarted = false;
    private float timerRandomized;
    private bool firstTimeIgnored = false;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AIScoreDisplay.text = opponentScore.ToString();
        float willPlayerWin = UnityEngine.Random.Range(0f, 1f);
        if (willPlayerWin >= 0f && willPlayerWin <= playerWinPercentage)
        {
            percentageOfHittingShot = UnityEngine.Random.Range(0.1f, 0.5f);
            percentageofHittingDirectShot = UnityEngine.Random.Range(0.1f, 0.2f);
        }
        else
        {
            percentageOfHittingShot = UnityEngine.Random.Range(0.4f, 1f);
            percentageofHittingDirectShot = UnityEngine.Random.Range(0.2f, 0.5f);
        }

        float AIWinPercentage = ((percentageOfHittingShot + percentageofHittingDirectShot) / 2) * 100;
        winPercentage.text = AIWinPercentage.ToString(); //For Debugging Purposes

        OnGameStart += () => { if (!hasCoroutineStarted) { StartCoroutine(ScoreCalculator()); hasCoroutineStarted = true; } };
        GameManager.instance.onGameOver += () => { StopCoroutine(ScoreCalculator()); hasCoroutineStarted = false; firstTimeIgnored = false; };
    }

    private IEnumerator ScoreCalculator()
    {
        while (!GameManager.instance.isGameOver)
        {
            if (firstTimeIgnored)
            {
                AIScoringChances();
                AIScoreDisplay.text = opponentScore.ToString();
                timerRandomized = UnityEngine.Random.Range((scoreCalculationInterval - 0.5f), (scoreCalculationInterval + 0.5f));
            }
            else
            {
                firstTimeIgnored = true;
            }
            yield return new WaitForSeconds(timerRandomized);
        }
    }

    private void AIScoringChances()
    {
        float scoringChance = UnityEngine.Random.Range(0f, 1f);
        float threePointerChance = UnityEngine.Random.Range(0f, 1f);
        if(scoringChance >=0 && scoringChance <= percentageOfHittingShot && !GameManager.instance.isGameOver)
        {
            if(threePointerChance >= 0 && threePointerChance <= percentageofHittingDirectShot && !GameManager.instance.isGameOver)
            {
                opponentScore += 3;
            }
            else if (threePointerChance > percentageofHittingDirectShot && !GameManager.instance.isGameOver)
            {
                opponentScore += 2;
            }
        }
    }
}