using UnityEngine;

public enum BombKind { Real, Fake, Gold, Big }

[System.Serializable]
public class BombStats
{
    public BombKind kind;
    public Color color = Color.white;
    public float fallSpeed = 2.5f;
    public int pointsOnCatch = 10;
    public int damageOnCatch = 0;
    public int damageOnMiss = 10;
    public Vector2 size = Vector2.one;
}