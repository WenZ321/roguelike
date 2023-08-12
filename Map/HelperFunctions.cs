using System.Collections.Generic;
using UnityEngine;

static public class HelperFunctions
{
    public static void GenerateTunnel(Vector2Int start, Vector2Int end, List<Vector2Int> coords)
    {
        int x = start.x,
            y = start.y;
        int x2 = end.x,
            y2 = end.y;
        while (true)
        {
            coords.Add(new Vector2Int(x, y));
            if (x == x2 && y == y2)
            {
                break;
            }

            if (x == x2)
            {
                if (y > y2)
                {
                    coords.Add(new Vector2Int(x + 1, y));
                    y -= 1;
                }
                else
                {
                    coords.Add(new Vector2Int(x + 1, y));
                    y += 1;
                }
            }
            if (y == y2)
            {
                if (x > x2)
                {
                    coords.Add(new Vector2Int(x, y + 1));
                    x -= 1;
                }
                else
                {
                    coords.Add(new Vector2Int(x, y + 1));
                    x += 1;
                }
            }
        }
    }

    public static void GenerateCoords(List<Vector2Int> coords, int mapWidth, int mapHeight, int maxRooms, int roomMaxSize)
    {
        coords.Add(new Vector2Int(mapWidth / 2, mapHeight / 2));
        for (int i = 1; i < maxRooms; i++)
        {
            int direction = UnityEngine.Random.Range(1, 5);
            switch (direction)
            {
                case 1: //Moving right
                    int x = coords[i - 1].x + UnityEngine.Random.Range(roomMaxSize, roomMaxSize + 6);
                    int y = coords[i - 1].y;
                    coords.Add(new Vector2Int(x, y));
                    break;
                case 2: //Moving left
                    int x2 = coords[i - 1].x - UnityEngine.Random.Range(roomMaxSize, roomMaxSize + 6);
                    int y2 = coords[i - 1].y;
                    coords.Add(new Vector2Int(x2, y2));
                    break;
                case 3: //Moving up
                    int x3 = coords[i - 1].x;
                    int y3 = coords[i - 1].y + UnityEngine.Random.Range(roomMaxSize, roomMaxSize + 6);
                    coords.Add(new Vector2Int(x3, y3));
                    break;
                case 4: //Moving down
                    int x4 = coords[i - 1].x;
                    int y4 = coords[i - 1].y - UnityEngine.Random.Range(roomMaxSize, roomMaxSize + 6);
                    coords.Add(new Vector2Int(x4, y4));
                    break;
                default:
                    break;
            }
            if (coords[i].x > mapWidth - roomMaxSize || coords[i].x < 0 || coords[i].y > mapHeight - roomMaxSize || coords[i].y < 0)
            {
                coords.Remove(new Vector2Int(coords[i].x, coords[i].y));
                i--;
            }
        }
    }

    public static string Choice(this System.Random rnd, IEnumerable<string> choices, IEnumerable<int> weights)     //This does the same thing as random.choices in python 
    {                                                                                                       //Returns a random list given items and the percentage they are weighed 
        var cumulativeWeight = new List<int>();
        int last = 0;
        foreach (var cur in weights)
        {
            last += cur;
            cumulativeWeight.Add(last);
        }
        int choice = rnd.Next(last);
        int i = 0;
        foreach (var cur in choices)
        {
            if (choice < cumulativeWeight[i])
            {
                return cur;
            }
            i++;
        }
        return null;
    }

    public static List<string> Choices(this System.Random rnd, IEnumerable<string> choices, IEnumerable<int> weights, int maxChoices)
    {
        var result = new List<string>();
        for (int i = 0; i < maxChoices; i++)
        {
            result.Add(rnd.Choice(choices, weights));
        }
        return result;
    }
}