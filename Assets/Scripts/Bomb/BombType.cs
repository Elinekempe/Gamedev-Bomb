using UnityEngine;

// Elk type heeft eigen regels voor snelheid, score, schade en uiterlijk.
public enum BombKind { Real, Fake, Gold, Big }

[System.Serializable]
public class BombStats
{
    // Deze data wordt per type in de GameManager ingesteld via de Inspector.
    public BombKind kind;
    public Color color;
    public float fallSpeed;
    public float horizontalSpeed;
    public int pointsOnCatch;
    public int pointsOnMiss;
    public int damageOnCatch;
    public int damageOnMiss;
    public Vector2 size;
}