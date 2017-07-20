using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAction : ButtonAction
{
    public override void Action()
    {
        Application.Quit();
    }
}
