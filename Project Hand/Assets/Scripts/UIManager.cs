using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text scoreText, hiscoreText;
    public RectTransform healthBar;
    public EnemySpawner spawner;
    public Player player;

    private int score, hiscore;

    void Start () {
        score = 0;

        if (PlayerPrefs.HasKey("hiscore")) {
            hiscore = PlayerPrefs.GetInt("hiscore");
        }
        else
            hiscore = 0;

        scoreText.text = score.ToString();
        hiscoreText.text = hiscore.ToString();
        updateHp(player.maxHp);
	}
	
	public void addScore(int points) {
        score += points;
        scoreText.text = score.ToString();

        if(score > hiscore) {
            hiscore = score;
            hiscoreText.text = hiscore.ToString();
            PlayerPrefs.SetInt("hiscore", hiscore);
        }

        if(score % 5 == 0) {
            spawner.decrementFrequency();
        }
    }

    public void updateHp(int hp) {
        healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 16 * hp);

        if(hp < 1) {
            player.SendMessage("Die");
            Invoke("resetGame", 1.5f);
        }
    }

    private void resetGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }
}
