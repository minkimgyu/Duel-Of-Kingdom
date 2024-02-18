using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WPP.AI.STAT;
using WPP.Collection;

namespace WPP.DeckManagement.UI
{
    public class CardInfoUI : MonoBehaviour
    {
        [SerializeField] private GameObject _cardInfoPanel;
        [Space]
        [SerializeField] private CardUI _cardUI;
        [SerializeField] private TextMeshProUGUI _cardName;
        [Space]
        [SerializeField] private Transform StatUIPrefab;
        [SerializeField] private Transform StatUIParent;
        // Start is called before the first frame update

        public void ShowCardInfo(Card card, int level)
        {
            _cardInfoPanel.SetActive(true);
            DestroyStats();

            // get cardData
            var cardData = CardDatabase.GetCardData(card.id, level);
            BaseStat unitStat = cardData.unit;
            _cardName.text = unitStat._name;

            _cardUI.SetCard(card);

            if(unitStat is LifeStat)
            {
                AddStatUI("Hit Point", (unitStat as LifeStat)._hp.ToString("0.#"));

                if (unitStat is UnitStat)
                {

                    if((unitStat as UnitStat)._targetTag.Length == 1)
                    {
                        AddStatUI("Target", (unitStat as UnitStat)._targetTag[0].ToString());
                    }

                    
                    AddStatUI("Damage", (unitStat as UnitStat)._damage.ToString("0.#"));
                    AddStatUI("Hit Speed", (unitStat as UnitStat)._hitSpeed.ToString("0.#"));

                    //AddStatUI("Speed", (unitStat as UnitStat)._moveSpeed.ToString("0.#"));
                    AddStatUI("Range", (unitStat as UnitStat)._range.ToString("0.#"));
                }

                if (unitStat is AttackBuildingStat)
                {
                    AddStatUI("Damage", (unitStat as AttackBuildingStat)._damage.ToString("0.#"));
                    AddStatUI("Hit Speed", (unitStat as AttackBuildingStat)._hitSpeed.ToString("0.#"));

                    AddStatUI("Range", (unitStat as AttackBuildingStat)._range.ToString("0.#"));

                    if (unitStat is LivingOutAttackBuildingStat)
                    {
                        AddStatUI("Lifetime", (unitStat as LivingOutAttackBuildingStat)._lifeTime.ToString("0.#"));
                    }
                }

                if (unitStat is LivingOutSpawnBuildingStat)
                {
                    var livingOutSpawnBuildingStat = unitStat as LivingOutSpawnBuildingStat;
                    AddStatUI("Lifetime", livingOutSpawnBuildingStat._lifeTime.ToString("0.#"));

                    var spawnUnitData = CardCollection.Instance().FindCard(livingOutSpawnBuildingStat._spawnUnitId);
                    if (spawnUnitData != null)
                    {
                        AddStatUI("Spawns", string.Format("{0} x{1}", spawnUnitData.unit._name, livingOutSpawnBuildingStat._spawnUnitCount));
                    }

                    AddStatUI("Spawn Speed", livingOutSpawnBuildingStat._spawnDelay.ToString("0.#") + "s");
                }
            }

            if (unitStat is MagicStat)
            {
                AddStatUI("Range", (unitStat as MagicStat)._range.ToString("0.#"));

                if (unitStat is ProjectileMagicStat)
                {
                    AddStatUI("Damage", (unitStat as ProjectileMagicStat)._damage.ToString("0.#"));
                }
            }
        }

        private void DestroyStats()
        {
            foreach (var child in StatUIParent)
            {
                Destroy((child as Transform).gameObject);
            }
        }

        private void AddStatUI(string statName, string statValue)
        {
            var statUI = Instantiate(StatUIPrefab, StatUIParent);
            statUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = statName;
            statUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = statValue;
        }
    }
}
