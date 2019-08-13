using UnityEngine;

/// <summary>
/// Gestor de ejecución del experimento.
/// </summary>
public class ExperimentManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject boardA, boardB;
    public GameObject chestA, chestB, chestCommon;
    public GameObject playerACursor, playerBCursor;

    public uint scoreA { get { return chestAScript.GetScore(); } }
    public uint scoreB { get { return chestBScript.GetScore(); } }
    public uint scoreCommon { get { return chestCommonScript.GetScore(); } }

    private GameManager gameManagerScript;
    private Vector2 playerACursorPos, playerBCursorPos;
    private BoardManager boardManagerA, boardManagerB;
    private ChestController chestAScript, chestBScript;
    private CommonChestController chestCommonScript;
    private Vector2 aiCursorPosition;
    private ManyCursorController cursorAScript, cursorBScript;

    private string path;
    private bool running;

    void Awake()
    {
        gameManagerScript = gameManager.GetComponent<GameManager>();
        boardManagerA = boardA.GetComponent<BoardManager>();
        boardManagerB = boardB.GetComponent<BoardManager>();
        cursorAScript = playerACursor.GetComponent<ManyCursorController>();
        cursorBScript = playerBCursor.GetComponent<ManyCursorController>();

        playerACursorPos = playerACursor.transform.position;
        playerBCursorPos = playerBCursor.transform.position;

        running = false;
    }

    void Update()
    {
        if (running)
        {
            // Termina experimento cuando se hayan recolectado todos los frutos.
            if (boardManagerA.CountFruits() == 0 && boardManagerB.CountFruits() == 0)
            {
                gameManagerScript.EndExperiment();
            }
        }
    }

    public void InitializeExperiment(
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB, bool commonCounter
        )
    {
        cursorAScript.speed = (int) speedA;
        cursorBScript.speed = (int) speedB;

        boardManagerA.SetUpExperiment(10, 10, playerAFruits);
        boardManagerB.SetUpExperiment(10, 10, playerBFruits);

        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();
        chestCommonScript = chestCommon.GetComponentInChildren<CommonChestController>();
        chestCommonScript.actAsCounter = commonCounter;

        chestAScript.SetToCapture(false);
        chestBScript.SetToCapture(false);

        running = true;
    }

    public void ActivateCursors()
    {
        playerACursor.transform.position = playerACursorPos;
        playerBCursor.transform.position = playerBCursorPos;

        playerACursor.SetActive(true);
        playerBCursor.SetActive(true);
    }

    public void DeactivateCursors()
    {
        if (playerACursor.activeSelf)
            playerACursor.SetActive(false);

        if (playerBCursor.activeSelf)
            playerBCursor.SetActive(false);
    }

    public void StopExperiment()
    {
        running = false;

        chestAScript.SetScore(0);
        chestBScript.SetScore(0);
        chestCommonScript.ResetScore();
        chestCommonScript.SetScore(0);
    }
}
