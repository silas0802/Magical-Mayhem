using System;
using System.Security.Cryptography;
using UnityEngine;

public class SeedGen
{
    //generate a seed based on the privious one by hashing it and saving it
    public float Seed(){
            float oldSeed = PlayerPrefs.HasKey("Seed")? PlayerPrefs.GetFloat("Seed") : RandomFloat();
            MD5 hash = MD5.Create();
            float newSeed = BitConverter.ToInt16(hash.ComputeHash(BitConverter.GetBytes(oldSeed)));
            hash.Dispose();
            newSeed /= 1000;
            PlayerPrefs.SetFloat("Seed", newSeed);
            return newSeed;
        }

    private float RandomFloat(){
        System.Random random = new();
        double dub = random.NextDouble();
        double dub2 = Math.Pow(2, random.Next(0, 8));
        return (float)(dub*dub2);
        
    }
}