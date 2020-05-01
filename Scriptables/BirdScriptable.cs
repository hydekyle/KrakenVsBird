using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Birds", menuName = "Scriptables/Birds")]
public class BirdScriptable : ScriptableObject
{
    public List<Sprite> eggs_sprites;
    public List<Bird> birds;
}