using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "HiddenSeed/CardFactory", fileName = "CardFactory")]
public class CardFactory : ScriptableObject
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<Sprite> availableFronts = new();

    public Sprite[] GetAvailableFronts() => availableFronts.ToArray();

    public CardModel CreateCard(int id, Sprite frontSprite)
    {
        var go = Instantiate(cardPrefab);
        var controller = go.GetComponent<CardModel>();
        controller.SetData(id, frontSprite);

        return controller;
    }
}