using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WPP.FileReader;

namespace WPP.DeckManagement
{
    public class CardDatabaseLoader : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onCardDatabaseLoaded;
        private void Start()
        {
            JsonParser.Instance().LoadDecks();
            JsonParser.Instance().LoadCardCollection();
            JsonParser.Instance().LoadCardInstances();

            _onCardDatabaseLoaded?.Invoke();
        }
    }
}
