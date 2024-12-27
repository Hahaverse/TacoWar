using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aeIKAYAKKI : Enemy
{
    public string scriptFile;
    public override string OnEncounter() //이벤트
    {
        return scriptFile; //반환값 = 다음 씬
    }
}
