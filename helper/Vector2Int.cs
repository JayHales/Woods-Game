/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
public struct Vector2Int {
    public readonly int X;
    public readonly int Y;

    public Vector2Int(int x, int y) {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return "{X: " + X + ", Y: " + Y + "}";
    }

    public Vector2 ToVector2() {
        return new Vector2(X, Y);
    }

    public static Vector2Int operator* (Vector2Int a, int b) {
        return new Vector2Int(a.X * b, a.Y * b); 
    }

    public static Vector2Int operator+ (Vector2Int a, Vector2Int b) {
        return new Vector2Int(a.X + b.X, a.Y + b.Y); 
    }

    public static readonly Vector2Int Zero = new Vector2Int(0, 0);
    public static readonly Vector2Int One = new Vector2Int(1, 1);

    public static readonly Vector2Int Right = new Vector2Int(1, 0);
    public static readonly Vector2Int Left = new Vector2Int(-1, 0);    
    public static readonly Vector2Int Up = new Vector2Int(0, 1);
    public static readonly Vector2Int Down = new Vector2Int(0, -1);
}*/