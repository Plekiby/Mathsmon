using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HPbar hpBar;
    [SerializeField] private GameObject expBar;
    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
        SetExp();
    }

    public IEnumerator UpdateHP()
    {
        float normalizedHP = Mathf.Clamp((float)_pokemon.HP / _pokemon.MaxHp, 0f, 1f);
        yield return hpBar.SetHPSmooth(normalizedHP);
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }
    public void SetLevel()
    {
        levelText.text = "Lvl " + _pokemon.Level;
    }

    private float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.getcurrentWins();
        int nextLevelExp = _pokemon.getwinsRequiredForNextLevel();

        if (nextLevelExp <= currLevelExp) // Prevent division by zero
            return 1f;

        float normalizedExp = (float)currLevelExp / nextLevelExp;
        return Mathf.Clamp01(normalizedExp); // Ensure the result is between 0 and 1
    }
}