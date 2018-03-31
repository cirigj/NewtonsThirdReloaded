using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PowerUpChance {
    public PowerUp prefab;
    public float chanceMin;
    public float chanceMax;
}

[CreateAssetMenu(fileName = "LootTable", menuName = "Loot Table")]
public class LootTable : ScriptableObject {

    public List<PowerUpChance> powerUps;

    public List<PowerUp> GetLoot (int drops) {
        float rand = Random.Range(0f, 1f);
        return Enumerable.Range(0, drops).Aggregate(new List<PowerUp>(), (l, i) => {
            PowerUpChance choice = powerUps.Where(pc => rand >= pc.chanceMin && rand <= pc.chanceMax).FirstOrDefault();
            if (choice != null) {
                l.Add(choice.prefab);
            }
            else {
                Debug.LogErrorFormat("PowerUp chance '{0}' yields no result!", rand);
            }
            return l;
        });
    }

}
