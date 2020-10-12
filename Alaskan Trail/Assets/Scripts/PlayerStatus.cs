using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour {

	public GameObject healthBar;
	public GameObject hungerBar;
	public GameObject thirstBar;

	public Text healthText;
	public Text hungerText;
	public Text thirstText;

    public void OpenMenu () {
    	gameObject.SetActive(true);

    	UpdateStats();
    }

    public void CloseMenu () {
    	gameObject.SetActive(false);
    }

    public void UpdateHealth () {
    	if (!gameObject.activeSelf) {
    		return;
    	}

    	PlayerController player = Game.instance.player;

    	healthBar.transform.localScale = new Vector3(player.health / (player.maxHealth * 1.0f), 1, 1);
    	healthText.text = "Health " + Mathf.Floor(player.health + 0.5f) + " / " + player.maxHealth;
    }

    public void UpdateHunger () {
    	if (!gameObject.activeSelf) {
    		return;
    	}

    	PlayerController player = Game.instance.player;

    	hungerBar.transform.localScale = new Vector3(player.hunger / (player.maxHunger * 1.0f), 1, 1);
    	hungerText.text = "Hunger " + Mathf.Floor(player.hunger + 0.5f) + " / " + player.maxHunger;
    }

    public void UpdateThirst () {
    	if (!gameObject.activeSelf) {
    		return;
    	}

    	PlayerController player = Game.instance.player;

    	thirstBar.transform.localScale = new Vector3(player.thirst / (player.maxThirst * 1.0f), 1, 1);
    	thirstText.text = "Thirst " + Mathf.Floor(player.thirst + 0.5f) + " / " + player.maxThirst;
    }

    private void UpdateStats () {
    	PlayerController player = Game.instance.player;

    	healthBar.transform.localScale = new Vector3(player.health / (player.maxHealth * 1.0f), 1, 1);
    	hungerBar.transform.localScale = new Vector3(player.hunger / (player.maxHunger * 1.0f), 1, 1);
    	thirstBar.transform.localScale = new Vector3(player.thirst / (player.maxThirst * 1.0f), 1, 1);

    	healthText.text = "Health " + Mathf.Floor(player.health + 0.5f) + " / " + player.maxHealth;
    	hungerText.text = "Hunger " + Mathf.Floor(player.hunger + 0.5f) + " / " + player.maxHunger;
    	thirstText.text = "Thirst " + player.thirst + " / " + player.maxThirst;
    }
}
