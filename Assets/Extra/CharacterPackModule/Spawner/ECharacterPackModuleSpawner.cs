﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECharacterPackModuleSpawner : MonoBehaviour
{
    private void Start()
    {
        GameObject CharacterBasicModulePrefab = FindObjectOfType<CharacterBasicModuleNormalSpawner>().CharacterBasicModulePrefab;
        CharacterBasicModulePrefab.AddComponent<CCharacterPack>();
    }
}