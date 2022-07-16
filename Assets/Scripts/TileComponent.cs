using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    [HideInInspector] public Tile tileData;
    [HideInInspector] private TileMap tileMap;
    [HideInInspector] private Color currentColor;
    [HideInInspector] private Color targetColor;
    [HideInInspector] public enum ColorState { none, impassable, available, range, path, enemy, ally, selected };
    [HideInInspector] public ColorState colorState;
    [HideInInspector] private float defaultOpacity;

    [Header("Colors")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float hoverOpacity = 0.75f;
    [SerializeField] public Color defaultColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] public Color impassableColor = new Color(0f, 0f, 0f, 0.25f);
    [SerializeField] public Color availableColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField] public Color rangeColor = new Color(1f, 0.5f, 0f, 0.25f);
    [SerializeField] public Color pathColor = new Color(1f, 1f, 0f, 0.25f);
    [SerializeField] public Color enemyColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] public Color allyColor = new Color(0f, 0f, 1f, 0.25f);
    [SerializeField] public Color selectedColor = new Color(1f, 1f, 0f, 0.25f);

    private void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        currentColor = sprite.color;
        defaultOpacity = sprite.color.a;
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
        switch (colorState)
        {
            case ColorState.none:
                targetColor = defaultColor;
                break;
            case ColorState.impassable:
                targetColor = impassableColor;
                break;
            case ColorState.available:
                targetColor = availableColor;
                break;
            case ColorState.range:
                targetColor = rangeColor;
                break;
            case ColorState.path:
                targetColor = pathColor;
                break;
            case ColorState.enemy:
                targetColor = enemyColor;
                break;
            case ColorState.ally:
                targetColor = allyColor;
                break;
            case ColorState.selected:
                targetColor = selectedColor;
                break;
            default: break;
        }
    }

    private void OnMouseOver()
    {
        if (tileData.unit != null)
        {
            tileData.unit.unitController.OnMouseOver();
        }
        else if (Input.GetButtonDown("Order Unit") && tileMap.PlayerCanOrder() && tileMap.actionState == TileMap.ActionState.movement)
        {
            tileMap.GeneratePathTo(tileData.posX, tileData.posZ, tileMap.selectedUnit, false);
        }
    }

    private void OnMouseEnter()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, hoverOpacity);
    }

    private void OnMouseExit()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, defaultOpacity);
    }

    private void SetColor(Color newColor)
    {
        sprite.color = newColor;
        currentColor = newColor;
    }
}
