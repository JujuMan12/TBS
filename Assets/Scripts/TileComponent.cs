using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    [HideInInspector] public Tile tileData;
    [HideInInspector] private SpriteRenderer sprite;
    [HideInInspector] private Color defaultColor;

    private void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
    }

    private void FixedUpdate()
    {
        if (sprite.color != defaultColor && ShouldHaveDefaultColor())
        {
            ResetColor();
        }
    }

    private void OnMouseUp()
    {
        if (tileData.passable)
        {
            tileData.tileMap.GeneratePathTo(tileData.posX, tileData.posZ);
        }
        else
        {
            //TODO
        }
    }

    private void OnMouseEnter()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.75f);
        defaultColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.75f);
    }

    private void OnMouseExit()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.25f);
        defaultColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.25f);
    }

    public void SetColor(Color newColor)
    {
        sprite.color = newColor;
    }

    public void ResetColor()
    {
        sprite.color = defaultColor;
    }

    private bool ShouldHaveDefaultColor()
    {
        List<Tile> currentPath = tileData.tileMap.selectedUnit.currentPath;
        if (currentPath != null)
        {
            return !currentPath.Contains(tileData);
        }
        else
        {
            return true;
        }
    }
}
