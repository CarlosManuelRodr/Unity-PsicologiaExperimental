﻿using UnityEngine;

public enum Selectable
{
    PlayerA,
    PlayerB,
    Both
}

public enum Player
{
    PlayerA,
    PlayerB
}

/// <summary>
/// Gestor de cursores. Utiliza la biblioteca ManyMouse para gestionar el input de múltiples cursores.
/// </summary>
public class ManyCursorController : MonoBehaviour
{
    public Player player = Player.PlayerA;

    [Range(0.0f, 0.1f)]
    public float cursorSpeed = 0.03f;
    public Sprite handOpen, handClosed;
    public AudioClip grab, release;
    public Selectable selectable = Selectable.Both;

    public bool isSelecting { get { return selecting; } }
    public int speed
    {
        get { return (int)(100.0f * cursorSpeed / 0.1f); }
        set {
                if (value >= 0 && value <= 100)
                {
                    cursorSpeed = 0.1f * value / 100.0f;
                }
            }
    }

    private bool selecting;
    private bool disabled = false;

    private ManyMouse mouse;
    private SpriteRenderer spriteRenderer;
    private Camera cam;

    private GameObject selected;
    private AudioSource audioSource;
    private Rect playableArea;
    private Selectable initialSelectable;

    private void Awake()
    {
        initialSelectable = selectable;
        playableArea = Rect.MinMaxRect(-7.6f, -4.3f, 7.6f, 4.3f);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        selecting = false;
    }

    void SetUpCursor()
    {
        int mouseNumber;
        if (ManyMouseWrapper.MouseCount < 2)
        {
            if (player == Player.PlayerA)
            {
                mouseNumber = 0;
                mouse = ManyMouseWrapper.GetMouseByID(mouseNumber);
                mouse.EventButtonDown = delegate { };
                mouse.EventButtonUp = delegate { };
                mouse.EventButtonDown += CloseHand;
                mouse.EventButtonUp += OpenHand;
            }
            else
                disabled = true;
        }
        else
        {
            if (player == Player.PlayerA)
                mouseNumber = PlayerPrefs.GetInt("MouseIdA", 0);
            else
                mouseNumber = PlayerPrefs.GetInt("MouseIdB", 1);

            mouse = ManyMouseWrapper.GetMouseByID(mouseNumber);
            mouse.EventButtonDown = delegate { };
            mouse.EventButtonUp = delegate { };
            mouse.EventButtonDown += CloseHand;
            mouse.EventButtonUp += OpenHand;
        }
    }

    public void SetPlayableArea(Rect rect)
    {
        playableArea = rect;
    }

    public void SelectableSwitch()
    {
        if (selectable == initialSelectable)
            selectable = Selectable.Both;
        else
            selectable = initialSelectable;
    }

    void OnEnable()
    {
        this.SetUpCursor();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        selecting = false;
        selected = null;
        selectable = initialSelectable;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!disabled)
        {
            Vector3 cursorDelta = cursorSpeed * mouse.Delta;
            Vector3 mousePosition = transform.position + cursorDelta;

            if (playableArea.Contains(mousePosition))
            {
                this.transform.position = mousePosition;
                if (isSelecting && selected != null)
                    selected.transform.position = mousePosition;
            }
        }
    }

    public void CloseHand(ManyMouse mouse, int buttonId)
    {
        if (buttonId == 0)
        {
            if (selected != null)
            {
                audioSource.PlayOneShot(grab);
                selected.GetComponent<FruitController>().Select();
                selecting = true;
            }

            spriteRenderer.sprite = handClosed;
        }
    }

    public void OpenHand(ManyMouse mouse, int buttonId)
    {
        if (buttonId == 0)
        {
            if (selected != null)
            {
                audioSource.PlayOneShot(release);
                selected.GetComponent<FruitController>().Deselect();
            }

            selecting = false;
            spriteRenderer.sprite = handOpen;
        }
    }

    bool IsSelectable(Collider2D other)
    {
        if (selectable == Selectable.Both)
        {
            if (other.tag == "ItemA" || other.tag == "ItemB")
                return true;
            else
                return false;
        }
        else
        {
            if (selectable == Selectable.PlayerA && other.tag == "ItemA")
                return true;
            if (selectable == Selectable.PlayerB && other.tag == "ItemB")
                return true;
        }
        return false;
    }

    public void SetSelectable(Selectable type)
    {
        selectable = type;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSelecting && IsSelectable(other))
        {
            selected = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isSelecting)
            selected = null;
    }
}
