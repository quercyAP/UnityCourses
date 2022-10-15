using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CharacterCollission : MonoBehaviour
{
    public GameObject dial;
    public GameObject dialTxt;
    public GameObject sword;
    public GameObject obstacle1;
    [HideInInspector]
    public float life = 0;
    public Slider lifeBar;
    public float lifeBase = 10;
    private void Awake()
    {
        life = lifeBase;
    }

    private void OnTriggerEnter(Collider other)
    {

        // déclencher le dialogue 1
        if (other.gameObject.name == "TriggerDialogue1")
        {
            // si on deja l'epee
            if (sword.activeInHierarchy)
            {
                // on detruit l'obstacle
                Destroy(obstacle1);
            }
            else
            {
                dial.SetActive(true);
                dialTxt.GetComponent<TextMeshProUGUI>().text = "Coucou bg, ramène toi avec une épée si tu veux partir !";
                StartCoroutine("HideDial");
            }
        }
        // drop de l'epée
        if (other.gameObject.name == "sword_pickup")
        {
            Destroy(other.gameObject);
            sword.SetActive(true);
        }
        // dialogue 2
        if (other.gameObject.name == "TriggerDialogue2" && obstacle1 == null)
        {
            dial.SetActive(true);
            dialTxt.GetComponent<TextMeshProUGUI>().text = "Va poutrer des mobs maintenant.";
            Destroy(other.gameObject);
            StartCoroutine("HideDial");
        }
        // drop des potion
        if (other.gameObject.name == "FioleLoot")
        {
            Destroy(other.gameObject);
        }
        // attaque sur les mobs
        if (other.gameObject.tag == "Mob")
        {
            takedamage(other.gameObject.GetComponent<MonsterAi>());
        }
    }

    void takedamage(MonsterAi monster)
    {
        life -= monster.Damage;
        monster.col.enabled = false;
        lifeBar.DOValue(life / lifeBase, 0.5f);
        if (life <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Camera.main.DOShakeRotation(0.18f, 8, 5, 8);
        }
    }

    IEnumerator HideDial()
    {
        yield return new WaitForSeconds(4);
        dial.SetActive(false);
    }
}
