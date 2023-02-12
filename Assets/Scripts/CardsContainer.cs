using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WordsContainer")]
public class CardsContainer : ScriptableObject
{
    [SerializeField] private List<CardInfo> cardInfos;

    public List<CardInfo> CardInfos => cardInfos;
}

[Serializable]
public struct CardInfo
{
    public string name;
    public string description;
}
