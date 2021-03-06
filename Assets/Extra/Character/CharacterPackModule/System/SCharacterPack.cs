﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.Events;

public class SCharacterPack : ComponentSystem
{
    public CharacterPackMenuController characterPackController;
    private struct TempItem
    {
        public string Name;
        public int Value;
    }
    protected override void OnUpdate()
    {
        PackPageJob();
        CharacterPackTaskJob();
    }

    public void CharacterPackTaskJob()
    {
        Entities.ForEach((CCharacterPack characterPack) =>
        {
            while (characterPack.TaskList.Count > 0)
            {
                var task = characterPack.TaskList.Pop();
                if (CharacterPackRemoveItem(characterPack, task.Losing))
                    CharacterPackAddItem(characterPack, task.Getting);
            }
        });
    }

    public void PackPageJob()
    {
        if( characterPackController!=null&&characterPackController.gameObject.activeSelf)
        Entities.ForEach((CCharacterPack pack,CCharacterBasicModule basic) =>
        {
            if (basic.isSelected)
            {
                List<TempItem> tempItemList = new List<TempItem>();
                foreach (var item in pack.Pack)
                {
                    tempItemList.Add(new TempItem { Name = item.Key, Value = item.Value });
                }
                for (int i = 0; i < characterPackController.characterPackMenuItemList.Length; i++)
                {
                    //因为dictionary是没有顺序的，如果点击按钮的途中添加按钮会出现问题，推荐是用stack
                    if (i < pack.Pack.Count)
                    {
                        characterPackController.characterPackMenuItemList[i].buttonText.text = tempItemList[i].Name;
                        characterPackController.characterPackMenuItemList[i].countText.text = tempItemList[i].Value.ToString();
                        characterPackController.characterPackMenuItemList[i].characterBasicModule = basic;
                        characterPackController.characterPackMenuItemList[i].characterPack = pack;
                        characterPackController.characterPackMenuItemList[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        characterPackController.characterPackMenuItemList[i].gameObject.SetActive(false);
                    }
                }
            }

        });

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!characterPackController.gameObject.activeSelf)
            {
                characterPackController.gameObject.SetActive(true);
            }
            else
            {
                characterPackController.gameObject.SetActive(false);
            }
        }
    }

    public void CharacterPackUpdataJob()
    {
        Entities.ForEach((CCharacterPack characterPack) =>
        {
            while (characterPack.TaskList.Count != 0)
            {
                TCharacterPack tempTask = characterPack.TaskList.Pop();
                if (CharacterPackRemoveItem(characterPack, tempTask.Losing))
                {
                    CharacterPackAddItem(characterPack, tempTask.Getting);
                }
                
            }

        });
    }

    public void CharacterPackAddItem(CCharacterPack characterPack, params string[] items)
    {
        foreach (var item in items)
        {
            if (characterPack.Pack.ContainsKey(item))
                characterPack.Pack[item]++;
            else
            {
                Debug.Log("添加新物品");
                characterPack.Pack.Add(item, 1);
            }
        }
    }

    public bool CharacterPackRemoveItem(CCharacterPack characterPack, params string[] items)
    {
        if (items.Length == 0)
        {
            return true;
        }
        UnityAction ua = delegate { };

        foreach (var item in items)
        {
            if (characterPack.Pack.ContainsKey(item))
            {
                if (characterPack.Pack[item] - 1 == 0)
                {
                    ua += delegate { characterPack.Pack.Remove(item); };
                }
                else if (characterPack.Pack[item] - 1 < 0)
                {
                    return false;
                }
                else
                {
                    ua += delegate { characterPack.Pack[item]--; };
                }
            }
            else
                return false;
        }
        ua.Invoke();
        return true;
    }
}
