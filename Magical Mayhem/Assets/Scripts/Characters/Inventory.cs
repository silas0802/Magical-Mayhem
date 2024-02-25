using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class Inventory
{
public int gold=50;
public Item[] items = new Item[6];
public Spell[] spells = new Spell[6];



}