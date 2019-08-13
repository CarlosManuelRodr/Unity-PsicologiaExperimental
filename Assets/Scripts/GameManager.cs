using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Gestiona la ejecución del juego. Presente durante toda la ejecución.
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject experiment;
    public GameObject eventSystem;
    public GameObject chestClearer;
    public bool debug = false;
    public uint rounds = 3;

    private ExperimentManager experimentManager;
    private Parallax parallax;
    private MainMenu mainMenuScript;
    private Jukebox jukebox;
    private ChestClear chestClearerScript;

    private bool prepareStart;
    private bool inExperiment;
    private bool clearing;
    private int currentID;
    private int currentRound;
    private uint totalScoreA, totalScoreB, totalScoreCommon;

    private uint param_playerAFruits, param_playerBFruits, param_speedA, param_speedB;
    private bool param_commonCounter;

    void Start()
    {
        // Adquiere referencias a componentes.
        parallax = GetComponent<Parallax>();
        jukebox = GetComponent<Jukebox>();
        chestClearerScript = chestClearer.GetComponent<ChestClear>();

        // Carga configuración de volumen de las preferencias de usuario.
        int musicVolume = PlayerPrefs.GetInt("Volume", -1);
        if (musicVolume == -1)
            musicVolume = 100;
        if (jukebox != null)
            jukebox.volume = (float) musicVolume;

        if (mainMenu != null)
        {
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
        }

        experimentManager = experiment.GetComponent<ExperimentManager>();

        // Inicia automáticamente el experimento en la configuración debug.
        // Usado en escena de depuración "Experiment".
        if (debug)
        {
            this.StartExperiment(3, 2, 30, 30, true);
            experimentManager.ActivateCursors();
            currentRound = 1;
        }

        prepareStart = false;
        inExperiment = false;
        clearing = false;
        totalScoreA = 0;
        totalScoreB = 0;
        totalScoreCommon = 0;
    }

    void Update()
    {
        // Bloquea sistema de eventos si el parallax está en movimiento.
        if (eventSystem != null)
        {
            if (eventSystem.activeSelf && parallax.isMoving)
                eventSystem.SetActive(false);
            if (!eventSystem.activeSelf && !parallax.isMoving)
                eventSystem.SetActive(true);
        }

        // Cuando el parallax deja de moverse, activa el nivel.
        if (prepareStart && !parallax.isMoving)
        {
            experimentManager.ActivateCursors();
            inExperiment = true;
            prepareStart = false;
        }

        // Tecla ESC configurada para que los experimentadores interumpan el experimento.
        if (inExperiment)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                this.EndExperiment();
        }

        // Si la limpieza del cofre ha terminado, termina juego.
        if (clearing)
        {
            if (!chestClearerScript.isRunning)
            {
                this.EndGame();
                clearing = false;
                chestClearerScript.StopClear();
            }
        }
    }

    public void StartExperiment(
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB, 
        bool commonCounter
        )
    {
        currentRound = 1;    // Comienza en ronda 1 de 3.

        totalScoreA = 0;
        totalScoreB = 0;
        totalScoreCommon = 0;

        // Inicia experimento y guarda configuración para rondas posteriores.
        parallax.MoveDown();
        experiment.SetActive(true);
        experimentManager.InitializeExperiment(playerAFruits, playerBFruits, speedA, speedB, commonCounter);

        param_playerAFruits = playerAFruits;
        param_playerBFruits = playerBFruits;
        param_speedA = speedA;
        param_speedB = speedB;
        param_commonCounter = commonCounter;

        prepareStart = true;
    }

    public void EndGame()
    {
        experimentManager.DeactivateCursors();
        parallax.MoveUp();
        experimentManager.StopExperiment();
        experiment.SetActive(false);

        inExperiment = false;
        totalScoreA = 0;
        totalScoreB = 0;

        if (mainMenu != null)
            mainMenuScript.EnableMenu(true);
    }

    public void EndExperiment()
    {
        if (!clearing)
        {
            // Reinicia experimento hasta llegar a la ronda 3.
            totalScoreA += experimentManager.scoreA;
            totalScoreB += experimentManager.scoreB;
            totalScoreCommon += experimentManager.scoreCommon;

            if (currentRound == rounds)
            {
                clearing = true;
                chestClearerScript.StartClear(totalScoreA, totalScoreB, totalScoreCommon);
            }
            else
            {
                currentRound++;

                experimentManager.StopExperiment();

                experimentManager.InitializeExperiment(
                    param_playerAFruits, param_playerBFruits, param_speedA, param_speedB,
                    param_commonCounter
                    );
            }
        }
    }
}
