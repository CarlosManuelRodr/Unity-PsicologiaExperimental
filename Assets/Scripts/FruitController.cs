﻿using UnityEngine;

/// <summary>
/// Controlador del fruto. Gestiona la interacción con el cursor y el cofre.
/// </summary>
public class FruitController : MonoBehaviour
{
    public GameObject highlight;
    public float returnSpeed = 1.0f;
    public bool isSelected { get { return selected; } }
    public bool isFalling { get { return falling; } }
    public bool isHighlighted { get { return highlight.activeSelf; } }

    private SpriteRenderer fruitRenderer, highlightRenderer = null;
    private Rigidbody2D rigidbody2d;
    private Camera cam;
    private Color red, yellow, green;
    private Vector3 startPos;
    private bool inChest;
    private bool returnToStart;
    private bool selected;
    private bool falling;
    private string selector = "";

    void Awake()
    {
        highlight.SetActive(false);
        highlightRenderer = highlight.GetComponent<SpriteRenderer>();
        fruitRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        // Colores del highlight. Usa colores predefinidos en caso de que
        // el parsing falle.
        if (!ColorUtility.TryParseHtmlString("#FF170BB6", out red))
            red = Color.red;
        if (!ColorUtility.TryParseHtmlString("#FFCA0BB6", out yellow))
            yellow = Color.yellow;
        if (!ColorUtility.TryParseHtmlString("#84FF0BB6", out green))
            green = Color.green;

        startPos = transform.position;
        inChest = false;
        returnToStart = false;
        selected = false;
        falling = false;
    }

    void Update()
    {
        if (returnToStart)
        {
            // Mueve fruto hasta que regrese a la posición inicial.
            float step = returnSpeed * Time.deltaTime;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, startPos, step);
            rigidbody2d.MovePosition(newPosition);


            if (transform.position == startPos)
            {
                returnToStart = false;
            }
        }
        else
        {
            // Asigna posición inicial en caso de que no haya sido asignada anteriormente.
            if (!selected && startPos != transform.position)
            {
                startPos = transform.position;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (highlightRenderer != null)
        {
            // Activa el highlight en caso de que el cursor entre en contacto con el fruto.
            if (other.tag == "CursorA" || other.tag == "CursorB")
            {
                ManyCursorController cursor = other.gameObject.GetComponent<ManyCursorController>();
                if (!cursor.isSelecting)
                    highlight.SetActive(true);

                selector = other.tag;
            }

            // En caso de que entre en contacto con un cofre, activa el highlight verde.
            if (other.tag == "Chest")
            {
                highlightRenderer.color = green;
                inChest = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (highlightRenderer != null)
        {
            if (other.tag == "CursorA" || other.tag == "CursorB" || other.tag == "AICursor")
            {
                highlight.SetActive(false);
            }

            if (other.tag == "Chest")
            {
                inChest = false;

                if (returnToStart)
                    highlightRenderer.color = red;
                else
                    highlightRenderer.color = yellow;
            }
        }
    }

    public void Select()
    {
        highlightRenderer.color = yellow;
        highlightRenderer.sortingOrder += 2;
        fruitRenderer.sortingOrder += 2;
        selected = true;
    }

    public void Deselect()
    {
        highlightRenderer.color = red;
        highlightRenderer.sortingOrder -= 2;
        fruitRenderer.sortingOrder -= 2;
        selected = false;

        if (transform.position != startPos)
        {
            // Si se deselecciona dentro de cofre, cae como cuerpo rígido. En caso contrario
            // regresa a la posición inicial.
            if (inChest)
            {
                rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
                falling = true;
            }
            else
                returnToStart = true;
        }
    }
}
