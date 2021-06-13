using System.Collections.Generic;
using UnityEngine;

public class MapInfo {
    public int width, height;
   
    public class Pillar {
        public int x, y;
        public int height;
    }
    public Pillar[] pillars;

    public class MapRule {
        public int minPillarCount;
        public int maxPillarCount;
        public int minPillarHeight;
        public int maxPillarHeight;

        public int minTargetCount;
        public int maxTargetCount;
    }
    public MapRule mapRule;

    public class Target {
        public int x, y, z;
        public bool isMoving;
    }
    public Target[] targets;

    public MapInfo(int w, int h, MapRule rule) {
        width = w;
        height = w;
        mapRule = rule;

        try {
            Create();
        }
        catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }

    void Create() {
        if (mapRule == null) throw new System.Exception("mapRule == null");
        if (width <= 0 || height <= 0) throw new System.Exception("map's width or height lt/eq 0");

        List<List<int>> positions = new List<List<int>>();
        for (int i = 0; i < height; ++i) {
            positions.Add(new List<int>());
            for (int j = 0; j < width; ++j) {
                positions[i].Add(j);
            }
        }

        (int, int) RandomIndex() {
            int y = Random.Range(0, positions.Count);
            int x = positions[y][Random.Range(0, positions[y].Count)];
            positions[y].RemoveAt(x);
            if (positions[y].Count == 0) positions.RemoveAt(y);
            return (x, y);
        }

        pillars = new Pillar[Random.Range(mapRule.minPillarCount, mapRule.maxPillarCount + 1)];
        for (int i = 0; i < pillars.Length; ++i) {
            var pos = RandomIndex();
            pillars[i] = new Pillar() {
                x = pos.Item1,
                y = pos.Item2,
                height = Random.Range(mapRule.minPillarHeight, mapRule.maxPillarHeight + 1)
            };
        }

        targets = new Target[Random.Range(mapRule.minTargetCount, mapRule.maxTargetCount)];
        for (int i = 0; i < targets.Length; ++i) {
            var pos = RandomIndex();
            targets[i] = new Target() {
                x = pos.Item1,
                y = pos.Item2,
                z = Random.Range(0, mapRule.maxPillarHeight),
                isMoving = Random.Range(0F, 1F) < 0.3f
            };
        }
    }
}