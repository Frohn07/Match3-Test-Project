using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private Crystal crystalPrefab;
    private Transform parentTransform;
    private Queue<Crystal> pool = new Queue<Crystal>();
    private List<Crystal> active = new List<Crystal>();

    public PoolManager(Crystal crystalPrefab, Transform parentTransform, int initCrystalCount)
    {
        this.crystalPrefab = crystalPrefab;
        this.parentTransform = parentTransform;

        for(int i = 0; i < initCrystalCount; i++)
        {
            CreateNewCrystal();
        }
    }

    private void CreateNewCrystal()
    {
        Crystal newCrystal = GameObject.Instantiate(crystalPrefab, parentTransform);
        newCrystal.gameObject.SetActive(false);
        pool.Enqueue(newCrystal);
    }

    public Crystal GetCrystal()
    {
        if (pool.Count == 0)
        {
            CreateNewCrystal();
        }

        Crystal crystal = pool.Dequeue();
        crystal.gameObject.SetActive(true);
        active.Add(crystal);
        return crystal;
    }

    public void ReturnCrystal(Crystal crystal)
    {
        if(crystal != null)
        {
            crystal.gameObject.SetActive(false);
            pool.Enqueue(crystal);
            active.Remove(crystal);
        }
    }

    public void ReturnAll()
    {
        for(int i = 0; i < active.Count; i++)
        {
            ReturnCrystal(active[i]);
        }
    }

}
