using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillFeedElement : MonoBehaviour
{
    public TMP_Text fromText;
    public TMP_Text toText;
    public LoadImageFromUrl fromPFP;
    public LoadImageFromUrl toPFP;
    public void UpdateData(string from,string to, long fromId, long toId)
    {
        fromText.text = from;
        toText.text = to;

        fromPFP.UpdateImageUsingId(fromId);
        toPFP.UpdateImageUsingId(toId);
    }
}
