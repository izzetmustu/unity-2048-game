using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOverGroup;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    private int score;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        bestText.text = LoadHighScore().ToString();

        gameOverGroup.alpha = 0;
        gameOverGroup.interactable = false;

        board.Clear();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOverGroup.interactable = true;

        StartCoroutine(Fade(gameOverGroup, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float end, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0;
        float duration = 0.5f;
        float start = canvasGroup.alpha;

        while(elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    public void IncreaseScore(int value)
    {
        SetScore(score + value);
    }

    private void SetScore(int value)
    {
        score = value;
        scoreText.text = score.ToString();
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int hiscore = LoadHighScore();
        if(score > hiscore)
        {
            PlayerPrefs.SetInt("hiscore", score);
            bestText.text = score.ToString();
        }
    }

    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

}
