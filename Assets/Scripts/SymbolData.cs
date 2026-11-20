using UnityEngine;

//This script describes one symbol and it could be a Seven, Bar, Bell, Cherry.
[CreateAssetMenu(fileName = "NewSymbol", menuName = "Slot Machine/Symbol Data")]
public class SymbolData : ScriptableObject
{
    public Sprite sprite;            
    public string symbolName;        // "Seven", "Cherry"...

    [Min(0)] public int payoutMultiplier = 5;  // win = bet x this
    [Min(1)] public int weight = 10;           // higher = appears more often
}