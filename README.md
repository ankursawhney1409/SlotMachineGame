# Slot Machine – Unity WebGL

3-reel slot machine made in Unity 2022.3 (C#).

How to play: choose bet with arrows, press SPIN or pull the lever.
3 matching symbols = bet x payout (Seven 50x, Bar 20x, Bell 10x, Cherry 5x).

Run: Build/WebGL needs a local server (e.g. python3 -m http.server inside Build/WebGL),
or open Assets/Scenes/SampleScene in Unity and press Play.

Scripts are in Assets/Scripts. Symbol payouts and chances are ScriptableObjects in Assets/Data.