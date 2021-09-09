using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICurtain : UIManager.UIBehaviour
{
    protected override IEnumerator OnShow() => this.PlayAndAwait("Show");
    protected override IEnumerator OnHide() => this.PlayAndAwait("Hide");
}
