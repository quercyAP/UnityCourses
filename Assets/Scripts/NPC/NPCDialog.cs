using System.Collections;
using TMPro;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public int NPCID;
    public int NPCState;
    public string[] StateDialogs;
    public float DialogueDuration = 5f;
    private TextMeshProUGUI Text;
    private bool dialogDisplayed = false;

    void Start()
    {
        Text = GetComponentInChildren<TextMeshProUGUI>();
        Text.text = "";
        Text.enabled = false;
        SwordSoul.Events.SetNPCState += Events_SetNPCState;
    }

    private void Events_SetNPCState(int npcID, int StateID)
    {
        if (NPCID == npcID)
        {
            NPCState = StateID;
            if(dialogDisplayed)
                Text.text = StateDialogs[NPCState];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // we hit player
        if(other.gameObject == SwordSoul.GameManager.Player.gameObject && !dialogDisplayed)
           StartCoroutine(ShowDialog(StateDialogs[NPCState]));
    }

    public IEnumerator ShowDialog(string dialog)
    {
        dialogDisplayed = true;
        Text.enabled = true;
        Text.text = dialog;
        yield return new WaitForSeconds(DialogueDuration);
        Text.enabled = false;
        dialogDisplayed = false;
    }
}