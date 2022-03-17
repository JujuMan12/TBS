using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    [HideInInspector] public Tile tileData;
    [HideInInspector] private TileMap tileMap;
    [HideInInspector] private SpriteRenderer sprite;
    [HideInInspector] private Color currentColor;
    [HideInInspector] private Color targetColor;
    [HideInInspector] public enum ColorState { none, impassable, available, path, enemy, ally, selected };
    [HideInInspector] public ColorState colorState;

    [Header("Colors")]
    [SerializeField] public Color defaultColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] public Color impassableColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] public Color availableColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField] public Color pathColor = new Color(1f, 1f, 0f, 0.25f);
    [SerializeField] public Color allyColor = new Color(0f, 0f, 1f, 0.25f);
    [SerializeField] public Color selectedColor = new Color(1f, 1f, 0f, 0.25f);

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        currentColor = sprite.color;
        targetColor = currentColor;
    }

    private void FixedUpdate()
    {
        SetTargetColor();

        if (currentColor != targetColor)
        {
            SetColor(targetColor);
        }
    }

    private void SetTargetColor()
    {
        if (colorState == ColorState.none)
        {
            targetColor = defaultColor;
        }
        else if (colorState == ColorState.impassable)
        {
            targetColor = impassableColor;
        }
        else if (colorState == ColorState.available)
        {
            targetColor = availableColor;
        }
        else if (colorState == ColorState.path)
        {
            targetColor = pathColor;
        }
        else if (colorState == ColorState.ally)
        {
            targetColor = allyColor;
        }
        else if (colorState == ColorState.selected)
        {
            targetColor = selectedColor;
        }
    }

    private void OnMouseUp()
    {
        if (tileData.unit != null) //TODO
        {
            tileMap.SelectUnit(tileData.unit.unitController);
        }
        else if (tileMap.selectedUnit != null && tileData.passable)
        {
            tileMap.GeneratePathTo(tileData.posX, tileData.posZ);
        }
        else
        {
            //TODO
        }
    }

    private void OnMouseOver()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.75f);
    }

    private void OnMouseExit()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.25f);
    }

    private void SetColor(Color newColor)
    {
        sprite.color = newColor;
        currentColor = newColor;
    }
}
