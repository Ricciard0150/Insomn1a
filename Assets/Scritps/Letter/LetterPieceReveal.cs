//using UnityEngine;

//public class LetterFinalReveal : MonoBehaviour
//{
//    [SerializeField] private GameObject letterPanel;      // painel da carta final
//    [SerializeField] private InterviewQuests dialogue;    // opcional: diálogo antes
//    [SerializeField] private string[] finalLines;

//    void Start()
//    {
//        if (LetterPieceManager.Instance != null)
//            LetterPieceManager.Instance.OnAllPiecesCollected += Reveal;
//    }

//    void OnDestroy()
//    {
//        if (LetterPieceManager.Instance != null)
//            LetterPieceManager.Instance.OnAllPiecesCollected -= Reveal;
//    }

//    private void Reveal()
//    {
//        if (dialogue != null && finalLines != null && finalLines.Length > 0)
//        {
//            dialogue.StartDialogue(finalLines);
//            StartCoroutine(ShowPanelAfterDialogue());
//        }
//        else
//        {
//            ShowPanel();
//        }
//    }

//    private System.Collections.IEnumerator ShowPanelAfterDialogue()
//    {
//        yield return new WaitWhile(() => dialogue.IsRunning);
//        ShowPanel();
//    }

//    private void ShowPanel()
//    {
//        if (letterPanel != null)
//            letterPanel.SetActive(true);
//    }
//}