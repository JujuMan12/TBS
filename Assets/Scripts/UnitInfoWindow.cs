using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoWindow : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private TileMap tileMap;

    [Header("Info")]
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI damageText;
    [SerializeField] private TMPro.TextMeshProUGUI dodgeText;
    [SerializeField] private TMPro.TextMeshProUGUI defenceText;

    private void FixedUpdate()
    {
        if (tileMap.selectedUnit != null)
        {
            nameText.text = tileMap.selectedUnit.unitName;
            damageText.text = $"{tileMap.selectedUnit.minDamage} - {tileMap.selectedUnit.maxDamage} ({tileMap.selectedUnit.aim * 100}%)";
            dodgeText.text = tileMap.selectedUnit.dodge * 100 + "%";
            defenceText.text = $"{tileMap.selectedUnit.defenceBonusMin} - {tileMap.selectedUnit.defenceBonusMax}";
        }
    }
}
