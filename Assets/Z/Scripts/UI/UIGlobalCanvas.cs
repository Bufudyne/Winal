using System;
using System.Collections;
using System.Collections.Generic;
using DTT.Singletons;
using UnityEngine;

public class UIGlobalCanvas : SingletonBehaviour<UIGlobalCanvas>
{
    public UILoadingPanel LoadingPanel;

    public void Reset()
    {
        LoadingPanel.OnEnable();
    }
}
